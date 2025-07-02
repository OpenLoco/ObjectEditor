using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using System.Text;

namespace OpenLoco.Dat.FileParsing
{
	public static class SawyerStreamWriter
	{
		public static RiffWavHeader WaveFormatExToRiff(WaveFormatEx hdr, int pcmDataLength)
			=> new(
				0x46464952, // "RIFF"
				(uint)(pcmDataLength + 36), // file size
				0x45564157, // "WAVE"
				0x20746d66, // "fmt "
				16, // size of fmt chunk
				1, // format tag
				(ushort)hdr.NumberOfChannels,
				(uint)hdr.SampleRate,
				(uint)hdr.AverageBytesPerSecond,
				4, //(ushort)waveFHeader.BlockAlign,
				16, //(ushort)waveFHeader.BitsPerSample,
				0x61746164, // "data"
				(uint)pcmDataLength // data size
				);

		public static WaveFormatEx RiffToWaveFormatEx(RiffWavHeader hdr)
			=> new(1, (short)hdr.NumberOfChannels, (int)hdr.SampleRate, (int)hdr.ByteRate, 2, 16, 0);
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

		public static byte[] SaveSoundEffectsToCSS(List<(RiffWavHeader header, byte[] data)> sounds)
		{
			using (var ms = new MemoryStream())
			using (var br = new BinaryWriter(ms))
			{
				// total sounds
				br.Write((uint)sounds.Count);

				var currOffset = 4 + (sounds.Count * 4); // 4 for sound count, then 32 sounds each have a 4-byte offset. its always 33 * 4 = 132 to start.

				// sound offsets
				foreach (var (header, data) in sounds)
				{
					br.Write((uint)currOffset);
					currOffset += 4 + data.Length + ObjectAttributes.StructSize<WaveFormatEx>();
				}

				// pcm data
				foreach (var (header, data) in sounds)
				{
					var waveHdr = RiffToWaveFormatEx(header);
					br.Write((uint)data.Length);
					br.Write(ByteWriter.WriteLocoStruct(waveHdr));
					br.Write(data);
				}

				ms.Flush();
				ms.Close();

				return ms.ToArray();
			}
		}
		public static byte[] SaveMusicToDat(RiffWavHeader header, byte[] data)
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
		public static void ExportMusicAsWave(string filename, RiffWavHeader header, byte[] pcmData)
		{
			using (var stream = File.Create(filename))
			{
				stream.Write(ByteWriter.WriteLocoStruct(header));
				stream.Write(pcmData);
				stream.Flush();
			}
		}

		public static void Save(string filename, string objName, SourceGame sourceGame, SawyerEncoding encoding, ILocoObject locoObject, ILogger logger, bool allowWritingAsVanilla)
		{
			ArgumentNullException.ThrowIfNull(locoObject);

			logger.Info($"Writing \"{objName}\" to {filename}");

			var objBytes = WriteLocoObject(objName, sourceGame, encoding, logger, locoObject, allowWritingAsVanilla);

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
			buffer.WriteByte(0xFF);
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
					buffer.WriteByte(0xFF);
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

		public static byte[] EncodeRLEImageData(G1Element32 img)
			=> EncodeRLEImageData(img.Flags, img.ImageData, img.Width, img.Height);

		// this is ugly as all hell but it works. plenty of room for cleanup and optimisation
		public static byte[] EncodeRLEImageData(G1ElementFlags flags, byte[] imageData, int width, int height)
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

		public static ReadOnlySpan<byte> WriteLocoObject(string objName, SourceGame sourceGame, SawyerEncoding encoding, ILogger logger, ILocoObject obj, bool allowWritingAsVanilla)
			=> WriteLocoObjectStream(objName, sourceGame, encoding, logger, obj, allowWritingAsVanilla).ToArray();

		public static ReadOnlySpan<byte> WriteChunk(ILocoStruct str, SawyerEncoding encoding)
			=> WriteChunkCore(ByteWriter.WriteLocoStruct(str), encoding);

		public static byte[] WriteChunkCore(ReadOnlySpan<byte> source, SawyerEncoding encoding)
		{
			var encoded = Encode(encoding, source);
			var objHeader = new ObjectHeader(encoding, (uint)encoded.Length).Write();
			return [.. objHeader.ToArray(), .. encoded];
		}

		public static MemoryStream WriteLocoObjectStream(string objName, SourceGame sourceGame, SawyerEncoding encoding, ILogger logger, ILocoObject obj, bool allowWritingAsVanilla)
		{
			using var rawObjStream = new MemoryStream();

			// obj
			var objBytes = ByteWriter.WriteLocoStruct(obj.Object);
			rawObjStream.Write(objBytes);

			// string table
			foreach (var ste in obj.StringTable.Table)
			{
				foreach (var language in ste.Value.Where(str => !string.IsNullOrEmpty(str.Value))) // skip strings with empty content
				{
					rawObjStream.WriteByte((byte)language.Key);

					var strBytes = Encoding.Latin1.GetBytes(language.Value);
					rawObjStream.Write(strBytes, 0, strBytes.Length);
					rawObjStream.WriteByte((byte)'\0');
				}

				rawObjStream.WriteByte(0xff);
			}

			// variable data
			if (obj.Object is ILocoStructVariableData objV)
			{
				var variableBytes = objV.SaveVariable();
				rawObjStream.Write(variableBytes);
			}

			// graphics data
			SaveImageTable(obj.G1Elements, rawObjStream);

			rawObjStream.Flush();

			// now obj is written, we can calculate the few bits of metadata (checksum and length) for the headers

			// s5 header
			var attr = AttributeHelper.Get<LocoStructTypeAttribute>(obj.Object.GetType());

			if (sourceGame == SourceGame.Vanilla && !allowWritingAsVanilla)
			{
				sourceGame = SourceGame.Custom;
				logger.Warning("Cannot save an object as 'Vanilla' - using 'Custom' instead");
			}

			var s5Header = new S5Header(objName, 0)
			{
				SourceGame = sourceGame,
				ObjectType = attr!.ObjectType
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

		static void SaveImageTable(List<G1Element32> g1Elements, Stream objStream)
		{
			if (g1Elements != null && g1Elements.Count != 0)
			{
				// encode if necessary
				List<G1Element32> encoded = [];
				var offsetBytesIntoImageData = 0;
				foreach (var g1Element in g1Elements)
				{
					// this copies everything but it should be fine for now
					var newElement = g1Element with
					{
						ImageData = g1Element.GetImageDataForSave(),
						Offset = (uint)offsetBytesIntoImageData,
					};

					offsetBytesIntoImageData += newElement.ImageData.Length;
					encoded.Add(newElement);
				}

				// write G1Header
				objStream.Write(BitConverter.GetBytes((uint32_t)encoded.Count));
				objStream.Write(BitConverter.GetBytes((uint32_t)encoded.Sum(x => x.ImageData.Length)));

				// write G1Element headers
				foreach (var g1Element in encoded)
				{
					objStream.Write(g1Element.Write());
				}

				// write G1Elements ImageData
				foreach (var g1Element in encoded)
				{
					objStream.Write(g1Element.ImageData);
				}
			}
		}

		public static void SaveG1(string filename, G1Dat g1)
		{
			using (var fs = File.OpenWrite(filename))
			{
				SaveImageTable(g1.G1Elements, fs);
			}
		}
	}
}
