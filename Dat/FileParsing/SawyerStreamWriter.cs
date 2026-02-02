using Common.Logging;
using Dat.Converters;
using Dat.Data;
using Dat.Loaders;
using Dat.Types;
using Dat.Types.Audio;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Sound;
using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dat.FileParsing;

public static class SawyerStreamWriter
{
	public static DatMusicWaveFormat LocoWaveFormatToRiff(SoundEffectWaveFormat hdr, int pcmDataLength)
		=> new(
			0x46464952, // "RIFF"
			(uint)(pcmDataLength + 36), // file size
			0x45564157, // "WAVE"
			0x20746d66, // "fmt "
			16, // size of fmt chunk
			1, // format tag
			(ushort)hdr.Channels,
			(uint)hdr.SampleRate,
			(uint)hdr.AverageBytesPerSecond,
			4, //(ushort)waveFHeader.BlockAlign,
			16, //(ushort)waveFHeader.BitsPerSample,
			0x61746164, // "data"
			(uint)pcmDataLength // data size
			);

	//public static LocoWaveFormat RiffToWaveFormatEx(RiffWavHeader hdr)
	//	=> new(1, (short)hdr.Channels, (int)hdr.SampleRate, (int)hdr.ByteRate, 2, 16, 0);
	//0x46464952, // "RIFF"
	//(uint)(pcmDataLength + 36), // file size
	//0x45564157, // "WAVE"
	//0x20746d66, // "fmt "
	//16, // size of fmt chunk
	//1, // format tag
	//(ushort)hdr.NumberOfChannels,
	//(uint)hdr.SampleRate,
	//(uint)hdr.AverageBytesPerSecond,
	//4, //(ushort)waveFHeader.BlockAlign,
	//16, //(ushort)waveFHeader.BitsPerSample,
	//0x61746164, // "data"
	//(uint)pcmDataLength // data size
	//);

	public static byte[] SaveSoundEffectsToCSS(List<(SoundEffectWaveFormat locoWaveHeader, byte[] data)> sounds)
	{
		using (var ms = new MemoryStream())
		using (var br = new LocoBinaryWriter(ms))
		{
			// total sounds
			br.Write((uint)sounds.Count);

			var currOffset = 4 + (sounds.Count * 4); // 4 for sound count, then 32 sounds each have a 4-byte offset. its always 33 * 4 = 132 to start.

			// sound offsets
			foreach (var (header, data) in sounds)
			{
				br.Write((uint)currOffset);
				currOffset += 4 + data.Length + ObjectAttributes.StructSize<DatSoundEffectWaveFormat>();
			}

			// pcm data
			foreach (var (header, data) in sounds)
			{
				br.Write((uint)data.Length);
				br.Write(header);
				br.Write(data);
			}

			ms.Flush();
			ms.Close();

			return ms.ToArray();
		}
	}

	public static byte[] SaveMusicToDat(DatMusicWaveFormat header, byte[] data)
	{
		using (var ms = new MemoryStream())
		using (var br = new BinaryWriter(ms))
		{
			br.Write(ByteWriter.WriteLocoStruct(header));
			br.Write(data);

			ms.Flush();
			ms.Close();

			return ms.ToArray();
		}
	}

	public static void Save(string filename, string objName, ObjectSource objectSource, SawyerEncoding encoding, LocoObject locoObject, ILogger logger, bool allowWritingAsVanilla)
	{
		ArgumentNullException.ThrowIfNull(locoObject);

		logger.Info($"Writing \"{objName}\" to {filename}");

		var objBytes = WriteLocoObject(objName, locoObject.ObjectType, objectSource, encoding, logger, locoObject, allowWritingAsVanilla).ToArray();

		try
		{
			var stream = File.Create(filename);
			stream.Write(objBytes);
			stream.Flush();
			stream.Close();
		}
		catch (Exception ex)
		{
			// will usually be UnauthorizedAccessException
			logger?.Error(ex);
			return;
		}

		logger.Info($"{objName} successfully saved to {filename}");
	}

	public static byte[] Encode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		=> encoding switch
		{
			SawyerEncoding.Uncompressed => data.ToArray(),
			SawyerEncoding.RunLengthSingle => EncodeRunLengthSingle(data),
			SawyerEncoding.RunLengthMulti => EncodeRunLengthSingle(EncodeRunLengthMulti(data)),
			SawyerEncoding.Rotate => EncodeRotate(data),
			_ => throw new InvalidDataException("Unknown chunk encoding scheme"),
		};

	static byte[] EncodeRunLengthSingle(ReadOnlySpan<byte> data)
	{
		using var buffer = new MemoryStream();
		var src = data;
		var srcPtr = 0;
		var srcLen = src.Length;
		var srcNormStart = 0;
		byte count = 0;

		while (srcPtr < srcLen - 1)
		{
			if ((count != 0 && src[srcPtr] == src[srcPtr + 1]) || count > 125)
			{
				buffer.WriteByte((byte)(count - 1));
				buffer.Write(src[srcNormStart..(srcNormStart + count)]);
				srcNormStart += count;
				count = 0;
			}

			if (src[srcPtr] == src[srcPtr + 1])
			{
				for (; count < 125 && srcPtr + count < srcLen; count++)
				{
					if (src[srcPtr] != src[srcPtr + count])
					{
						break;
					}
				}

				var a = (byte)(257 - count);
				var b = src[srcPtr];
				buffer.WriteByte(a);
				buffer.WriteByte(b);
				srcPtr += count;
				srcNormStart = srcPtr;
				count = 0;
			}
			else
			{
				count++;
				srcPtr++;
			}
		}

		if (srcPtr == srcLen - 1)
		{
			count++;
		}

		if (count != 0)
		{
			buffer.WriteByte((byte)(count - 1));
			buffer.Write(src[srcNormStart..(srcNormStart + count)]);
		}

		return buffer.ToArray();
	}

	static byte[] EncodeRunLengthMulti(ReadOnlySpan<byte> data)
	{
		using var buffer = new MemoryStream();

		var srcLen = data.Length;
		if (srcLen == 0)
		{
			return [];
		}

		// Need to emit at least one byte, otherwise there is nothing to repeat
		buffer.WriteByte(LocoConstants.Terminator);
		buffer.WriteByte(data[0]);

		// Iterate through remainder of the source buffer
		for (var i = 1; i < srcLen;)
		{
			var searchIndex = (i < 32) ? 0 : (i - 32);
			var searchEnd = i - 1;

			var bestRepeatIndex = 0;
			var bestRepeatCount = 0;
			for (var repeatIndex = searchIndex; repeatIndex <= searchEnd; repeatIndex++)
			{
				var repeatCount = 0;
				var maxRepeatCount = Math.Min(Math.Min(7, searchEnd - repeatIndex), srcLen - i - 1);
				// maxRepeatCount should not exceed srcLen
				//assert(repeatIndex + maxRepeatCount < srcLen);
				//assert(i + maxRepeatCount < srcLen);
				for (size_t j = 0; j <= maxRepeatCount; j++)
				{
					if (data[(int)(repeatIndex + j)] == data[(int)(i + j)])
					{
						repeatCount++;
					}
					else
					{
						break;
					}
				}

				if (repeatCount > bestRepeatCount)
				{
					bestRepeatIndex = repeatIndex;
					bestRepeatCount = repeatCount;

					// Maximum repeat count is 8
					if (repeatCount == 8)
					{
						break;
					}
				}
			}

			if (bestRepeatCount == 0)
			{
				buffer.WriteByte(LocoConstants.Terminator);
				buffer.WriteByte(data[i]);
				i++;
			}
			else
			{
				buffer.WriteByte((uint8_t)((bestRepeatCount - 1) | ((32 - (i - bestRepeatIndex)) << 3)));
				i += bestRepeatCount;
			}
		}

		return buffer.ToArray();
	}

	static byte[] EncodeRotate(ReadOnlySpan<byte> data)
	{
		using var buffer = new MemoryStream();

		uint8_t code = 1;
		for (var i = 0; i < data.Length; ++i)
		{
			buffer.WriteByte(RotateLeft(data[i], code));
			code = (uint8_t)((code + 2) & 7);
		}

		return buffer.ToArray();
	}

	static uint8_t RotateLeft(uint8_t value, uint8_t shift)
	{
		shift &= 7; // Ensure shift is within 0-7 for 8-bit bytes
		return (uint8_t)((value << shift) | (value >> (8 - shift)));
	}
	public static byte[] EncodeRLEImageData(GraphicsElement img)
		=> EncodeRLEImageData((DatG1ElementFlags)img.Flags, img.ImageData, img.Width, img.Height);

	public static byte[] EncodeRLEImageData(DatG1Element32 img)
		=> EncodeRLEImageData(img.Flags, img.ImageData, img.Width, img.Height);

	// this is ugly as all hell but it works. plenty of room for cleanup and optimisation
	public static byte[] EncodeRLEImageData(DatG1ElementFlags flags, byte[] imageData, int width, int height)
	{
		using var ms = new MemoryStream();

		var lines = new List<List<(int StartX, List<byte> RunBytes)>>();

		// calculate the segments per line in the input image
		foreach (var line in imageData.Chunk(width))
		{
			List<(int StartX, List<byte> RunBytes)> segments = [];
			for (var x = 0; x < width;)
			{
				// find the start of a segment. previous pixel may be a segment
				if (line[x] != 0x0)
				{
					// find the end
					var startOfSegment = x;
					List<byte> run = [];
					while (x < width && line[x] != 0x0 && run.Count < 127) // runs can only be 127 bytes in length. if the run is truly longer, then it gets split into multiple runs
					{
						run.Add(line[x]);
						x++;
					}

					segments.Add((startOfSegment, run));
				}
				else
				{
					x++;
				}
			}

			lines.Add(segments);
		}

		// write source pointers. will be (2 * img.Height) bytes. need to know RLE data first to know the offsets
		var headerOffset = lines.Count * 2;
		var bytesTotal = 0;
		for (var yy = 0; yy < height; ++yy)
		{
			// bytes per previous line is the sum of all the bytes in the runs plus the number of line segments * 2
			var bytesPreviousLine = yy == 0
				? 0
				: lines[yy - 1].Sum(x => x.RunBytes.Count) + (Math.Max(1, lines[yy - 1].Count) * 2);
			bytesTotal += bytesPreviousLine;

			var value = headerOffset + bytesTotal;
			var low = (byte)(value & 0xFF);
			var high = (byte)((value >> 8) & 0xFF);
			ms.WriteByte(low);
			ms.WriteByte(high);
		}

		// write lines
		foreach (var line in lines)
		{
			// if fully empty line
			if (line.Count == 0)
			{
				ms.WriteByte(0x80);
				ms.WriteByte(0x00);
				continue;
			}

			// line has at least one segment
			for (var i = 0; i < line.Count; ++i)
			{
				var (StartX, RunBytes) = line[i];

				if (RunBytes.Count > 127)
				{
					throw new ArgumentException("Segment length cannot exceed 127 pixels");
				}

				var count = i == line.Count - 1 ? RunBytes.Count | 0x80 : RunBytes.Count;
				ms.WriteByte((byte)count);
				ms.WriteByte((byte)StartX);
				ms.Write(RunBytes.ToArray());
			}
		}

		return ms.ToArray();
	}

	public static ReadOnlySpan<byte> WriteChunk(ILocoStruct str, SawyerEncoding encoding)
		=> WriteChunkCore(ByteWriter.WriteLocoStruct(str), encoding);

	public static byte[] WriteChunkCore(ReadOnlySpan<byte> source, SawyerEncoding encoding)
	{
		var encoded = Encode(encoding, source);
		var objHeader = new ObjectHeader(encoding, (uint)encoded.Length).Write();
		return [.. objHeader.ToArray(), .. encoded];
	}

	public static void WriteLocoObject(Stream stream, LocoObject obj)
	{
		switch (obj.ObjectType)
		{
			case ObjectType.Airport:
				AirportObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Bridge:
				BridgeObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Building:
				BuildingObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Cargo:
				CargoObjectLoader.Save(stream, obj);
				break;
			case ObjectType.CliffEdge:
				CliffEdgeObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Climate:
				ClimateObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Competitor:
				CompetitorObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Currency:
				CurrencyObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Dock:
				DockObjectLoader.Save(stream, obj);
				break;
			case ObjectType.HillShapes:
				HillShapesObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Industry:
				IndustryObjectLoader.Save(stream, obj);
				break;
			case ObjectType.InterfaceSkin:
				InterfaceSkinObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Land:
				LandObjectLoader.Save(stream, obj);
				break;
			case ObjectType.LevelCrossing:
				LevelCrossingObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Region:
				RegionObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Road:
				RoadObjectLoader.Save(stream, obj);
				break;
			case ObjectType.RoadExtra:
				RoadExtraObjectLoader.Save(stream, obj);
				break;
			case ObjectType.RoadStation:
				RoadStationObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Scaffolding:
				ScaffoldingObjectLoader.Save(stream, obj);
				break;
			case ObjectType.ScenarioText:
				ScenarioTextObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Snow:
				SnowObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Sound:
				SoundObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Steam:
				SteamObjectLoader.Save(stream, obj);
				break;
			case ObjectType.StreetLight:
				StreetLightObjectLoader.Save(stream, obj);
				break;
			case ObjectType.TownNames:
				TownNamesObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Track:
				TrackObjectLoader.Save(stream, obj);
				break;
			case ObjectType.TrackExtra:
				TrackExtraObjectLoader.Save(stream, obj);
				break;
			case ObjectType.TrackSignal:
				TrackSignalObjectLoader.Save(stream, obj);
				break;
			case ObjectType.TrackStation:
				TrackStationObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Tree:
				TreeObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Tunnel:
				TunnelObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Vehicle:
				VehicleObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Wall:
				WallObjectLoader.Save(stream, obj);
				break;
			case ObjectType.Water:
				WaterObjectLoader.Save(stream, obj);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(obj.ObjectType), $"unknown object type {obj.ObjectType}");
		}
	}

	public static MemoryStream WriteLocoObject(string objName, ObjectType objectType, ObjectSource objectSource, SawyerEncoding encoding, ILogger logger, LocoObject obj, bool allowWritingAsVanilla)
	{
		var validationResults = new List<ValidationResult>();
		if (!Validator.TryValidateObject(obj.Object, new ValidationContext(obj.Object), validationResults))
		{
			throw new ArgumentException($"{objName} was invalid: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}", nameof(obj));
		}

		using var rawObjStream = new MemoryStream();
		WriteLocoObject(rawObjStream, obj);
		rawObjStream.Flush();

		// now obj is written, we can calculate the few bits of metadata (checksum and length) for the headers

		// s5 header
		var sourceGame = objectSource.Convert();
		if (sourceGame == DatObjectSource.Vanilla && !allowWritingAsVanilla)
		{
			sourceGame = DatObjectSource.Custom;
			logger.Warning("Cannot save an object as 'Vanilla' - using 'Custom' instead");
		}

		var s5Header = new S5Header(objName, 0)
		{
			ObjectSource = sourceGame,
			ObjectType = objectType.Convert(),
		};

		// calculate checksum
		var headerFlag = BitConverter.GetBytes(s5Header.Flags).AsSpan()[0..1];
		var asciiName = objName.PadRight(8, ' ').Take(8).Select(c => (byte)c).ToArray();
		var rawObjBytes = rawObjStream.ToArray();
		s5Header.Checksum = SawyerStreamUtils.ComputeObjectChecksum(headerFlag, asciiName, rawObjBytes);

		using var encodedObjStream = new MemoryStream(Encode(encoding, rawObjBytes));
		var objHeader = new ObjectHeader(encoding, (uint32_t)encodedObjStream.Length);

		// actual writing
		var headerStream = new MemoryStream();

		// s5 header
		headerStream.Write(s5Header.Write());

		// obj header
		headerStream.Write(objHeader.Write());

		// loco object itself, including string and graphics table
		headerStream.Write(encodedObjStream.ToArray());

		// stream cleanup
		headerStream.Flush();

		headerStream.Close();
		encodedObjStream.Close();

		return headerStream;
	}

	public static void WriteVariableStream(Stream ms, LocoObject obj)
	{
		if (obj.Object is ILocoStructVariableData objV)
		{
			var variableBytes = objV.SaveVariable();
			ms.Write(variableBytes);
		}
	}

	public static void WriteStringTable(Stream ms, StringTable table)
	{
		const bool wantVanillaExact = false;
		if (wantVanillaExact)
		{
			WriteVanillaStringTable(ms, table);
		}
		else
		{
			WriteSimpleStringTable(ms, table);
		}
	}

	// this is a byte-perfect method for recreating the vanilla objects (specifically Currency)
	// however it has unnecessary bytes when strings are missing in certain languages
	// but other strings in the table have values for that language
	static void WriteVanillaStringTable(Stream ms, StringTable table)
	{
		//var languagesUsed = table.Table
		//	.Select(x => x.Value)
		//	.Select(x => x
		//		.Where(str => !string.IsNullOrEmpty(str.Value))
		//		.Select(str => str.Key))
		//	.SelectMany(x => x)
		//	.Distinct()
		//	.ToImmutableHashSet();

		List<LanguageId> languagesUsed = [LanguageId.English_UK, LanguageId.English_US];

		foreach (var ste in table.Table)
		{
			foreach (var language in ste.Value)
			{
				// skip strings with empty content
				if (!string.IsNullOrEmpty(language.Value))
				{
					ms.WriteByte((uint8_t)language.Key);
					ms.Write(Encoding.Latin1.GetBytes(language.Value));
					ms.WriteByte((uint8_t)'\0');
				}
				else if (languagesUsed.Contains(language.Key)) // but if the string is empty, AND its language has other valid strings, vanilla objects actually wrote these useless bytes
				{
					// vanilla currency objects do this!!!!
					ms.WriteByte((uint8_t)language.Key);
					ms.WriteByte((uint8_t)'\0');
				}
			}

			ms.WriteByte(LocoConstants.Terminator);
		}
	}

	// this is a simplified and more-correct way to write the string table to bytes
	// it is perfectly compatible with vanilla loco, but doesn't produce byte-accurate objects
	static void WriteSimpleStringTable(Stream ms, StringTable table)
	{
		foreach (var ste in table.Table)
		{
			foreach (var language in ste.Value.Where(x => !string.IsNullOrEmpty(x.Value))) // skip strings with empty content
			{
				ms.WriteByte((uint8_t)language.Key);
				ms.Write(Encoding.Latin1.GetBytes(language.Value));
				ms.WriteByte((uint8_t)'\0');
			}

			ms.WriteByte(LocoConstants.Terminator);
		}
	}

	public static void WriteImageTable(Stream ms, List<GraphicsElement> graphicsElements)
	{
		var g1Elements = graphicsElements.Select(x => x.Convert()).ToList();

		if (g1Elements != null && g1Elements.Count != 0)
		{
			// encode if necessary
			List<DatG1Element32> encoded = [];
			var offsetBytesIntoImageData = 0U;
			var previousOffset = 0U;
			foreach (var g1Element in g1Elements)
			{
				// this copies everything but it should be fine for now
				var newElement = g1Element with
				{
					ImageData = g1Element.Flags.HasFlag(DatG1ElementFlags.DuplicatePrevious) ? [] : g1Element.GetImageDataForSave(),
					Offset = g1Element.Flags.HasFlag(DatG1ElementFlags.DuplicatePrevious) ? previousOffset : offsetBytesIntoImageData,
				};

				offsetBytesIntoImageData += (uint32_t)newElement.ImageData.Length;
				encoded.Add(newElement);

				previousOffset = newElement.Offset;
			}

			// write G1Header
			ms.Write(BitConverter.GetBytes((uint32_t)encoded.Count));
			ms.Write(BitConverter.GetBytes((uint32_t)encoded.Sum(x => x.ImageData.Length)));

			// write G1Element headers
			foreach (var g1Element in encoded)
			{
				ms.Write(g1Element.Write());
			}

			// write G1Elements ImageData
			foreach (var g1Element in encoded)
			{
				ms.Write(g1Element.ImageData);
			}
		}
	}

	public static void SaveG1(string filename, G1Dat g1)
	{
		using (var fs = File.OpenWrite(filename))
		{
			WriteImageTable(fs, g1.ImageTable.GraphicsElements);
		}
	}
}
