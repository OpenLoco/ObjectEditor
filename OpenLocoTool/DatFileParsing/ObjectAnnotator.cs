using System.Diagnostics;
using System.Reflection;
using System.Text;
using OpenLocoTool.Headers;

namespace OpenLocoTool.DatFileParsing
{
	public static class ObjectAnnotator
	{
		public static IList<Annotation> Annotate(byte[] bytelist, out byte[] fullData)
		{
			var annotations = new List<Annotation>();
			var runningCount = 0;

			// S5 Header Annotations
			var s5HeaderAnnotation = new Annotation("S5 Header", 0, S5Header.StructLength);
			annotations.Add(s5HeaderAnnotation);
			annotations.Add(new Annotation("Flags", s5HeaderAnnotation, 0, 4));
			annotations.Add(new Annotation("Name: '" + Encoding.ASCII.GetString(bytelist[4..12]) + "'", s5HeaderAnnotation, 4, 8));
			annotations.Add(new Annotation("Checksum", s5HeaderAnnotation, 12, 4));

			var s5Header = S5Header.Read(bytelist.AsSpan()[0..S5Header.StructLength]);
			runningCount += S5Header.StructLength;

			// Object Header Annotations
			var objectHeaderAnnotation = new Annotation("Object Header", runningCount, ObjectHeader.StructLength);
			annotations.Add(objectHeaderAnnotation);
			annotations.Add(new Annotation("Encoding", objectHeaderAnnotation, runningCount, 1));
			annotations.Add(new Annotation("Data Length", objectHeaderAnnotation, runningCount + 1, 4));
			var objectHeader = ObjectHeader.Read(bytelist.AsSpan()[runningCount..(runningCount + ObjectHeader.StructLength)]);
			runningCount += ObjectHeader.StructLength;

			// Decode Loco Struct
			fullData = bytelist[..runningCount]
				.Concat(SawyerStreamReader.Decode(objectHeader.Encoding, bytelist.AsSpan()[runningCount..(int)(runningCount + objectHeader.DataLength)]))
				.ToArray();

			var locoStruct = SawyerStreamReader.GetLocoStruct(s5Header.ObjectType, fullData.AsSpan()[runningCount..]);
			if (locoStruct == null)
			{
				Debugger.Break();
				throw new NullReferenceException("loco object was null");
			}

			var structSize = AttributeHelper.Get<LocoStructSizeAttribute>(locoStruct.GetType());
			var locoStructSize = structSize!.Size;

			var locoStructAnnotation = new Annotation("Loco Struct", runningCount, locoStructSize);
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

			annotations.Add(new Annotation("Loco Variables", runningCount, currentRemainingData - remainingData.Length));
			runningCount += currentRemainingData - remainingData.Length;

			annotations.AddRange(AnnotateG1Data(fullData, runningCount));

			return annotations;
		}

		public static IList<Annotation> AnnotateG1Data(byte[] fullData, int runningCount = 0)
		{
			var g1Annotation = new Annotation("G1", runningCount, fullData.Length - runningCount);
			var g1HeaderAnnotation = new Annotation("Header", g1Annotation, runningCount, 8);
			var annotations = new List<Annotation>();

			if (runningCount < fullData.Length)
			{
				annotations.Add(g1Annotation);
				annotations.Add(g1HeaderAnnotation);
				annotations.Add(new Annotation("Number Of Entries", g1HeaderAnnotation, runningCount, sizeof(uint32_t)));
				annotations.Add(new Annotation("Total Size", g1HeaderAnnotation, runningCount + sizeof(uint32_t), sizeof(uint32_t)));

				var g1Header = new G1Header(
					BitConverter.ToUInt32(fullData.AsSpan()[runningCount..(runningCount + 4)]),
					BitConverter.ToUInt32(fullData.AsSpan()[runningCount..(runningCount + 8)]));

				runningCount += 8;

				var g1DataAnnotation = new Annotation("Data", g1Annotation, runningCount, 1)
				{
					End = fullData.Length
				};

				var gHeadersAnnotation = new Annotation("Headers", g1DataAnnotation, runningCount, 1);
				annotations.Add(g1DataAnnotation);
				annotations.Add(gHeadersAnnotation);

				var g32elements = new List<G1Element32>();

				for (var i = 0; i < g1Header.NumEntries; i++)
				{
					var g32Element = (G1Element32)ByteReader.ReadLocoStruct<G1Element32>(fullData.AsSpan()[runningCount..]);
					var g32ElementAnnotation = new Annotation("Header " + (i + 1), gHeadersAnnotation, runningCount, G1Element32.StructLength);

					annotations.Add(g32ElementAnnotation);
					annotations.AddRange(AnnotateProperties(g32Element, runningCount, g32ElementAnnotation));

					g32elements.Add(g32Element);
					runningCount += G1Element32.StructLength;
				}

				gHeadersAnnotation.End = runningCount;

				var imageDataStart = runningCount;
				var g1ImageDataAnnotation = new Annotation("Images", g1DataAnnotation, runningCount, 8);

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

					annotations.Add(new Annotation("Image " + (i + 1), g1ImageDataAnnotation, imageStart, imageSize));
				}
			}

			return annotations;
		}

		public static int AnnotateStringTable(byte[] fullData, int runningCount, ILocoStruct locoStruct, IList<Annotation> annotations)
		{
			var root = new Annotation("String Table", runningCount, 1);
			annotations.Add(root);

			var stringAttr = locoStruct.GetType().GetCustomAttribute(typeof(LocoStringTableAttribute), inherit: false) as LocoStringTableAttribute;
			var stringsInTable = stringAttr?.Count ?? 0;

			for (var i = 0; i < stringsInTable; i++)
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
				var nullIndex = 0;
				var elementRoot = new Annotation("Element " + i, root, runningCount, index);
				annotations.Add(elementRoot);

				do
				{
					annotations.Add(new Annotation(((LanguageId)fullData[runningCount]).ToString(), elementRoot, runningCount, 1));
					runningCount++;
					nullIndex = Array.IndexOf(fullData[runningCount..], (byte)0);

					var stringElement = Encoding.ASCII.GetString(fullData[runningCount..(runningCount + nullIndex)]);

					annotations.Add(new Annotation($"'{stringElement}'", elementRoot, runningCount, stringElement.Length + 1));
					runningCount += nullIndex + 1;
				} while (runningCount < endIndexOfStringList);

				runningCount = endIndexOfStringList + 1;
			}

			root.End = runningCount;
			return runningCount;
		}

		static IList<Annotation> AnnotateProperties(object o, int runningCount = 0, Annotation? root = null)
		{
			var annotations = new List<Annotation>();

			foreach (var p in o.GetType().GetProperties())
			{
				var offset = p.GetCustomAttribute<LocoStructOffsetAttribute>();
				if (offset != null)
				{
					var location = runningCount + offset!.Offset;
					annotations.Add(new Annotation(p.Name, root, location, 1));
				}
			}

			return annotations;
		}
	}
}