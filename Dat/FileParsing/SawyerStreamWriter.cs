using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects.Sound;
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

		public static void Save(string filename, string objName, ILocoObject locoObject, ILogger? logger = null)
		{
			ArgumentNullException.ThrowIfNull(locoObject);

			logger?.Info($"Writing \"{objName}\" to {filename}");

			var objBytes = WriteLocoObject(objName, locoObject);

			var stream = File.Create(filename);
			stream.Write(objBytes);
			stream.Flush();
			stream.Close();
		}

		public static ReadOnlySpan<byte> WriteLocoObject(string objName, ILocoObject obj)
		 => WriteLocoObjectStream(objName, obj).ToArray();

		public static MemoryStream WriteLocoObjectStream(string objName, ILocoObject obj)
		{
			using var objStream = new MemoryStream();

			// obj
			var objBytes = ByteWriter.WriteLocoStruct(obj.Object);
			objStream.Write(objBytes);

			// string table
			foreach (var ste in obj.StringTable.Table)
			{
				foreach (var language in ste.Value.Where(str => !string.IsNullOrEmpty(str.Value))) // skip strings with empty content
				{
					objStream.WriteByte((byte)language.Key);

					var strBytes = Encoding.Latin1.GetBytes(language.Value);
					objStream.Write(strBytes, 0, strBytes.Length);
					objStream.WriteByte((byte)'\0');
				}

				objStream.WriteByte(0xff);
			}

			// variable data
			if (obj.Object is ILocoStructVariableData objV)
			{
				var variableBytes = objV.Save();
				objStream.Write(variableBytes);
			}

			// graphics data
			SaveImageTable(obj.G1Elements, objStream);

			objStream.Flush();

			// now obj is written, we can calculate the few bits of metadata (checksum and length) for the headers

			// s5 header
			var attr = AttributeHelper.Get<LocoStructTypeAttribute>(obj.Object.GetType());
			var s5Header = new S5Header(objName, 0)
			{
				SourceGame = SourceGame.Custom,
				ObjectType = attr!.ObjectType
			};

			// calculate checksum
			var headerFlag = BitConverter.GetBytes(s5Header.Flags).AsSpan()[0..1];
			var asciiName = objName.PadRight(8, ' ').Take(8).Select(c => (byte)c).ToArray();
			var checksum = SawyerStreamUtils.ComputeDatChecksum(headerFlag, asciiName, objStream.ToArray());
			s5Header = s5Header with { Checksum = checksum };

			var objHeader = new ObjectHeader(SawyerEncoding.Uncompressed, (uint32_t)objStream.Length);

			// actual writing
			var headerStream = new MemoryStream();

			// s5 header
			headerStream.Write(s5Header.Write());

			// obj header
			headerStream.Write(objHeader.Write());

			// loco object itself, including string and graphics table
			headerStream.Write(objStream.ToArray());

			// stream cleanup
			headerStream.Flush();

			headerStream.Close();
			objStream.Close();

			return headerStream;
		}

		private static void SaveImageTable(List<G1Element32> g1Elements, Stream objStream)
		{
			if (g1Elements != null && g1Elements.Count != 0)
			{
				// write G1Header
				objStream.Write(BitConverter.GetBytes((uint32_t)g1Elements.Count));
				objStream.Write(BitConverter.GetBytes((uint32_t)g1Elements.Sum(x => G1Element32.StructLength + x.ImageData.Length)));

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

		//public static ReadOnlySpan<byte> Encode(SawyerEncoding encoding, ReadOnlySpan<byte> data, ILogger? logger = null)
		//{
		//	switch (encoding)
		//	{
		//		case SawyerEncoding.Uncompressed:
		//			return data;
		//		case SawyerEncoding.RunLengthSingle:
		//			return EncodeRunLengthSingle(data);
		//		//case SawyerEncoding.runLengthMulti:
		//		//	return encodeRunLengthMulti(decodeRunLengthSingle(data));
		//		//case SawyerEncoding.rotate:
		//		//	return encodeRotate(data);
		//		default:
		//			logger?.Error("Unknown chunk encoding scheme");
		//			throw new InvalidDataException("Unknown encoding");
		//	}
		//}

		//public static void WriteToFile(string filename, ReadOnlySpan<byte> s5Header, ReadOnlySpan<byte> objectHeader, ReadOnlySpan<byte> encodedData)
		//{
		//	var stream = File.Create(filename);
		//	stream.Write(s5Header);
		//	stream.Write(objectHeader);
		//	stream.Write(encodedData);
		//	stream.Flush();
		//	stream.Close();
		//}

		// taken from openloco SawyerStreamReader::encodeRunLengthSingle
		// not sure why it doesn't work, but it doesn't work. gets the first 10 or so bytes correct for SIGC3.dat but then fails
		//private static Span<byte> EncodeRunLengthSingle(ReadOnlySpan<byte> data)
		//{
		//	List<byte> buffer = [];
		//	var src = 0; // ptr
		//	var srcNormStart = 0; // ptr
		//	var srcEnd = data.Length;
		//	var count = 0;

		//	while (src < srcEnd - 1)
		//	{
		//		if ((count != 0 && data[src] == data[src + 1]) || count > 125)
		//		{
		//			buffer.Add((byte)(count - 1));
		//			buffer.AddRange(Enumerable.Repeat(data[srcNormStart], count));
		//			srcNormStart += count;
		//			count = 0;
		//		}

		//		if (data[src] == data[src + 1])
		//		{
		//			for (; count < 125 && src + count < srcEnd; count++)
		//			{
		//				if (data[src] != data[count])
		//				{
		//					break;
		//				}
		//			}

		//			buffer.Add((byte)(257 - count));
		//			buffer.Add(data[src]);
		//			src += count;
		//			srcNormStart = src;
		//			count = 0;
		//		}
		//		else
		//		{
		//			count++;
		//			src++;
		//		}
		//	}

		//	if (data[src] == data[srcEnd - 1])
		//	{
		//		count++;
		//	}

		//	if (count != 0)
		//	{
		//		buffer.Add((byte)(count - 1));
		//		buffer.AddRange(Enumerable.Repeat(data[srcNormStart], count));
		//	}

		//	// convert to span
		//	Span<byte> encodedSpan = new byte[buffer.Count];
		//	var counter = 0;
		//	foreach (var b in buffer)
		//	{
		//		encodedSpan[counter++] = b;
		//	}

		//	return encodedSpan;
		//}

		public static void SaveG1(G1Dat g1, string filename)
		{
			using (var fs = File.OpenWrite(filename))
			{
				SaveImageTable(g1.G1Elements, fs);
			}
		}
	}
}
