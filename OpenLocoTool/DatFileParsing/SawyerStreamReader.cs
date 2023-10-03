﻿using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Linq;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using System.Runtime.InteropServices;

namespace OpenLocoTool.DatFileParsing
{
	public class SawyerStreamReader
	{
		private readonly ILogger Logger;

		public SawyerStreamReader(ILogger logger)
			=> Logger = logger;

		static uint ComputeObjectChecksum(ReadOnlySpan<byte> flagByte, ReadOnlySpan<byte> name, ReadOnlySpan<byte> data)
		{
			uint32_t ComputeChecksum(ReadOnlySpan<byte> data, uint32_t seed)
			{
				var checksum = seed;
				foreach (var d in data)
				{
					checksum = BitOperations.RotateLeft(checksum ^ d, 11);
				}

				return checksum;
			}

			const uint32_t objectChecksumMagic = 0xF369A75B;
			var checksum = ComputeChecksum(flagByte, objectChecksumMagic);
			checksum = ComputeChecksum(name, checksum);
			checksum = ComputeChecksum(data, checksum);
			return checksum;
		}

		public IG1Dat LoadG1(string filename)
		{
			ReadOnlySpan<byte> fullData = LoadBytesFromFile(filename);
			var (g1Header, imageTable, imageTableBytesRead) = LoadImageTable(fullData);
			Logger.Log(LogLevel.Info, $"FileLength={new FileInfo(filename).Length} NumEntries={g1Header.NumEntries} TotalSize={g1Header.TotalSize} ImageTableLength={imageTableBytesRead}");
			return new G1Dat(g1Header, imageTable);
		}

		public int AnnotateStringTable(byte[] fullData, int running_count, ILocoStruct locoStruct, IList<Annotation> annotations)
		{
			Annotation root = new Annotation("String Table", running_count, 1);
			annotations.Add(root);
			var stringAttr = locoStruct.GetType().GetCustomAttribute(typeof(LocoStringCountAttribute), inherit: false) as LocoStringCountAttribute;
			var stringsInTable = stringAttr?.Count ?? 1;
			for (int i = 0; i < stringsInTable; i++)
			{
				int index = Array.IndexOf(fullData[running_count..], (byte)0xFF);
				int endIndexOfStringList = index + running_count;
				int null_index = 0;
				Annotation elementRoot = new Annotation("Element " + i, root, running_count, index);
				annotations.Add(elementRoot);
				do
				{
					annotations.Add(new Annotation(((LanguageId)fullData[running_count]).ToString(), elementRoot, running_count, 1));
					running_count++;
					null_index = Array.IndexOf(fullData[running_count..], (byte)0);
					string string_element = new string(fullData[running_count..(running_count + null_index)].Select(b => (char)b).ToArray());
					annotations.Add(new Annotation("'" + string_element + "'", elementRoot, running_count, string_element.Length + 1));
					running_count += null_index + 1;
				} while (running_count < endIndexOfStringList);
				running_count = endIndexOfStringList + 1;
			}
			root.End = running_count;
			return running_count;
		}

		void annotateProperties(object o, IList<Annotation> annotations, int running_count = 0, Annotation? root = null)
		{
			foreach (var p in o.GetType().GetProperties())
			{
				var offset = p.GetCustomAttribute<LocoStructOffsetAttribute>();
				if (offset != null)
				{
					int location = running_count + (int)offset!.Offset;
					annotations.Add(new Annotation(p.Name, root!, location, 1));
				}
			}
		}
		public IList<Annotation> Annotate(byte[] bytelist, out byte[] fullData)
		{
			List<Annotation> annotations = new List<Annotation>();
			int running_count = 0;
			// S5 Header Annotations
			Annotation s5HeaderAnnotation = new Annotation("S5 Header", 0, S5Header.StructLength);
			annotations.Add(s5HeaderAnnotation);
			annotations.Add(new Annotation("Flags", s5HeaderAnnotation, 0, 4));
			annotations.Add(new Annotation("Name: '" + System.Text.Encoding.ASCII.GetString(bytelist[4..12]) + "'", 
   										   s5HeaderAnnotation, 
										   4, 
										   8));
			annotations.Add(new Annotation("Checksum",
										   s5HeaderAnnotation,
										   12,
										   4));
			var s5Header = S5Header.Read(bytelist[0..S5Header.StructLength]);
			running_count += S5Header.StructLength;

			// Object Header Annotations
			Annotation objectHeaderAnnotation = new Annotation("Object Header", running_count, ObjectHeader.StructLength);
			annotations.Add(new Annotation("Encoding", objectHeaderAnnotation, running_count, 1));
			annotations.Add(new Annotation("Data Length", objectHeaderAnnotation, running_count + 1, 4));
			var objectHeader = ObjectHeader.Read(bytelist[running_count..(running_count + ObjectHeader.StructLength)]);
			running_count += ObjectHeader.StructLength;

			// Decode Loco Struct
			fullData = bytelist[..running_count].Concat(Decode(objectHeader.Encoding, bytelist[running_count..(int)(running_count + objectHeader.DataLength)]))
				.ToArray();
			var locoStruct = GetLocoStruct(s5Header.ObjectType, fullData[running_count..]);
			if (locoStruct == null)
			{
				Debugger.Break();
				throw new NullReferenceException("loco object was null");
			}

			var structSize = AttributeHelper.Get<LocoStructSizeAttribute>(locoStruct.GetType());
			var locoStructSize = structSize!.Size;

			Annotation locoStructAnnotation = new Annotation("Loco Struct", running_count, locoStructSize);
			annotations.Add(locoStructAnnotation);
			annotateProperties(locoStruct, annotations, running_count, locoStructAnnotation);

			running_count += structSize!.Size;

			// String Table
			running_count = AnnotateStringTable(fullData, running_count, locoStruct, annotations);

			ReadOnlySpan<byte> remainingData = fullData[running_count..];
			int currentRemainingData = remainingData.Length;
			if (locoStruct is ILocoStructVariableData locoStructExtra)
			{
				remainingData = locoStructExtra.Load(remainingData);
			}
			annotations.Add(new Annotation("Loco Variables", running_count, currentRemainingData - remainingData.Length));
			running_count += currentRemainingData - remainingData.Length;

			return AnnotateG1Data(fullData, annotations, running_count);
		}

		public IList<Annotation> AnnotateG1Data(byte[] fullData, IList<Annotation> annotations, int running_count = 0)
		{
			Annotation g1Annotation = new Annotation("G1", running_count, fullData.Length - running_count);
			Annotation g1HeaderAnnotation = new Annotation("Header", g1Annotation, running_count, 8);
			if(running_count < fullData.Length)
			{
				annotations.Add(g1Annotation);
				annotations.Add(g1HeaderAnnotation);
				annotations.Add(new Annotation("Number Of Entries", g1HeaderAnnotation, running_count, sizeof(UInt32)));
				annotations.Add(new Annotation("Total Size", g1HeaderAnnotation, running_count + sizeof(UInt32), sizeof(UInt32)));
				var g1Header = new G1Header(
					BitConverter.ToUInt32(fullData[running_count..(running_count + 4)]),
					BitConverter.ToUInt32(fullData[running_count..(running_count + 8)]));
				running_count += 8;
				Annotation g1DataAnnotation = new Annotation("Data", g1Annotation, running_count, 1);
				g1DataAnnotation.End = fullData.Length;
				Annotation gHeadersAnnotation = new Annotation("Headers", g1DataAnnotation, running_count, 1);
				annotations.Add(g1DataAnnotation);
				annotations.Add(gHeadersAnnotation);
				int imageDataStart = running_count;

				int g1Element32Size = 0x10;

				List<G1Element32> g32elements = new List<G1Element32>();

				for (int i = 0; i < g1Header.NumEntries; i++)
				{
					var g32Element = (G1Element32)ByteReader.ReadLocoStruct<G1Element32>(fullData[running_count..]);
					Annotation g32ElementAnnotation = new Annotation("Header " + (i + 1), gHeadersAnnotation, running_count, g1Element32Size);
					annotations.Add(g32ElementAnnotation);
					annotateProperties(g32Element, annotations, running_count, g32ElementAnnotation);
					g32elements.Add(g32Element);
					running_count += g1Element32Size;
				}
				gHeadersAnnotation.End = running_count;

				imageDataStart = running_count;

				Annotation g1ImageDataAnnotation = new Annotation("Images", g1DataAnnotation, running_count, 8);
				annotations.Add(g1ImageDataAnnotation);
				g1ImageDataAnnotation.End = fullData.Length;

				for (int i = 0; i < g32elements.Count; i++)
				{
					int imageStart = imageDataStart + (int)g32elements[i].Offset;
					int imageSize = fullData.Length - imageStart;
					if (i + 1 < g32elements.Count)
					{
						imageSize = (int)g32elements[i + 1].Offset - (int)g32elements[i].Offset;
					}
					annotations.Add(new Annotation("Image " + (i + 1), g1ImageDataAnnotation, imageStart, imageSize));
					running_count = imageDataStart + (int)g32elements[i].Offset;
				}
			}
			return annotations;
		}

		// load file
		public ILocoObject LoadFull(string filename, bool loadExtra = true)
		{
			ReadOnlySpan<byte> fullData = LoadBytesFromFile(filename);

			// make openlocotool useful objects
			var s5Header = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			var decodedData = Decode(objectHeader.Encoding, remainingData);
			remainingData = decodedData;
			var locoStruct = GetLocoStruct(s5Header.ObjectType, remainingData);

			if (locoStruct == null)
			{
				Debugger.Break();
				throw new NullReferenceException("loco object was null");
			}

			var structSize = AttributeHelper.Get<LocoStructSizeAttribute>(locoStruct.GetType());
			var locoStructSize = structSize!.Size;
			remainingData = remainingData[locoStructSize..];

			var headerFlag = BitConverter.GetBytes(s5Header.Flags).AsSpan()[0..1];
			var checksum = ComputeObjectChecksum(headerFlag, fullData[4..12], decodedData);

			if (checksum != s5Header.Checksum)
			{
				//throw new ArgumentException($"{s5Header.Name} had incorrect checksum. expected={s5Header.Checksum} actual={checksum}");
				Logger.Error($"{s5Header.Name} had incorrect checksum. expected={s5Header.Checksum} actual={checksum}");
			}

			// every object has a string table
			var (stringTable, stringTableBytesRead) = LoadStringTable(remainingData, locoStruct);
			remainingData = remainingData[stringTableBytesRead..];

			// special handling per object type
			if (loadExtra && locoStruct is ILocoStructVariableData locoStructExtra)
			{
				remainingData = locoStructExtra.Load(remainingData);
			}

			//try
			{
				var (g1Header, imageTable, imageTableBytesRead) = LoadImageTable(remainingData);
				Logger.Log(LogLevel.Info, $"FileLength={new FileInfo(filename).Length} HeaderLength={S5Header.StructLength} DataLength={objectHeader.DataLength} StringTableLength={stringTableBytesRead} ImageTableLength={imageTableBytesRead}");

				return new LocoObject(s5Header, objectHeader, locoStruct, stringTable, g1Header, imageTable);
			}
			//catch (Exception ex)
			//{
			//	Logger.Error(ex.ToString());
			//	return new LocoObject(objectHeader, locoStruct, stringTable, new G1Header(0, 0), new List<G1Element32>());
			//}
		}

		(StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, ILocoStruct locoStruct)
		{
			var stringAttr = locoStruct.GetType().GetCustomAttribute(typeof(LocoStringCountAttribute), inherit: false) as LocoStringCountAttribute;
			var stringsInTable = stringAttr?.Count ?? 1;
			var strings = new StringTable();

			if (data.Length == 0 || stringsInTable == 0)
			{
				return (strings, 0);
			}

			var ptr = 0;

			for (var i = 0; i < stringsInTable; ++i)
			{
				for (; ptr < data.Length && data[ptr] != 0xFF;)
				{
					var lang = (LanguageId)data[ptr++];
					var ini = ptr;

					while (data[ptr++] != '\0') ;

					var str = Encoding.ASCII.GetString(data[ini..(ptr - 1)]); // do -1 to exclude the \0

					if (strings.ContainsKey((i, lang)))
					{
						Logger.Error($"Key {(i, lang)} already exists (this shouldn't happen)");
						break;
					}
					else
					{
						strings.Add((i, lang), str);
					}
				}

				ptr++; // add one because we skipped the 0xFF byte at the end
			}

			return (strings, ptr);
		}

		static (G1Header header, List<G1Element32> table, int bytesRead) LoadImageTable(ReadOnlySpan<byte> data)
		{
			var g1Element32s = new List<G1Element32>();

			if (data.Length == 0)
			{
				return (new G1Header(0, 0), g1Element32s, 0);
			}

			var g1Header = new G1Header(
				BitConverter.ToUInt32(data[0..4]),
				BitConverter.ToUInt32(data[4..8]));

			var g1ElementHeaders = data[8..];

			const int g1Element32Size = 0x10; // todo: lookup from the LocoStructSize attribute
			var imageData = g1ElementHeaders[((int)g1Header.NumEntries * g1Element32Size)..];
			g1Header.ImageData = imageData.ToArray();
			for (var i = 0; i < g1Header.NumEntries; ++i)
			{
				var g32ElementData = g1ElementHeaders[(i * g1Element32Size)..((i + 1) * g1Element32Size)];
				var g32Element = (G1Element32)ByteReader.ReadLocoStruct<G1Element32>(g32ElementData);
				g1Element32s.Add(g32Element);
			}

			// set image data
			for (var i = 0; i < g1Header.NumEntries; ++i)
			{
				var currElement = g1Element32s[i];
				var nextOffset = i < g1Header.NumEntries - 1
					? g1Element32s[i + 1].Offset
					: g1Header.TotalSize;

				currElement.ImageData = imageData[(int)currElement.Offset..(int)nextOffset].ToArray();

				if (currElement.Flags.HasFlag(G1ElementFlags.IsRLECompressed))
				{
					currElement.ImageData = DecodeRLEImageData(currElement);
				}
			}

			return (g1Header, g1Element32s, g1ElementHeaders.Length + imageData.Length);
		}

		public static byte[] DecodeRLEImageData(G1Element32 img)
		{
			// not sure why this happens, but this seems 'legit'; airport files have these
			if (img.ImageData.Length == 0)
			{
				return img.ImageData;
			}

			var width = img.Width;
			var height = img.Height;

			var dstLineWidth = img.Width;
			var dst0Index = 0; // dstLineWidth * img.yOffset + img.xOffset;

			var srcBuf = img.ImageData;
			var dstBuf = new byte[img.Width * img.Height]; // Assuming a single byte per pixel

			var srcY = 0;

			if (srcY < 0)
			{
				srcY++;
				height--;
				dst0Index += dstLineWidth;
			}

			for (var i = 0; i < height; i++)
			{
				var y = srcY + i;

				var lineOffset = srcBuf[y * 2] | (srcBuf[(y * 2) + 1] << 8);

				var nextRunIndex = lineOffset;
				var dstLineStartIndex = dst0Index + (dstLineWidth * i);

				while (true)
				{
					var srcIndex = nextRunIndex;

					var rleInfoByte = srcBuf[srcIndex++];
					var dataSize = rleInfoByte & 0x7F;
					var isEndOfLine = (rleInfoByte & 0x80) != 0;

					var firstPixelX = srcBuf[srcIndex++];
					nextRunIndex = srcIndex + dataSize;

					var x = firstPixelX - 0; // img.xOffset;
					var numPixels = dataSize;

					if (x > 0)
					{
						x++;
						srcIndex++;
						numPixels--;
					}
					else if (x < 0)
					{
						srcIndex += -x;
						numPixels += x;
						x = 0;
					}

					numPixels = Math.Min(numPixels, width - x);

					var dstIndex = dstLineStartIndex + x;

					if (numPixels > 0)
					{
						Array.Copy(srcBuf, srcIndex, dstBuf, dstIndex, numPixels);
					}

					if (isEndOfLine)
						break;
				}
			}

			return dstBuf;
		}

		public byte[] LoadBytesFromFile(string filename)
		{
			if (!File.Exists(filename))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {filename}");
				throw new InvalidOperationException($"File doesn't exist: {filename}");
			}

			Logger.Log(LogLevel.Info, $"Loading {filename}");
			return File.ReadAllBytes(filename);
		}

		public S5Header LoadHeader(string filename)
		{
			if (!File.Exists(filename))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {filename}");

				throw new InvalidOperationException($"File doesn't exist: {filename}");
			}

			Logger.Log(LogLevel.Info, $"Loading header for {filename}");
			var size = S5Header.StructLength;
			var data = new byte[size];

			using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				var bytesRead = fs.Read(data, 0, size);
				if (bytesRead != size)
				{
					throw new InvalidOperationException($"bytes read ({bytesRead}) didn't match bytes expected ({size})");
				}

				return S5Header.Read(data);
			}
		}

		public static ILocoStruct GetLocoStruct(ObjectType objectType, ReadOnlySpan<byte> data)
			=> objectType switch
			{
				ObjectType.Airport => ByteReader.ReadLocoStruct<AirportObject>(data),
				ObjectType.Bridge => ByteReader.ReadLocoStruct<BridgeObject>(data),
				ObjectType.Building => ByteReader.ReadLocoStruct<BuildingObject>(data),
				ObjectType.Cargo => ByteReader.ReadLocoStruct<CargoObject>(data),
				ObjectType.CliffEdge => ByteReader.ReadLocoStruct<CliffEdgeObject>(data),
				ObjectType.Climate => ByteReader.ReadLocoStruct<ClimateObject>(data),
				ObjectType.Competitor => ByteReader.ReadLocoStruct<CompetitorObject>(data),
				ObjectType.Currency => ByteReader.ReadLocoStruct<CurrencyObject>(data),
				ObjectType.Dock => ByteReader.ReadLocoStruct<DockObject>(data),
				ObjectType.HillShapes => ByteReader.ReadLocoStruct<HillShapesObject>(data),
				ObjectType.Industry => ByteReader.ReadLocoStruct<IndustryObject>(data),
				ObjectType.InterfaceSkin => ByteReader.ReadLocoStruct<InterfaceSkinObject>(data),
				ObjectType.Land => ByteReader.ReadLocoStruct<LandObject>(data),
				ObjectType.LevelCrossing => ByteReader.ReadLocoStruct<LevelCrossingObject>(data),
				ObjectType.Region => ByteReader.ReadLocoStruct<RegionObject>(data),
				ObjectType.RoadExtra => ByteReader.ReadLocoStruct<RoadExtraObject>(data),
				ObjectType.Road => ByteReader.ReadLocoStruct<RoadObject>(data),
				ObjectType.RoadStation => ByteReader.ReadLocoStruct<RoadStationObject>(data),
				ObjectType.Scaffolding => ByteReader.ReadLocoStruct<ScaffoldingObject>(data),
				ObjectType.ScenarioText => ByteReader.ReadLocoStruct<ScenarioTextObject>(data),
				ObjectType.Snow => ByteReader.ReadLocoStruct<SnowObject>(data),
				ObjectType.Sound => ByteReader.ReadLocoStruct<SoundObject>(data),
				ObjectType.Steam => ByteReader.ReadLocoStruct<SteamObject>(data),
				ObjectType.StreetLight => ByteReader.ReadLocoStruct<StreetLightObject>(data),
				ObjectType.TownNames => ByteReader.ReadLocoStruct<TownNamesObject>(data),
				ObjectType.TrackExtra => ByteReader.ReadLocoStruct<TrackExtraObject>(data),
				ObjectType.Track => ByteReader.ReadLocoStruct<TrackObject>(data),
				ObjectType.TrainSignal => ByteReader.ReadLocoStruct<TrainSignalObject>(data),
				ObjectType.TrainStation => ByteReader.ReadLocoStruct<TrainStationObject>(data),
				ObjectType.Tree => ByteReader.ReadLocoStruct<TreeObject>(data),
				ObjectType.Tunnel => ByteReader.ReadLocoStruct<TunnelObject>(data),
				ObjectType.Vehicle => ByteReader.ReadLocoStruct<VehicleObject>(data),
				ObjectType.Wall => ByteReader.ReadLocoStruct<WallObject>(data),
				ObjectType.Water => ByteReader.ReadLocoStruct<WaterObject>(data),
				_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
			};

		// taken from openloco's SawyerStreamReader::readChunk
		public byte[] Decode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		{
			switch (encoding)
			{
				case SawyerEncoding.Uncompressed:
					return data.ToArray();
				case SawyerEncoding.RunLengthSingle:
					return DecodeRunLengthSingle(data);
				case SawyerEncoding.RunLengthMulti:
					return DecodeRunLengthMulti(DecodeRunLengthSingle(data));
				case SawyerEncoding.Rotate:
					return DecodeRotate(data);
				default:
					Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private static byte[] DecodeRunLengthSingle(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			for (var i = 0; i < data.Length; ++i)
			{
				var rleCodeByte = data[i];
				if ((rleCodeByte & 128) > 0)
				{
					i++;
					if (i >= data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					buffer.AddRange(Enumerable.Repeat(data[i], 257 - rleCodeByte));
				}
				else
				{
					if (i + 1 >= data.Length || i + 1 + rleCodeByte + 1 > data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					var copyLen = rleCodeByte + 1;

					for (var j = 0; j < copyLen; ++j)
					{
						buffer.Add(data[i + 1 + j]);
					}

					i += rleCodeByte + 1;
				}
			}

			// convert to span
			var decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthMulti
		private static byte[] DecodeRunLengthMulti(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			for (var i = 0; i < data.Length; i++)
			{
				if (data[i] == 0xFF)
				{
					i++;
					if (i >= data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					buffer.Add(data[i]);
				}
				else
				{
					var offset = (data[i] >> 3) - 32;
					//assert(offset < 0);
					if (-offset > buffer.Count)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					var copySrc = 0 + buffer.Count + offset;
					var copyLen = (data[i] & 7) + 1;

					// Copy it to temp buffer first as we can't copy buffer to itself due to potential
					// realloc in between reserve and push
					//uint8_t copyBuffer[32];
					//assert(copyLen <= sizeof(copyBuffer));
					//std::memcpy(copyBuffer, copySrc, copyLen);
					//buffer.push_back(copyBuffer, copyLen);

					buffer.AddRange(buffer.GetRange(copySrc, copyLen));
				}
			}

			// convert to span
			var decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		private byte[] DecodeRotate(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			byte code = 1;
			for (var i = 0; i < data.Length; i++)
			{
				buffer.Add(Ror(data[i], code));
				code = (byte)((code + 2) & 7);
			}

			// convert to span
			var decodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				decodedSpan[counter++] = b;
			}

			return decodedSpan;
		}

		private static byte Ror(byte x, byte shift)
		{
			const byte byteDigits = 8;
			return (byte)((x >> shift) | (x << (byteDigits - shift)));
		}
	}
}
