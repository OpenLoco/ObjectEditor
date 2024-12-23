using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects.Sound;
using OpenLoco.Dat.Types;
using System;
using System.Numerics;
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
			=> data.ToArray();

		//public static byte[] Encode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		//	=> encoding switch
		//	{
		//		SawyerEncoding.Uncompressed => data.ToArray(),
		//		SawyerEncoding.RunLengthSingle => EncodeRunLengthSingle(data),
		//		SawyerEncoding.RunLengthMulti => throw new NotImplementedException(),
		//		SawyerEncoding.Rotate => EncodeRotate(data),
		//		_ => throw new InvalidDataException("Unknown chunk encoding scheme"),
		//	};

		public static byte[] EncodeRunLengthSingle(ReadOnlySpan<byte> data)
		{
			using var buffer = new MemoryStream();

			var src = 0;
			var srcEnd = data.Length;
			var srcNormStart = 0;
			byte count = 0;

			while (src < srcEnd - 1)
			{
				if ((count != 0 && data[src] == data[src + 1]) || count > 125)
				{
					buffer.WriteByte((byte)(count - 1));
					buffer.Write(data[srcNormStart..(srcNormStart + count)]);
					srcNormStart += count;
					count = 0;
				}
				if (data[src] == data[src + 1])
				{
					for (; count < 125 && src + count < srcEnd; count++)
					{
						if (data[src] != data[src + count])
						{
							break;
						}
					}
					buffer.WriteByte((byte)(257 - count));
					buffer.WriteByte(data[src]);
					src += count;
					srcNormStart = src;
					count = 0;
				}
				else
				{
					count++;
					src++;
				}
			}
			if (src == srcEnd - 1)
			{
				count++;
			}
			if (count != 0)
			{
				buffer.WriteByte((byte)(count - 1));
				buffer.Write(data[srcNormStart..(srcNormStart + count)]);
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

		public static ReadOnlySpan<byte> WriteLocoObject(string objName, SourceGame sourceGame, SawyerEncoding encoding, ILogger logger, ILocoObject obj, bool allowWritingAsVanilla)
			=> WriteLocoObjectStream(objName, sourceGame, encoding, logger, obj, allowWritingAsVanilla).ToArray();

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
				var variableBytes = objV.Save();
				rawObjStream.Write(variableBytes);
			}

			// graphics data
			SaveImageTable(obj.G1Elements, rawObjStream);

			rawObjStream.Flush();

			//todo: actually use encoding
			using var encodedObjStream = new MemoryStream(Encode(encoding, rawObjStream.ToArray()));

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
			s5Header.Checksum = SawyerStreamUtils.ComputeObjectChecksum(headerFlag, asciiName, encodedObjStream.ToArray());

			var objHeader = new ObjectHeader(SawyerEncoding.Uncompressed, (uint32_t)encodedObjStream.Length);

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
				// write G1Header
				objStream.Write(BitConverter.GetBytes((uint32_t)g1Elements.Count));
				objStream.Write(BitConverter.GetBytes((uint32_t)g1Elements.Sum(x => x.ImageData.Length)));

				var offsetBytesIntoImageData = 0;
				// write G1Element headers
				foreach (var g1Element in g1Elements)
				{
					// we need to update the offsets of the image data
					// and we're not going to compress the data on save, so make sure the RLECompressed flag is not set
					var newElement = g1Element with
					{
						Offset = (uint)offsetBytesIntoImageData,
						Flags = g1Element.Flags & ~G1ElementFlags.IsRLECompressed
					};

					objStream.Write(newElement.Write());
					offsetBytesIntoImageData += g1Element.ImageData.Length;
				}

				// write G1Elements ImageData
				foreach (var g1Element in g1Elements)
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
