using Common.Logging;
using Dat.Data;
using Dat.Objects;
using Dat.Types;
using Dat.Types.SCV5;
using System.Text;

namespace Dat.FileParsing;

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
				// vanilla objects will have sourcegameflag == 0 and checksum == 0. custom objects will have a checksum specified - may need custom handling
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

	public static (S5Header s5Header, ObjectHeader objHeader, byte[] decodedData)? LoadAndDecodeFromFile(string filename, ILogger logger)
		=> LoadAndDecodeFromStream(LoadBytesFromFile(filename), logger);

	public static (S5Header s5Header, ObjectHeader objHeader, byte[] decodedData)? LoadAndDecodeFromStream(ReadOnlySpan<byte> fullData, ILogger logger)
	{
		if (!TryGetHeadersFromBytes(fullData, out var hdrs, logger))
		{
			return null;
		}

		var remainingData = fullData[(S5Header.StructLength + ObjectHeader.StructLength)..];

		byte[] decodedData;
		try
		{
			decodedData = Decode(hdrs.Obj.Encoding, remainingData);
		}
		catch (InvalidDataException ex)
		{
			logger?.Error(ex);
			return (hdrs.S5, hdrs.Obj, []);
		}

		var headerFlag = BitConverter.GetBytes(hdrs.S5.Flags).AsSpan()[0..1];
		var checksum = SawyerStreamUtils.ComputeObjectChecksum(headerFlag, fullData[4..12], decodedData);

		if (checksum != hdrs.S5.Checksum)
		{
			logger?.Error($"{hdrs.S5.Name} had incorrect checksum. expected={hdrs.S5.Checksum} actual={checksum}");
		}

		return (hdrs.S5, hdrs.Obj, decodedData);
	}

	public static bool TryGetHeadersFromBytes(ReadOnlySpan<byte> data, out (S5Header S5, ObjectHeader Obj) hdrs, ILogger logger)
	{
		hdrs = default;
		if (data.Length < (S5Header.StructLength + ObjectHeader.StructLength))
		{
			return false;
		}

		var s5 = S5Header.Read(data[0..S5Header.StructLength]);
		var oh = ObjectHeader.Read(data[S5Header.StructLength..(S5Header.StructLength + ObjectHeader.StructLength)]);

		if (!s5.IsValid())
		{
			logger.Error("S5 header was invalid");
			return false;
		}

		if (!oh.IsValid())
		{
			logger.Error("Object header was invalid");
			return false;
		}

		hdrs.S5 = s5;
		hdrs.Obj = oh;
		return true;
	}

	// load file
	public static (DatFileInfo DatFileInfo, ILocoObject? LocoObject)? LoadFullObjectFromFile(string filename, ILogger logger, bool loadExtra = true)
		=> LoadFullObjectFromStream(File.ReadAllBytes(filename), logger, filename, loadExtra);

	public static (DatFileInfo DatFileInfo, ILocoObject? LocoObject) LoadFullObjectFromStream(ReadOnlySpan<byte> data, ILogger logger, string filename = "<in-memory>", bool loadExtra = true)
	{
		logger.Info($"Full-loading \"{filename}\" with loadExtra={loadExtra}");

		var obj = LoadAndDecodeFromStream(data, logger);
		if (obj == null || obj.Value.decodedData.Length == 0)
		{
			logger.Error($"{filename} was unable to be decoded");
			return (new DatFileInfo(S5Header.NullHeader, ObjectHeader.NullHeader), null);
		}

		var s5Header = obj.Value.s5Header;
		var objectHeader = obj.Value.objHeader;
		var decodedData = obj.Value.decodedData;

		if (decodedData.Length == 0)
		{
			logger.Warning($"No data was decoded from {filename}, file is malformed.");
			return (new DatFileInfo(s5Header, objectHeader), null);
		}

		ReadOnlySpan<byte> remainingData = decodedData;

		var locoStruct = GetLocoStruct(s5Header.ObjectType, remainingData);
		ArgumentNullException.ThrowIfNull(locoStruct, paramName: filename);

		var structSize = AttributeHelper.Get<LocoStructSizeAttribute>(locoStruct.GetType());
		var locoStructSize = structSize!.Size;
		remainingData = remainingData[locoStructSize..];

		// every object has a string table
		var (stringTable, stringTableBytesRead) = LoadStringTable(remainingData, locoStruct, logger);
		remainingData = remainingData[stringTableBytesRead..];

		// some objects have variable-sized data
		if (loadExtra && locoStruct is ILocoStructVariableData locoStructExtra)
		{
			remainingData = locoStructExtra.LoadVariable(remainingData);
		}

		LocoObject? newObj;
		try
		{
			// some objects have graphics data
			var (_, imageTable, imageTableBytesRead) = LoadImageTable(remainingData);
			logger.Info($"HeaderLength={S5Header.StructLength} DataLength={objectHeader.DataLength} StringTableLength={stringTableBytesRead} ImageTableLength={imageTableBytesRead}");
			newObj = new LocoObject(locoStruct, stringTable, imageTable);
			remainingData = remainingData[imageTableBytesRead..];
		}
		catch (Exception ex)
		{
			newObj = new LocoObject(locoStruct, stringTable);
			logger.Error(ex, "Error loading graphics table");
		}

		if (remainingData.Length > 0)
		{
			logger.Debug($"\"{s5Header.Name}\" has {remainingData.Length} bytes unaccounted for. What is this extra data???");
		}

		// some objects have extra computation that must be done after the object is fully loaded
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
				var s5Name = s5Header.Name;
				if (!s5Header.IsVanilla())
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
		while (data[ptr++] != '\0')
		{ }

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

	public static (StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, string[] stringNames, ILogger logger)
	{
		var stringTable = new StringTable();

		if (data.Length == 0 || stringNames.Length == 0)
		{
			logger.Warning("No data for language table");
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

	public static (StringTable table, int bytesRead) LoadStringTable(ReadOnlySpan<byte> data, ILocoStruct locoStruct, ILogger logger)
	{
		var locoStructType = locoStruct.GetType();
		var stringTableStrings = AttributeHelper.Has<LocoStringTableAttribute>(locoStructType)
			? AttributeHelper.Get<LocoStringTableAttribute>(locoStructType)!.Strings
			: [.. AttributeHelper.GetAllPropertiesWithAttribute<LocoStringAttribute>(locoStructType).Select(s => s.Name)];

		return LoadStringTable(data, stringTableStrings, logger);
	}

	public static S5File? LoadSave(string filename, ILogger? logger)
	{
		var data = LoadBytesFromFile(filename, logger).AsSpan();
		if (data.Length < S5File.StructLength)
		{
			return null;
		}

		var s5FileHeader = S5File.Read(data);
		_ = data[S5File.StructLength..];
		return s5FileHeader;
	}

	public static G1Dat? LoadG1(string filename, ILogger logger)
	{
		ReadOnlySpan<byte> fullData = LoadBytesFromFile(filename);
		var (g1Header, imageTable, imageTableBytesRead) = LoadImageTable(fullData);
		logger.Info($"FileLength={new FileInfo(filename).Length} NumEntries={g1Header.NumEntries} TotalSize={g1Header.TotalSize} ImageTableLength={imageTableBytesRead}");
		return new G1Dat(g1Header, imageTable);
	}

	static (G1Header header, List<G1Element32> table, int bytesRead) LoadImageTable(ReadOnlySpan<byte> data)
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

			if (currElement.Flags.HasFlag(G1ElementFlags.DuplicatePrevious))
			{
				if (i == 0)
				{
					throw new ArgumentException("First image cannot have DuplicatePrevious flag since there is no previous image");
				}

				currElement.ImageData = [.. g1Element32s[i - 1].ImageData];
			}
			else
			{
				var nextOffset = GetNextNonDuplicateOffset(g1Element32s, i, (uint)g1Header.ImageData.Length);
				currElement.ImageData = imageData[(int)currElement.Offset..(int)nextOffset].ToArray();
			}

			// if rleCompressed, uncompress it, except if the duplicate-previous flag is also set - by the current code here, the previous
			// image (which was also compressed) is now uncompressed, so we don't need do double-uncompress it.
			if (currElement.Flags.HasFlag(G1ElementFlags.IsRLECompressed) && !currElement.Flags.HasFlag(G1ElementFlags.DuplicatePrevious))
			{
				currElement.ImageData = DecodeRLEImageData(currElement);
			}
		}

		return (g1Header, g1Element32s, G1Header.StructLength + (int)g1Header.TotalSize);
	}

	static uint GetNextNonDuplicateOffset(List<G1Element32> g1Element32s, int i, uint imageDateLength)
	{
		uint nextOffset;
		do
		{
			nextOffset = i == g1Element32s.Count - 1
				? imageDateLength
				: g1Element32s[i + 1].Offset;

			i++;
		}
		while (i < g1Element32s.Count && g1Element32s[i].Flags.HasFlag(G1ElementFlags.DuplicatePrevious));

		return nextOffset;
	}

	public static byte[] DecodeRLEImageData(G1Element32 img)
	{
		var srcBuf = img.ImageData;
		var dstBuf = new byte[img.Width * img.Height]; // Assuming a single byte per pixel - these are palette images

		// For every line/row in the image
		for (var y = 0; y < img.Height; ++y)
		{
			// The first part of the source pointer is a list of offsets to different lines
			// This will move the pointer to the correct source line.
			var nextRunIndex = srcBuf[y * 2] | (srcBuf[(y * 2) + 1] << 8); // takes 2 bytes and makes a uint16_t
			var dstLineStartIndex = img.Width * y;

			// For every segment in the line
			var isEndOfLine = false;
			while (!isEndOfLine)
			{
				var srcIndex = nextRunIndex;
				var dataSize = srcBuf[srcIndex++];
				var firstPixelX = srcBuf[srcIndex++];
				isEndOfLine = (dataSize & 0x80) != 0;
				dataSize &= 0x7F;

				// Have our next source pointer point to the next data section
				nextRunIndex = srcIndex + dataSize;

				var x = firstPixelX;
				int numPixels = dataSize;

				// If the end position is further out than the whole image
				// end position then we need to shorten the line again
				numPixels = Math.Min(numPixels, img.Width - x);

				var dstIndex = dstLineStartIndex + x;

				if (numPixels > 0)
				{
					// Since we're sampling each pixel at this zoom level, just do a straight std::memcpy
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
			ObjectType.TrackSignal => ByteReader.ReadLocoStruct<TrackSignalObject>(data),
			ObjectType.TrackStation => ByteReader.ReadLocoStruct<TrackStationObject>(data),
			ObjectType.Tree => ByteReader.ReadLocoStruct<TreeObject>(data),
			ObjectType.Tunnel => ByteReader.ReadLocoStruct<TunnelObject>(data),
			ObjectType.Vehicle => ByteReader.ReadLocoStruct<VehicleObject>(data),
			ObjectType.Wall => ByteReader.ReadLocoStruct<WallObject>(data),
			ObjectType.Water => ByteReader.ReadLocoStruct<WaterObject>(data),
			_ => throw new ArgumentOutOfRangeException(nameof(objectType), $"unknown object type {objectType}")
		};

	// taken from openloco's SawyerStreamReader::readChunk
	public static byte[] Decode(SawyerEncoding encoding, ReadOnlySpan<byte> data, int minDecodedBytes = int.MaxValue)
		=> encoding switch
		{
			SawyerEncoding.Uncompressed => data.ToArray(),
			SawyerEncoding.RunLengthSingle => DecodeRunLengthSingle(data, minDecodedBytes),
			SawyerEncoding.RunLengthMulti => DecodeRunLengthMulti(DecodeRunLengthSingle(data)),
			SawyerEncoding.Rotate => DecodeRotate(data),
			_ => throw new InvalidDataException("Unknown chunk encoding scheme"),
		};

	//public static (RiffWavHeader header, byte[] data) LoadWavFile(string filename)
	//	=> LoadWavFile(File.ReadAllBytes(filename));

	//public static (RiffWavHeader header, byte[] data) LoadWavFile(byte[] data)
	//{
	//	using (var ms = new MemoryStream(data))
	//	{
	//		return LoadWavFile(ms);
	//	}
	//}

	//public static (RiffWavHeader header, byte[] data) LoadWavFile(Stream ms)
	//{
	//	using (var br = new BinaryReader(ms))
	//	{
	//		var headerBytes = br.ReadBytes(ObjectAttributes.StructSize<RiffWavHeader>());
	//		var header = ByteReader.ReadLocoStruct<RiffWavHeader>(headerBytes);

	//		var pcmData = new byte[header.DataLength];
	//		_ = br.Read(pcmData);
	//		return (header, pcmData);
	//	}
	//}

	public static T ReadChunk<T>(ref ReadOnlySpan<byte> data) where T : class
		=> ByteReader.ReadLocoStruct<T>(ReadChunkCore(ref data));

	public static ReadOnlySpan<byte> ReadChunkCore(ref ReadOnlySpan<byte> data)
	{
		// read encoding and length
		var chunk = ObjectHeader.Read(data[..ObjectHeader.StructLength]);
		data = data[ObjectHeader.StructLength..];

		var chunkBytes = data[..(int)chunk.DataLength];
		// decode bytes
		data = data[(int)chunk.DataLength..];
		return Decode(chunk.Encoding, chunkBytes);
	}

	public static List<(LocoWaveFormat header, byte[] data)> LoadSoundEffectsFromCSS(string filename)
		=> LoadSoundEffectsFromCSS(File.ReadAllBytes(filename));

	public static List<(LocoWaveFormat header, byte[] data)> LoadSoundEffectsFromCSS(byte[] data)
	{
		var result = new List<(LocoWaveFormat, byte[])>();

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
				var header = ByteReader.ReadLocoStruct<LocoWaveFormat>(br.ReadBytes(ObjectAttributes.StructSize<LocoWaveFormat>()));

				var pcmData = br.ReadBytes((int)pcmLen);
				result.Add((header, pcmData));
			}
		}

		return result;
	}

	// taken from openloco SawyerStreamReader::decodeRunLengthSingle
	static byte[] DecodeRunLengthSingle(ReadOnlySpan<byte> data, int minDecodedBytes = int.MaxValue)
	{
		using var ms = new MemoryStream();

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

				ms.Write(data[(i + 1)..(i + 1 + copyLen)]);
				i += rleCodeByte + 1;
			}

			// this is an early terminate - only used for indexing since we only need to parse
			// up to a certain byte position instead of the full object
			if (ms.Position >= minDecodedBytes)
			{
				return ms.ToArray();
			}
		}

		return ms.ToArray();
	}

	// taken from openloco SawyerStreamReader::decodeRunLengthMulti
	static byte[] DecodeRunLengthMulti(ReadOnlySpan<byte> data)
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

	static byte[] DecodeRotate(ReadOnlySpan<byte> data)
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
