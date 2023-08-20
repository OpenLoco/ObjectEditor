using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OpenLocoTool.DatFileParsing
{
	public enum LocoLanguageId : uint8_t
	{
		english_uk,
		english_us,
		french,
		german,
		spanish,
		italian,
		dutch,
		swedish,
		japanese,
		korean,
		chinese_simplified,
		chinese_traditional,
		id_12,
		portuguese,
		blank = 254,
		end = 255
	};

	public class SawyerStreamReader
	{
		private readonly ILogger Logger;

		public SawyerStreamReader(ILogger logger)
			=> Logger = logger;

		static uint ComputeObjectChecksum(ReadOnlySpan<byte> flagByte, ReadOnlySpan<byte> name, ReadOnlySpan<byte> data)
		{
			uint32_t computeChecksum(ReadOnlySpan<byte> data, uint32_t seed)
			{
				var checksum = seed;
				foreach (var d in data)
				{
					checksum = BitOperations.RotateLeft(checksum ^ d, 11);
				}

				return checksum;
			}

			const uint32_t objectChecksumMagic = 0xF369A75B;
			var checksum = computeChecksum(flagByte, objectChecksumMagic);

			checksum = computeChecksum(name, checksum);
			checksum = computeChecksum(data, checksum);
			return checksum;
		}

		// load file
		public ILocoObject LoadFull(string filename)
		{
			Span<byte> fullData = LoadBytesFromFile(filename);

			// make openlocotool useful objects
			var objectHeader = ObjectHeader.Read(fullData);
			var data = fullData[ObjectHeader.StructLength..].ToArray();

			var decodedData = Decode(objectHeader.Encoding, data);
			var locoStruct = GetLocoStruct(objectHeader.ObjectType, decodedData);

			var headerFlag = BitConverter.GetBytes(objectHeader.Flags).AsSpan()[0..1];
			var checksum = ComputeObjectChecksum(headerFlag, fullData[4..12], decodedData);

			if (checksum != objectHeader.Checksum)
			{
				throw new ArgumentException($"{objectHeader.Name} had incorrect checksum. expected={objectHeader.Checksum} actual={checksum}");
			}

			var structAttr = (LocoStructSizeAttribute)locoStruct.GetType().GetCustomAttribute(typeof(LocoStructSizeAttribute), inherit: false);
			var locoStructSize = structAttr.Size;

			// string table
			var remainingData = decodedData[locoStructSize..];
			var stringAttr = (LocoStringCountAttribute)locoStruct.GetType().GetCustomAttribute(typeof(LocoStringCountAttribute), inherit: false);
			var locoStrings = stringAttr?.Count ?? 1;
			var (stringTable, stringTableBytesRead) = LoadStringTable(remainingData, locoStrings);
			remainingData = remainingData[stringTableBytesRead..];

			// special handling per object type
			{
				if (locoStruct is BridgeObject bo)
				{
					var bytesToRead = (bo.TrackNumCompatible + bo.RoadNumCompatible) * ObjectHeader.StructLength;
					remainingData = remainingData[bytesToRead..];
				}

				if (locoStruct is VehicleObject vo)
				{
					var dependentObjects = new List<ObjectHeader>();

					const byte trackType = 0xFF;

					// dependent objects
					if (!vo.Flags.HasFlag(VehicleObjectFlags.unk_09) && (vo.Mode == TransportMode.Rail || vo.Mode == TransportMode.Road))
					{
						var trackHeader = ObjectHeader.Read(remainingData);
						remainingData = remainingData[ObjectHeader.StructLength..];
						dependentObjects.Add(trackHeader);

						// load the object handle for the track header, and set tracktype to its id
					}

					vo = vo with { TrackType = trackType };

					// track mods
					remainingData = remainingData[(ObjectHeader.StructLength * vo.NumMods)..];

					var numSimultaneousCargoTypes = vo.NumSimultaneousCargoTypes;
					// cargo types
					for (var i = 0; i < vo.CargoTypes.Length; ++i)
					{
						var index = numSimultaneousCargoTypes;
						vo.MaxCargo[i] = remainingData[0];
						remainingData = remainingData[1..]; // uint8_t
						if (vo.MaxCargo[index] == 0)
						{
							continue;
						}

						var ptr = BitConverter.ToUInt16(remainingData[0..2]);
						while (ptr != 0xFFFFU)
						{
							var cargoMatchFlags = BitConverter.ToUInt16(remainingData[0..2]);
							remainingData = remainingData[2..]; // uint16_t
							var unk = BitConverter.ToUInt16(remainingData[0..2]);
							remainingData = remainingData[1..]; // uint8_t

							for (var cargoType = 0; cargoType < 32; ++cargoType) // 32 is ObjectType::MaxObjects[cargo]
							{
								//var cargoObject = new CargoObject();
								vo.CargoTypes[index] = 0;
							}

							ptr = BitConverter.ToUInt16(remainingData[0..2]);
						}

						remainingData = remainingData[2..]; // uint16_t

						if (vo.CargoTypes[index] == 0)
						{
							vo.MaxCargo[index] = 0;
						}
						else
						{
							numSimultaneousCargoTypes++;
						}
					}

					vo = vo with { NumSimultaneousCargoTypes = numSimultaneousCargoTypes };

					// animation
					foreach (var anim in vo.Animation)
					{
						if (anim.Type == SimpleAnimationType.None)
						{
							continue;
						}

						remainingData = remainingData[ObjectHeader.SubHeaderLength..];
					}

					// numCompat
					remainingData = remainingData[(ObjectHeader.SubHeaderLength * vo.NumCompat)..];

					// rack rail
					if (vo.Flags.HasFlag(VehicleObjectFlags.RackRail))
					{
						remainingData = remainingData[ObjectHeader.SubHeaderLength..];
					}

					// driving sound
					if (vo.DrivingSoundType != DrivingSoundType.None)
					{
						remainingData = remainingData[ObjectHeader.SubHeaderLength..];
					}

					// driving sound
					remainingData = remainingData[(ObjectHeader.SubHeaderLength * vo.NumStartSounds)..];
					//for (byte i = 0; i < Math.Clamp(vo.NumStartSounds, (byte)0, (byte)127); ++i)
					//{

					//}
					//if (vo.DrivingSoundType != DrivingSoundType.None)
					//{
					//}

					// should be at image table now
				}
			}

			// g1/gfx table
			{
				var (g1Header, imageTable, imageTableBytesRead) = LoadImageTable(remainingData);
				Logger.Log(LogLevel.Info, $"FileLength={new FileInfo(filename).Length} HeaderLength={ObjectHeader.StructLength} DataLength={objectHeader.DataLength} StringTableLength={stringTableBytesRead} ImageTableLength={imageTableBytesRead}");

				return new LocoObject(objectHeader, locoStruct, stringTable, g1Header, imageTable);
			}
		}

		static (StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, int stringsInTable)
		{
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
					var lang = (LocoLanguageId)data[ptr++];
					var ini = ptr;
					while (data[ptr++] != '\0') ;
					var str = Encoding.ASCII.GetString(data[ini..(ptr - 1)]); // do -1 to exclude the \0
					strings.Add((i, lang), str);
				}

				ptr++;
			}

			return (strings, ptr); // add one because we 'read' the 0xFF byte at the end (ie we skipped it)
		}

		// === not working ===
		// airport
		// building
		// dock
		// industry
		// land
		// === working ===
		// bridge (requires RLE)
		// cargo
		// cliffEdge
		// climate (no images)
		// competitor
		// currency
		// hillshapes
		// interfaceskin
		// levelCrossing (requires RLE)
		// roadExtra (requires RLE)
		// scaffolding (requires RLE)
		// snow
		// streetlight
		// tree
		// tunnel (requires RLE)
		// wall
		// water (though it seems bugged)
		(G1Header header, List<G1Element32> table, int bytesRead) LoadImageTable(ReadOnlySpan<byte> data)
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
					? g1Element32s[i + 1].offset
					: g1Header.TotalSize;

				currElement.ImageData = imageData[(int)currElement.offset..(int)nextOffset].ToArray();

				if (currElement.flags.HasFlag(G1ElementFlags.IsRLECompressed))
				{
					currElement.ImageData = DecodeRLEImageData(currElement);
				}
			}

			return (g1Header, g1Element32s, g1ElementHeaders.Length + imageData.Length);
		}

		public static byte[] DecodeRLEImageData(G1Element32 img)
		{
			var width = img.width;
			var height = img.height;

			var dstLineWidth = img.width;
			var dst0Index = 0; // dstLineWidth * img.yOffset + img.xOffset;

			var srcBuf = img.ImageData;
			var dstBuf = new byte[img.width * img.height]; // Assuming a single byte per pixel

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
				return default;
			}

			Logger.Log(LogLevel.Info, $"Loading {filename}");
			return File.ReadAllBytes(filename);
		}

		public ObjectHeader LoadHeader(string filename)
		{
			if (!File.Exists(filename))
			{
				Logger.Log(LogLevel.Error, $"Path doesn't exist: {filename}");
				return default;
			}

			Logger.Log(LogLevel.Info, $"Loading header for {filename}");
			var size = ObjectHeader.StructLength;
			var data = new byte[size];

			using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				var bytesRead = fs.Read(data, 0, size);
				if (bytesRead != size)
				{
					throw new InvalidOperationException($"bytes read ({bytesRead}) didn't match bytes expected ({size})");
				}
			}

			return ObjectHeader.Read(data);
		}

		public static ILocoStruct GetLocoStruct(ObjectType objectType, ReadOnlySpan<byte> data)
			=> objectType switch
			{
				ObjectType.airport => ByteReader.ReadLocoStruct<AirportObject>(data),
				ObjectType.bridge => ByteReader.ReadLocoStruct<BridgeObject>(data),
				ObjectType.building => ByteReader.ReadLocoStruct<BuildingObject>(data),
				ObjectType.cargo => ByteReader.ReadLocoStruct<CargoObject>(data),
				ObjectType.cliffEdge => ByteReader.ReadLocoStruct<CliffEdgeObject>(data),
				ObjectType.climate => ByteReader.ReadLocoStruct<ClimateObject>(data),
				ObjectType.competitor => ByteReader.ReadLocoStruct<CompetitorObject>(data),
				ObjectType.currency => ByteReader.ReadLocoStruct<CurrencyObject>(data),
				ObjectType.dock => ByteReader.ReadLocoStruct<DockObject>(data),
				ObjectType.hillShapes => ByteReader.ReadLocoStruct<HillShapesObject>(data),
				ObjectType.industry => ByteReader.ReadLocoStruct<IndustryObject>(data),
				ObjectType.interfaceSkin => ByteReader.ReadLocoStruct<InterfaceSkinObject>(data),
				ObjectType.land => ByteReader.ReadLocoStruct<LandObject>(data),
				ObjectType.levelCrossing => ByteReader.ReadLocoStruct<LevelCrossingObject>(data),
				ObjectType.region => ByteReader.ReadLocoStruct<RegionObject>(data),
				ObjectType.roadExtra => ByteReader.ReadLocoStruct<RoadExtraObject>(data),
				ObjectType.road => ByteReader.ReadLocoStruct<RoadObject>(data),
				ObjectType.roadStation => ByteReader.ReadLocoStruct<RoadStationObject>(data),
				ObjectType.scaffolding => ByteReader.ReadLocoStruct<ScaffoldingObject>(data),
				ObjectType.scenarioText => ByteReader.ReadLocoStruct<ScenarioTextObject>(data),
				ObjectType.snow => ByteReader.ReadLocoStruct<SnowObject>(data),
				ObjectType.sound => ByteReader.ReadLocoStruct<SoundObject>(data),
				ObjectType.steam => ByteReader.ReadLocoStruct<SteamObject>(data),
				ObjectType.streetLight => ByteReader.ReadLocoStruct<StreetLightObject>(data),
				ObjectType.townNames => ByteReader.ReadLocoStruct<TownNamesObject>(data),
				ObjectType.trackExtra => ByteReader.ReadLocoStruct<TrackExtraObject>(data),
				ObjectType.track => ByteReader.ReadLocoStruct<TrackObject>(data),
				ObjectType.trainSignal => ByteReader.ReadLocoStruct<TrainSignalObject>(data),
				ObjectType.trainStation => ByteReader.ReadLocoStruct<TrainStationObject>(data),
				ObjectType.tree => ByteReader.ReadLocoStruct<TreeObject>(data),
				ObjectType.tunnel => ByteReader.ReadLocoStruct<TunnelObject>(data),
				ObjectType.vehicle => ByteReader.ReadLocoStruct<VehicleObject>(data),
				ObjectType.wall => ByteReader.ReadLocoStruct<WallObject>(data),
				ObjectType.water => ByteReader.ReadLocoStruct<WaterObject>(data),
				_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
			};

		// taken from openloco's SawyerStreamReader::readChunk
		public byte[] Decode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		{
			switch (encoding)
			{
				case SawyerEncoding.uncompressed:
					return data.ToArray();
				case SawyerEncoding.runLengthSingle:
					return decodeRunLengthSingle(data);
				case SawyerEncoding.runLengthMulti:
					return decodeRunLengthMulti(decodeRunLengthSingle(data));
				case SawyerEncoding.rotate:
					return decodeRotate(data);
				default:
					Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private static byte[] decodeRunLengthSingle(ReadOnlySpan<byte> data)
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

					var copyLen = 257 - rleCodeByte;
					var copyByte = data[i];
					buffer.AddRange(Enumerable.Repeat(copyByte, copyLen));
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
		private static byte[] decodeRunLengthMulti(ReadOnlySpan<byte> data)
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

		private byte[] decodeRotate(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();

			byte code = 1;
			for (var i = 0; i < data.Length; i++)
			{
				buffer.Add(ror(data[i], code));
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

		private static byte ror(byte x, byte shift)
		{
			const byte byteDigits = 8;
			return (byte)((x >> shift) | (x << (byteDigits - shift)));
		}
	}
}
