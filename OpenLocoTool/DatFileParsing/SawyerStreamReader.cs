using System.Diagnostics;
using System.Text;
using OpenLocoTool.Headers;
using OpenLocoTool.Objects;
using OpenLocoTool.Types;
using OpenLocoToolCommon;

namespace OpenLocoTool.DatFileParsing
{

	public static class SawyerStreamReader
	{
		public static List<S5Header> LoadVariableCountS5Headers(ReadOnlySpan<byte> data, int count)
		{
			List<S5Header> result = [];
			for (var i = 0; i < count; ++i)
			{
				var header = S5Header.Read(data[..S5Header.StructLength]);
				if (header.Checksum != 0 || header.Flags != 255)
				{
					result.Add(header);
				}

				data = data[S5Header.StructLength..];
			}

			return result;
		}

		public static S5Header LoadS5HeaderFromFile(string filename, ILogger? logger = null)
		{
			if (!File.Exists(filename))
			{
				logger?.Error($"Path doesn't exist: {filename}");
			}

			logger?.Info($"Loading header for {filename}");

			using var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None, 32, FileOptions.SequentialScan | FileOptions.Asynchronous);
			using var reader = new BinaryReader(fileStream);
			var data = reader.ReadBytes(S5Header.StructLength);
			return data.Length != S5Header.StructLength
				? throw new InvalidOperationException($"bytes read ({data.Length}) didn't match bytes expected ({S5Header.StructLength})")
				: S5Header.Read(data);
		}

		public static byte[] LoadBytesFromFile(string filename, ILogger? logger = null)
		{
			if (!File.Exists(filename))
			{
				var ex = new InvalidOperationException($"File doesn't exist: {filename}");
				logger?.Error(ex);
			}

			logger?.Info($"Loading {filename}");
			return File.ReadAllBytes(filename);
		}

		public static (S5Header s5Header, ObjectHeader objHeader, byte[] decodedData) LoadAndDecodeFromFile(string filename, ILogger? logger = null)
		{
			ReadOnlySpan<byte> fullData = LoadBytesFromFile(filename);

			var s5Header = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			var decodedData = Decode(objectHeader.Encoding, remainingData.ToArray());
			//remainingData = decodedData;

			var headerFlag = BitConverter.GetBytes(s5Header.Flags).AsSpan()[0..1];
			var checksum = SawyerStreamUtils.ComputeObjectChecksum(headerFlag, fullData[4..12], decodedData);

			if (checksum != s5Header.Checksum)
			{
				logger?.Error($"{s5Header.Name} had incorrect checksum. expected={s5Header.Checksum} actual={checksum}");
			}

			return (s5Header, objectHeader, decodedData);
		}

		// load file
		public static (DatFileInfo DatFileInfo, ILocoObject LocoObject) LoadFullObjectFromFile(string filename, bool loadExtra = true, ILogger? logger = null)
		{
			logger?.Info($"Full-loading \"{filename}\" with loadExtra={loadExtra}");

			var (s5Header, objectHeader, decodedData) = LoadAndDecodeFromFile(filename, logger);
			ReadOnlySpan<byte> remainingData = decodedData;

			var locoStruct = GetLocoStruct(s5Header.ObjectType, remainingData);

			if (locoStruct == null)
			{
				Debugger.Break();
				throw new NullReferenceException($"{filename} was unable to be decoded");
			}

			var structSize = AttributeHelper.Get<LocoStructSizeAttribute>(locoStruct.GetType());
			var locoStructSize = structSize!.Size;
			remainingData = remainingData[locoStructSize..];

			// every object has a string table
			var (stringTable, stringTableBytesRead) = LoadStringTable(remainingData, locoStruct);
			remainingData = remainingData[stringTableBytesRead..];

			// some objects have variable-sized data
			if (loadExtra && locoStruct is ILocoStructVariableData locoStructExtra)
			{
				remainingData = locoStructExtra.Load(remainingData);
			}

			LocoObject? newObj;
			try
			{
				// some objects have graphics data
				var (_, imageTable, imageTableBytesRead) = LoadImageTable(remainingData);
				logger?.Info($"FileLength={new FileInfo(filename).Length} HeaderLength={S5Header.StructLength} DataLength={objectHeader.DataLength} StringTableLength={stringTableBytesRead} ImageTableLength={imageTableBytesRead}");

				newObj = new LocoObject(locoStruct, stringTable, imageTable);
			}
			catch (Exception ex)
			{
				newObj = new LocoObject(locoStruct, stringTable);
				logger?.Error(ex, "Error loading graphics table");
			}

			// add to object manager
			SObjectManager.Add(newObj);

			return new(new DatFileInfo(s5Header, objectHeader), newObj);
		}

		public static (StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, string[] stringNames, ILogger? logger = null)
		{
			var stringTable = new StringTable();

			if (data.Length == 0 || stringNames.Length == 0)
			{
				return (stringTable, 0);
			}

			var ptr = 0;

			foreach (var locoString in stringNames)
			{
				stringTable.Table.Add(locoString, []);
				var languageDict = stringTable[locoString];

				// add empty strings for every single language
				foreach (var language in Enum.GetValues<LanguageId>())
				{
					languageDict.Add(language, string.Empty);
				}

				for (; ptr < data.Length && data[ptr] != 0xFF;)
				{
					var lang = (LanguageId)data[ptr++];
					var ini = ptr;

					while (data[ptr++] != '\0') ;

					var str = Encoding.Latin1.GetString(data[ini..(ptr - 1)]); // do -1 to exclude the \0

					if (!languageDict.ContainsKey(lang))
					{
						logger?.Error($"Skipping unknown language: \"{lang}\"");
						break;
					}
					languageDict[lang] = str;
				}

				ptr++; // add one because we skipped the 0xFF byte at the end
			}

			return (stringTable, ptr);
		}

		public static (StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, ILocoStruct locoStruct)
		{
			var locoStructType = locoStruct.GetType();
			var stringTableStrings = AttributeHelper.Has<LocoStringTableAttribute>(locoStructType)
				? AttributeHelper.Get<LocoStringTableAttribute>(locoStructType)!.Strings
				: AttributeHelper.GetAllPropertiesWithAttribute<LocoStringAttribute>(locoStructType).Select(s => s.Name).ToArray();

			return LoadStringTable(data, stringTableStrings);
		}

		public static G1Dat LoadG1(string filename, ILogger? logger = null)
		{
			ReadOnlySpan<byte> fullData = LoadBytesFromFile(filename);
			var (g1Header, imageTable, imageTableBytesRead) = LoadImageTable(fullData);
			logger?.Info($"FileLength={new FileInfo(filename).Length} NumEntries={g1Header.NumEntries} TotalSize={g1Header.TotalSize} ImageTableLength={imageTableBytesRead}");
			return new G1Dat(g1Header, imageTable);
		}

		public static (G1Header header, List<G1Element32> table, int bytesRead) LoadImageTable(ReadOnlySpan<byte> data)
		{
			var g1Element32s = new List<G1Element32>();

			if (data.Length < ObjectAttributes.StructSize<G1Header>())
			{
				return (new G1Header(0, 0), g1Element32s, 0);
			}

			var g1Header = new G1Header(
				BitConverter.ToUInt32(data[0..4]),
				BitConverter.ToUInt32(data[4..8]));

			var g1ElementHeaders = data[8..];

			var imageData = g1ElementHeaders[((int)g1Header.NumEntries * G1Element32.StructLength)..];
			g1Header.ImageData = imageData.ToArray();
			for (var i = 0; i < g1Header.NumEntries; ++i)
			{
				var g32ElementData = g1ElementHeaders[(i * G1Element32.StructLength)..((i + 1) * G1Element32.StructLength)];
				var g32Element = ByteReader.ReadLocoStruct<G1Element32>(g32ElementData);
				g1Element32s.Add(g32Element);
			}

			// set image data
			for (var i = 0; i < g1Header.NumEntries; ++i)
			{
				var currElement = g1Element32s[i];
				var nextOffset = i < g1Header.NumEntries - 1
					? g1Element32s[i + 1].Offset
					: (uint)g1Header.ImageData.Length;

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
				return [];
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
					{
						break;
					}
				}
			}

			return dstBuf;
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
		public static byte[] Decode(SawyerEncoding encoding, byte[] data) => encoding switch
		{
			SawyerEncoding.Uncompressed => data,
			SawyerEncoding.RunLengthSingle => DecodeRunLengthSingle(data),
			SawyerEncoding.RunLengthMulti => DecodeRunLengthMulti(DecodeRunLengthSingle(data)),
			SawyerEncoding.Rotate => DecodeRotate(data),
			_ => throw new InvalidDataException("Unknown chunk encoding scheme"),
		};

		public static (RiffWavHeader header, byte[] data) LoadMusicTrack(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			using (var br = new BinaryReader(ms))
			{
				var headerBytes = br.ReadBytes(ObjectAttributes.StructSize<RiffWavHeader>());
				var header = ByteReader.ReadLocoStruct<RiffWavHeader>(headerBytes);

				var pcmData = new byte[header.DataLength];
				br.Read(pcmData);
				return (header, pcmData);
			}
		}

		public static List<(WaveFormatEx header, byte[] data)> LoadSoundEffectsFromCSS(byte[] data)
		{
			var result = new List<(WaveFormatEx, byte[])>();

			using (var ms = new MemoryStream(data))
			using (var br = new BinaryReader(ms))
			{
				var numSounds = br.ReadUInt32();
				var soundOffsets = new uint32_t[numSounds];
				for (var i = 0; i < numSounds; ++i)
				{
					soundOffsets[i] = br.ReadUInt32();
				}

				for (var i = 0; i < numSounds; ++i)
				{
					br.BaseStream.Position = soundOffsets[i];
					var pcmLen = br.ReadUInt32();
					var format = ByteReader.ReadLocoStruct<WaveFormatEx>(br.ReadBytes(ObjectAttributes.StructSize<WaveFormatEx>()));

					var pcmData = br.ReadBytes((int)pcmLen);
					result.Add((format, pcmData));
				}
			}

			return result;
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthSingle
		private static byte[] DecodeRunLengthSingle(byte[] data)
		{
			var ms = new MemoryStream();

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

					var count = 257 - rleCodeByte;

					var arr = new byte[count];
					Array.Fill(arr, data[i]);
					ms.Write(arr);
				}
				else
				{
					if (i + 1 >= data.Length || i + 1 + rleCodeByte + 1 > data.Length)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					var copyLen = rleCodeByte + 1;

					ms.Write(data, i + 1, copyLen);
					i += rleCodeByte + 1;
				}
			}

			return ms.ToArray();
		}

		// taken from openloco SawyerStreamReader::decodeRunLengthMulti
		private static byte[] DecodeRunLengthMulti(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = [];

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
					if (-offset > buffer.Count)
					{
						throw new ArgumentException("Invalid RLE run");
					}

					var copySrc = 0 + buffer.Count + offset;
					var copyLen = (data[i] & 7) + 1;

					buffer.AddRange(buffer.GetRange(copySrc, copyLen));
				}
			}

			return [.. buffer];
		}

		private static byte[] DecodeRotate(ReadOnlySpan<byte> data)
		{
			static byte Ror(byte x, byte shift)
			{
				const byte byteDigits = 8;
				return (byte)((x >> shift) | (x << (byteDigits - shift)));
			}

			var buffer = new byte[data.Length];

			byte code = 1;
			for (var i = 0; i < data.Length; i++)
			{
				buffer[i] = Ror(data[i], code);
				code = (byte)((code + 2) & 7);
			}

			return buffer;
		}
	}
}
