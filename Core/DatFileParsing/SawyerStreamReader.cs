using System.Text;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Objects;
using OpenLoco.ObjectEditor.Types;
using OpenLoco.ObjectEditor.Logging;
using OpenLoco.ObjectEditor.Data;
using Core.Objects;
using Core.Objects.Sound;
using Zenith.Core;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	public static class SawyerStreamReader
	{
		public static List<S5Header> LoadVariableCountS5Headers(ReadOnlySpan<byte> data, int max)
		{
			List<S5Header> result = [];
			for (var i = 0; i < max; ++i)
			{
				if (data[0] != 0xFF)
				{
					var header = S5Header.Read(data[..S5Header.StructLength]);
					if (header.Checksum != 0 || header.Flags != 255)
					{
						result.Add(header);
					}
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
			=> LoadAndDecodeFromStream(LoadBytesFromFile(filename), logger);

		public static (S5Header s5Header, ObjectHeader objHeader, byte[] decodedData) LoadAndDecodeFromStream(ReadOnlySpan<byte> fullData, ILogger? logger = null)
		{
			var s5Header = S5Header.Read(fullData[0..S5Header.StructLength]);
			var remainingData = fullData[S5Header.StructLength..];

			var objectHeader = ObjectHeader.Read(remainingData[0..ObjectHeader.StructLength]);
			remainingData = remainingData[ObjectHeader.StructLength..];

			byte[] decodedData;
			try
			{
				decodedData = Decode(objectHeader.Encoding, remainingData.ToArray());
			}
			catch (InvalidDataException ex)
			{
				logger?.Error(ex);
				return (s5Header, objectHeader, []);
			}
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
		public static (DatFileInfo DatFileInfo, ILocoObject? LocoObject) LoadFullObjectFromFile(string filename, bool loadExtra = true, ILogger? logger = null)
			=> LoadFullObjectFromStream(File.ReadAllBytes(filename), filename, loadExtra, logger);

		public static (DatFileInfo DatFileInfo, ILocoObject? LocoObject) LoadFullObjectFromStream(ReadOnlySpan<byte> data, string filename = "<in-memory>", bool loadExtra = true, ILogger? logger = null)
		{
			logger?.Info($"Full-loading \"{filename}\" with loadExtra={loadExtra}");

			var (s5Header, objectHeader, decodedData) = LoadAndDecodeFromStream(data, logger);

			if (decodedData.Length == 0)
			{
				logger?.Warning($"No data was decoded from {filename}, file is malformed.");
				return new(new DatFileInfo(s5Header, objectHeader), null);
			}

			ReadOnlySpan<byte> remainingData = decodedData;

			var locoStruct = GetLocoStruct(s5Header.ObjectType, remainingData);
			Verify.NotNull(locoStruct, paramName: filename);

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
				logger?.Info($"HeaderLength={S5Header.StructLength} DataLength={objectHeader.DataLength} StringTableLength={stringTableBytesRead} ImageTableLength={imageTableBytesRead}");

				newObj = new LocoObject(locoStruct, stringTable, imageTable);
			}
			catch (Exception ex)
			{
				newObj = new LocoObject(locoStruct, stringTable);
				logger?.Error(ex, "Error loading graphics table");
			}

			// some objects have variable-sized data
			if (loadExtra && locoStruct is ILocoStructPostLoad locoStructPostLoad)
			{
				locoStructPostLoad.PostLoad();
			}

			ValidateLocoStruct(s5Header, locoStruct, logger);

			return new(new DatFileInfo(s5Header, objectHeader), newObj);
		}

		static void ValidateLocoStruct(S5Header s5Header, ILocoStruct locoStruct, ILogger? logger)
		{
			var warnings = new List<string>();

			try
			{
				if (s5Header.SourceGame == SourceGame.Vanilla)
				{
					var s5Name = s5Header.Name.Trim();
					if (!OriginalObjectFiles.Names.TryGetValue(s5Name, out var value) || s5Header.Checksum != value)
					{
						warnings.Add($"\"{s5Header.Name}\" is not a vanilla object but is marked as such.");
					}
				}

				if (!locoStruct.Validate())
				{
					warnings.Add($"\"{s5Header.Name}\" failed validation");
				}

				if (warnings.Count != 0)
				{
					foreach (var warning in warnings)
					{
						logger?.Warning(warning);
					}
				}
				else
				{
					logger?.Info($"\"{s5Header.Name}\" validated successfully");
				}
			}
			catch (NotImplementedException)
			{
				logger?.Debug2($"{s5Header.ObjectType} object type is missing validation function");
			}
		}

		static string CStringToString(ReadOnlySpan<byte> data, Encoding enc)
		{
			var ptr = 0;
			while (data[ptr++] != '\0') ;
			return enc.GetString(data[0..(ptr - 1)]); // do -1 to exclude the \0
		}

		static Dictionary<LanguageId, string> GetNewLanguageDictionary()
		{
			var languageDict = new Dictionary<LanguageId, string>();
			foreach (var language in Enum.GetValues<LanguageId>())
			{
				languageDict.Add(language, string.Empty);
			}
			return languageDict;
		}

		public static (StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, string[] stringNames, ILogger? logger = null)
		{
			var stringTable = new StringTable();

			if (data.Length == 0 || stringNames.Length == 0)
			{
				logger?.Warning("No data for language table");
				return (stringTable, 0);
			}

			var ptr = 0;

			foreach (var locoString in stringNames)
			{
				// init language table
				stringTable.Table.Add(locoString, GetNewLanguageDictionary());
				var languageDict = stringTable[locoString];

				// read string
				for (; ptr < data.Length && data[ptr] != 0xFF; ++ptr)
				{
					var lang = (LanguageId)data[ptr++];
					languageDict[lang] = CStringToString(data[ptr..], Encoding.Latin1);
					ptr += languageDict[lang].Length;
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

		public static G1Dat? LoadG1(string filename, ILogger? logger = null)
		{
			if (!File.Exists(filename))
			{
				logger?.Debug($"File {filename} does not exist");
				return null;
			}

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

			return (g1Header, g1Element32s, G1Header.StructLength + g1ElementHeaders.Length + imageData.Length);
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

		public static (RiffWavHeader header, byte[] data) LoadWavFile(byte[] data)
		{
			using (var ms = new MemoryStream(data))
			using (var br = new BinaryReader(ms))
			{
				var headerBytes = br.ReadBytes(ObjectAttributes.StructSize<RiffWavHeader>());
				var header = ByteReader.ReadLocoStruct<RiffWavHeader>(headerBytes);

				var pcmData = new byte[header.DataLength];
				_ = br.Read(pcmData);
				return (header, pcmData);
			}
		}

		public static T ReadChunk<T>(ref ReadOnlySpan<byte> data) where T : class
			=> ByteReader.ReadLocoStruct<T>(ReadChunkCore(ref data));

		public static byte[] ReadChunkCore(ref ReadOnlySpan<byte> data)
		{
			// read encoding and length
			var chunk = ObjectHeader.Read(data[..ObjectHeader.StructLength]);
			data = data[ObjectHeader.StructLength..];

			// decode bytes
			var chunkBytes = data[..(int)chunk.DataLength];
			data = data[(int)chunk.DataLength..];
			return Decode(chunk.Encoding, chunkBytes.ToArray());
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
					var header = ByteReader.ReadLocoStruct<WaveFormatEx>(br.ReadBytes(ObjectAttributes.StructSize<WaveFormatEx>()));

					var pcmData = br.ReadBytes((int)pcmLen);
					result.Add((header, pcmData));
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
