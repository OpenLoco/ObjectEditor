using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenLoco.Dat.FileParsing
{
	public static class ObjectAnnotator
	{
		public static IList<HexAnnotation> Annotate(byte[] byteList)
		{
			var annotations = new List<HexAnnotation>();
			var runningCount = 0;

			// S5 Header Annotations
			var s5HeaderAnnotation = new HexAnnotation("S5 Header", 0, S5Header.StructLength);
			annotations.Add(s5HeaderAnnotation);
			annotations.Add(new HexAnnotation("Flags", s5HeaderAnnotation, 0, 4));
			annotations.Add(new HexAnnotation("Name: '" + Encoding.ASCII.GetString(byteList[4..12]) + "'", s5HeaderAnnotation, 4, 8));
			annotations.Add(new HexAnnotation("Checksum", s5HeaderAnnotation, 12, 4));

			var s5Header = S5Header.Read(byteList.AsSpan()[0..S5Header.StructLength]);
			runningCount += S5Header.StructLength;

			// Object Header Annotations
			var objectHeaderAnnotation = new HexAnnotation("Object Header", runningCount, ObjectHeader.StructLength);
			annotations.Add(objectHeaderAnnotation);
			annotations.Add(new HexAnnotation("Encoding", objectHeaderAnnotation, runningCount, 1));
			annotations.Add(new HexAnnotation("Data Length", objectHeaderAnnotation, runningCount + 1, 4));
			var objectHeader = ObjectHeader.Read(byteList.AsSpan()[runningCount..(runningCount + ObjectHeader.StructLength)]);
			runningCount += ObjectHeader.StructLength;

			// Decode Loco Struct
			byte[] fullData =
			[
				.. byteList[..runningCount],
				.. SawyerStreamReader.Decode(objectHeader.Encoding, byteList.AsSpan()[runningCount..(int)(runningCount + objectHeader.DataLength)]),
			];

			var locoStruct = SawyerStreamReader.GetLocoStruct(s5Header.ObjectType, fullData.AsSpan()[runningCount..]);
			ArgumentNullException.ThrowIfNull(locoStruct);

			var structSize = AttributeHelper.Get<LocoStructSizeAttribute>(locoStruct.GetType());
			var locoStructSize = structSize!.Size;

			var locoStructAnnotation = new HexAnnotation("Loco Struct", runningCount, locoStructSize);
			annotations.Add(locoStructAnnotation);
			annotations.AddRange(AnnotateProperties(locoStruct, runningCount, locoStructAnnotation));

			runningCount += structSize!.Size;

			// String Table
			runningCount = AnnotateStringTable(fullData, runningCount, locoStruct, annotations);

			ReadOnlySpan<byte> remainingData = fullData.AsSpan()[runningCount..];
			var currentRemainingData = remainingData.Length;
			if (locoStruct is ILocoStructVariableData locoStructExtra)
			{
				remainingData = locoStructExtra.Load(remainingData);
			}

			annotations.Add(new HexAnnotation("Loco Variables", runningCount, currentRemainingData - remainingData.Length));
			runningCount += currentRemainingData - remainingData.Length;

			annotations.AddRange(AnnotateG1Data(fullData, runningCount));

			return annotations;
		}

		public static IList<HexAnnotation> AnnotateG1Data(byte[] fullData, int runningCount = 0)
		{
			var g1Annotation = new HexAnnotation("G1", runningCount, fullData.Length - runningCount);
			var g1HeaderAnnotation = new HexAnnotation("Header", g1Annotation, runningCount, 8);
			var annotations = new List<HexAnnotation>();

			if (runningCount < fullData.Length)
			{
				annotations.Add(g1Annotation);
				annotations.Add(g1HeaderAnnotation);
				annotations.Add(new HexAnnotation("Number Of Entries", g1HeaderAnnotation, runningCount, sizeof(uint32_t)));
				annotations.Add(new HexAnnotation("Total Size", g1HeaderAnnotation, runningCount + sizeof(uint32_t), sizeof(uint32_t)));

				var g1Header = new G1Header(
					BitConverter.ToUInt32(fullData.AsSpan()[runningCount..(runningCount + 4)]),
					BitConverter.ToUInt32(fullData.AsSpan()[runningCount..(runningCount + 8)]));

				runningCount += 8;

				var g1DataAnnotation = new HexAnnotation("Data", g1Annotation, runningCount, 1)
				{
					End = fullData.Length
				};

				var gHeadersAnnotation = new HexAnnotation("Headers", g1DataAnnotation, runningCount, 1);
				annotations.Add(g1DataAnnotation);
				annotations.Add(gHeadersAnnotation);

				var g32elements = new List<G1Element32>();

				for (var i = 0; i < g1Header.NumEntries; i++)
				{
					var g32Element = ByteReader.ReadLocoStruct<G1Element32>(fullData.AsSpan()[runningCount..]);
					var g32ElementAnnotation = new HexAnnotation("Header " + (i + 1), gHeadersAnnotation, runningCount, G1Element32.StructLength);

					annotations.Add(g32ElementAnnotation);
					annotations.AddRange(AnnotateProperties(g32Element, runningCount, g32ElementAnnotation));

					g32elements.Add(g32Element);
					runningCount += G1Element32.StructLength;
				}

				gHeadersAnnotation.End = runningCount;

				var imageDataStart = runningCount;
				var g1ImageDataAnnotation = new HexAnnotation("Images", g1DataAnnotation, runningCount, 8);

				annotations.Add(g1ImageDataAnnotation);
				g1ImageDataAnnotation.End = fullData.Length;

				for (var i = 0; i < g32elements.Count; i++)
				{
					var imageStart = imageDataStart + (int)g32elements[i].Offset;
					var imageSize = fullData.Length - imageStart;
					if (i + 1 < g32elements.Count)
					{
						imageSize = (int)g32elements[i + 1].Offset - (int)g32elements[i].Offset;
					}

					annotations.Add(new HexAnnotation("Image " + (i + 1), g1ImageDataAnnotation, imageStart, imageSize));
				}
			}

			return annotations;
		}

		static int AnnotateStringTable(byte[] fullData, int runningCount, ILocoStruct locoStruct, List<HexAnnotation> annotations)
		{
			var root = new HexAnnotation("String Table", runningCount, 1);
			annotations.Add(root);

			var locoStructType = locoStruct.GetType();
			var stringTableStrings = AttributeHelper.Has<LocoStringTableAttribute>(locoStructType)
				? AttributeHelper.Get<LocoStringTableAttribute>(locoStructType)!.Strings
				: AttributeHelper.GetAllPropertiesWithAttribute<LocoStringAttribute>(locoStructType).Select(s => s.Name).ToArray();

			var i = 0;
			foreach (var locoString in stringTableStrings)
			{
				var index = 0;
				var continuing = true;
				do
				{
					index += Array.IndexOf(fullData[(runningCount + index)..], (byte)0);
					// The terminating sequence is actually 0x00 0xFF as
					// 0xFF is a character in the encoding scheme
					if (fullData[runningCount + ++index] != 0xFF)
					{
						index++;
					}
					else
					{
						continuing = false;
					}
				}
				while (continuing);

				var endIndexOfStringList = index + runningCount;
				var elementRoot = new HexAnnotation("Element " + i++, root, runningCount, index);
				annotations.Add(elementRoot);

				do
				{
					annotations.Add(new HexAnnotation(((LanguageId)fullData[runningCount]).ToString(), elementRoot, runningCount, 1));
					runningCount++;
					var nullIndex = Array.IndexOf(fullData[runningCount..], (byte)0);

					var stringElement = Encoding.ASCII.GetString(fullData[runningCount..(runningCount + nullIndex)]);

					annotations.Add(new HexAnnotation($"'{stringElement}'", elementRoot, runningCount, stringElement.Length + 1));
					runningCount += nullIndex + 1;
				} while (runningCount < endIndexOfStringList);

				runningCount = endIndexOfStringList + 1;
			}

			root.End = runningCount;
			return runningCount;
		}

		static List<HexAnnotation> AnnotateProperties(object o, int runningCount = 0, HexAnnotation? root = null)
		{
			var annotations = new List<HexAnnotation>();

			foreach (var p in o.GetType().GetProperties())
			{
				var offset = p.GetCustomAttribute<LocoStructOffsetAttribute>();
				if (offset != null)
				{
					var location = runningCount + offset!.Offset;

					var propType = p.PropertyType;

					// should probably use recursion
					while (propType.IsGenericType)
					{
						propType = propType.GenericTypeArguments[0];
					}

					while (propType?.IsArray == true)
					{
						propType = propType.GetElementType();
					}

					if (propType?.IsEnum == true)
					{
						propType = propType.GetEnumUnderlyingType();
					}

					if (propType == null)
					{
						continue;
					}

					var locoSize = propType.GetCustomAttribute<LocoStructSizeAttribute>();

					var length = locoSize != null
						? locoSize.Size
						: Marshal.SizeOf(propType);

					var lengthAttr = p.GetCustomAttribute<LocoArrayLengthAttribute>();
					if (lengthAttr != null)
					{
						length *= lengthAttr.Length;
					}

					annotations.Add(new HexAnnotation(p.Name, root, location, length));
				}
			}

			return annotations;
		}
	}
}
