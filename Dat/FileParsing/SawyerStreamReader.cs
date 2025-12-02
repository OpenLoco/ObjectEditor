using Common.Logging;
using Dat.Converters;
using Dat.Data;
using Dat.Loaders;
using Dat.Types;
using Dat.Types.SCV5;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Sound;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dat.FileParsing;

public static class SawyerStreamReader
{
	public static (S5Header s5Header, ObjectHeader objHeader, byte[] decodedData)? LoadAndDecode(Stream stream, ILogger logger)
	{
		if (!TryGetHeadersFromBytes(stream, out var hdrs, logger))
		{
			return null;
		}

		using var br = new LocoBinaryReader(stream);
		var remainingData = br.ReadToEnd();

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
		var computedChecksum = SawyerStreamUtils.ComputeObjectChecksum(headerFlag, Encoding.ASCII.GetBytes(hdrs.S5.Name.PadRight(8, ' ')), decodedData);

		if (computedChecksum != hdrs.S5.Checksum)
		{
			logger?.Error($"{hdrs.S5.Name} had incorrect checksum. Header={hdrs.S5.Checksum} Computed={computedChecksum}");
		}

		if (hdrs.S5.IsVanilla())
		{
			if (OriginalObjectFiles.Names.TryGetValue(hdrs.S5.Name.Trim(), out var originalChecksum))
			{
				logger?.Debug($"{hdrs.S5.Name} is a vanilla object with checksums [Steam={originalChecksum.SteamChecksum} GoG={originalChecksum.GoGChecksum}]");
			}
			else
			{
				logger?.Warning($"{hdrs.S5.Name} is marked as vanilla but is not in the original object list!");
			}
		}

		return (hdrs.S5, hdrs.Obj, decodedData);
	}

	public static bool TryGetHeadersFromBytes(byte[] data, out (S5Header S5, ObjectHeader Obj) hdrs, ILogger logger)
	{
		using var ms = new MemoryStream(data);
		return TryGetHeadersFromBytes(ms, hdrs: out hdrs, logger);
	}

	public static bool TryGetHeadersFromBytes(Stream stream, out (S5Header S5, ObjectHeader Obj) hdrs, ILogger logger)
	{
		hdrs = default;
		if (stream.Length < (S5Header.StructLength + ObjectHeader.StructLength))
		{
			return false;
		}

		using var br = new LocoBinaryReader(stream);

		var s5 = S5Header.Read(br.ReadBytes(S5Header.StructLength));
		var oh = ObjectHeader.Read(br.ReadBytes(ObjectHeader.StructLength));

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

	public static (DatInfo DatFileInfo, LocoObject? LocoObject) LoadFullObject(string filename, ILogger logger, bool loadExtra = true)
	{
		using var fs = new FileStream(filename, FileMode.Open);
		return LoadFullObject(fs, logger, filename, loadExtra);
	}

	public static (DatInfo DatFileInfo, LocoObject? LocoObject) LoadFullObject(byte[] data, ILogger logger, string filename = "<in-memory>", bool loadExtra = true)
	{
		using var ms = new MemoryStream(data);
		return LoadFullObject(ms, logger, filename: filename, loadExtra: loadExtra);
	}

	public static (DatInfo DatFileInfo, LocoObject? LocoObject) LoadFullObject(Stream stream, ILogger logger, string filename, bool loadExtra = true)
	{
		logger.Info($"Full-loading \"{filename}\" with loadExtra={loadExtra}");

		var obj = LoadAndDecode(stream, logger);
		if (obj == null || obj.Value.decodedData.Length == 0)
		{
			logger.Error($"{filename} was unable to be decoded");
			return (new DatInfo(S5Header.NullHeader, ObjectHeader.NullHeader), null);
		}

		var s5Header = obj.Value.s5Header;
		var objectHeader = obj.Value.objHeader;
		var decodedData = obj.Value.decodedData;

		if (decodedData.Length == 0)
		{
			logger.Warning($"No data was decoded from {filename}, file is malformed.");
			return (new DatInfo(s5Header, objectHeader), null);
		}

		using (var decodedStream = new MemoryStream(decodedData))
		{
			var locoObject = ReadLocoObject(s5Header.ObjectType, decodedStream);
			ValidateLocoStruct(s5Header, locoObject.Object, logger);
			return new(new DatInfo(s5Header, objectHeader), locoObject);
		}
	}

	static void ValidateLocoStruct(S5Header s5Header, ILocoStruct locoStruct, ILogger? logger)
	{
		var warnings = new List<string>();

		try
		{
			if (s5Header.ObjectSource == DatObjectSource.Vanilla)
			{
				var s5Name = s5Header.Name;
				if (!s5Header.IsVanilla())
				{
					warnings.Add($"\"{s5Header.Name}\" is not a vanilla object but is marked as such.");
				}
			}

			foreach (var failedValidation in locoStruct.Validate(new ValidationContext(locoStruct)))
			{
				warnings.Add($"\"{s5Header.Name}\" failed validation: {failedValidation}");
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

	static string CStringToString(LocoBinaryReader br, Encoding enc)
	{
		List<byte> data = [];
		byte b;
		while ((b = br.ReadByte()) != '\0')
		{
			data.Add(b);
		}

		return enc.GetString([.. data]); // do -1 to exclude the \0
	}

	public static StringTable ReadStringTableStream(Stream stream, string[] stringNames, ILogger? logger)
	{
		var stringTable = new StringTable();

		if (stream.Length == 0 || stringNames.Length == 0)
		{
			logger?.Warning("No data for language table");
			return stringTable;
		}

		using (var br = new LocoBinaryReader(stream))
		{
			foreach (var locoString in stringNames)
			{
				// init language table
				var languageDict = stringTable.AddNewString(locoString);

				// read string
				while (br.PeekByte() != LocoConstants.Terminator)
				{
					var lang = ((DatLanguageId)br.ReadByte()).Convert();
					languageDict[lang] = CStringToString(br, Encoding.Latin1);
				}

				br.SkipTerminator();
			}
		}

		return stringTable;
	}

	public static S5File? LoadSave(string filename, ILogger? logger)
	{
		var data = File.ReadAllBytes(filename);
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
		using (var ms = new FileStream(filename, FileMode.Open))
		using (var br = new LocoBinaryReader(ms))
		{
			var (g1Header, imageTable) = ReadImageTable(br);
			logger.Info($"FileLength={new FileInfo(filename).Length} NumEntries={g1Header.NumEntries} TotalSize={g1Header.TotalSize} ImageTableLength={ms.Position}");
			return new G1Dat(g1Header, imageTable);
		}
	}

	public static (G1Header Header, List<GraphicsElement> Table) ReadImageTable(LocoBinaryReader br)
	{
		if (br.BaseStream.Length - br.BaseStream.Position < ObjectAttributes.StructSize<G1Header>())
		{
			return (new G1Header(0, 0), []);
		}

		// read G1Header
		var numEntries = br.ReadUInt32();
		var totalSize = br.ReadUInt32();
		var g1Header = new G1Header(numEntries, totalSize);

		// read G1Element headers
		var g1Element32s = new List<DatG1Element32>();
		for (var i = 0; i < g1Header.NumEntries; ++i)
		{
			var g32Element = ByteReader.ReadLocoStruct<DatG1Element32>(br.ReadBytes(DatG1Element32.StructLength));
			g1Element32s.Add(g32Element);
		}

		var imageData = br.ReadToEnd();
		g1Header.ImageData = [.. imageData];

		var graphicsElements = new List<GraphicsElement>();

		// set image data
		for (var i = 0; i < g1Header.NumEntries; ++i)
		{
			var currElement = g1Element32s[i];

			if (currElement.Flags.HasFlag(DatG1ElementFlags.DuplicatePrevious))
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
				currElement.ImageData = [.. imageData[(int)currElement.Offset..(int)nextOffset]];
			}

			// if rleCompressed, decompress it, except if the duplicate-previous flag is also set - by the current code here, the previous
			// image (which was also compressed) is now decompressed, so we don't need do double-decompress it.
			if (currElement.Flags.HasFlag(DatG1ElementFlags.IsRLECompressed) && !currElement.Flags.HasFlag(DatG1ElementFlags.DuplicatePrevious))
			{
				currElement.ImageData = DecodeRLEImageData(currElement);
			}

			graphicsElements.Add(currElement.Convert(DefaultImageTableNameProvider.GetImageName(i), i));
		}

		return (g1Header, graphicsElements);
	}

	static uint GetNextNonDuplicateOffset(List<DatG1Element32> g1Element32s, int i, uint imageDateLength)
	{
		uint nextOffset;
		do
		{
			nextOffset = i == g1Element32s.Count - 1
				? imageDateLength
				: g1Element32s[i + 1].Offset;

			i++;
		}
		while (i < g1Element32s.Count && g1Element32s[i].Flags.HasFlag(DatG1ElementFlags.DuplicatePrevious));

		return nextOffset;
	}

	public static byte[] DecodeRLEImageData(DatG1Element32 img)
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

	public static LocoObject ReadLocoObject(DatObjectType objectType, MemoryStream stream)
		=> objectType switch
		{
			DatObjectType.Airport => AirportObjectLoader.Load(stream),
			DatObjectType.Bridge => BridgeObjectLoader.Load(stream),
			DatObjectType.Building => BuildingObjectLoader.Load(stream),
			DatObjectType.Cargo => CargoObjectLoader.Load(stream),
			DatObjectType.CliffEdge => CliffEdgeObjectLoader.Load(stream),
			DatObjectType.Climate => ClimateObjectLoader.Load(stream),
			DatObjectType.Competitor => CompetitorObjectLoader.Load(stream),
			DatObjectType.Currency => CurrencyObjectLoader.Load(stream),
			DatObjectType.Dock => DockObjectLoader.Load(stream),
			DatObjectType.HillShapes => HillShapesObjectLoader.Load(stream),
			DatObjectType.Industry => IndustryObjectLoader.Load(stream),
			DatObjectType.InterfaceSkin => InterfaceSkinObjectLoader.Load(stream),
			DatObjectType.Land => LandObjectLoader.Load(stream),
			DatObjectType.LevelCrossing => LevelCrossingObjectLoader.Load(stream),
			DatObjectType.Region => RegionObjectLoader.Load(stream),
			DatObjectType.Road => RoadObjectLoader.Load(stream),
			DatObjectType.RoadExtra => RoadExtraObjectLoader.Load(stream),
			DatObjectType.RoadStation => RoadStationObjectLoader.Load(stream),
			DatObjectType.Scaffolding => ScaffoldingObjectLoader.Load(stream),
			DatObjectType.ScenarioText => ScenarioTextObjectLoader.Load(stream),
			DatObjectType.Snow => SnowObjectLoader.Load(stream),
			DatObjectType.Sound => SoundObjectLoader.Load(stream),
			DatObjectType.Steam => SteamObjectLoader.Load(stream),
			DatObjectType.StreetLight => StreetLightObjectLoader.Load(stream),
			DatObjectType.TownNames => TownNamesObjectLoader.Load(stream),
			DatObjectType.Track => TrackObjectLoader.Load(stream),
			DatObjectType.TrackExtra => TrackExtraObjectLoader.Load(stream),
			DatObjectType.TrackSignal => TrackSignalObjectLoader.Load(stream),
			DatObjectType.TrackStation => TrackStationObjectLoader.Load(stream),
			DatObjectType.Tree => TreeObjectLoader.Load(stream),
			DatObjectType.Tunnel => TunnelObjectLoader.Load(stream),
			DatObjectType.Vehicle => VehicleObjectLoader.Load(stream),
			DatObjectType.Wall => WallObjectLoader.Load(stream),
			DatObjectType.Water => WaterObjectLoader.Load(stream),
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

	public static List<(SoundEffectWaveFormat header, byte[] data)> LoadSoundEffectsFromCSS(string filename)
		=> LoadSoundEffectsFromCSS(File.ReadAllBytes(filename));

	public static List<(SoundEffectWaveFormat header, byte[] data)> LoadSoundEffectsFromCSS(byte[] data)
	{
		var result = new List<(SoundEffectWaveFormat, byte[])>();

		using (var ms = new MemoryStream(data))
		using (var br = new LocoBinaryReader(ms))
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
				var sfx = br.ReadSoundEffect();
				var pcmData = br.ReadBytes((int)pcmLen);
				result.Add((sfx, pcmData));
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
			if (data[i] == LocoConstants.Terminator)
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
