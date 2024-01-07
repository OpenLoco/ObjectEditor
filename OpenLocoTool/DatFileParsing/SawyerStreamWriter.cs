using System.Text;
using OpenLocoTool.Data;
using OpenLocoTool.Headers;
using OpenLocoToolCommon;

namespace OpenLocoTool.DatFileParsing
{
	public static class SawyerStreamWriter
	{
		public static void Save(string filepath, string objName, ILocoObject locoObject, ILogger? logger = null)
		{
			ArgumentNullException.ThrowIfNull(locoObject);

			logger?.Info($"Writing \"{objName}\" to {filepath}");

			var objBytes = WriteLocoObject(objName, locoObject);

			var stream = File.Create(filepath);
			stream.Write(objBytes);
			stream.Flush();
			stream.Close();
		}

		public static ReadOnlySpan<byte> WriteLocoObject(string objName, ILocoObject obj)
		{
			var objStream = new MemoryStream();

			// obj
			var objBytes = ByteWriter.WriteLocoStruct(obj.Object);
			objStream.Write(objBytes);

			// string table
			foreach (var ste in obj.StringTable.table)
			{
				foreach (var language in ste.Value)
				{
					objStream.WriteByte((byte)language.Key);

					var strBytes = Encoding.ASCII.GetBytes(language.Value);
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
			if (obj.G1Elements != null && obj.G1Elements.Count != 0)
			{
				// write G1Header
				objStream.Write(BitConverter.GetBytes(obj.G1Elements.Count));
				objStream.Write(BitConverter.GetBytes(obj.G1Elements.Sum(x => G1Element32.StructLength + x.ImageData.Length)));

				var offsetBytesIntoImageData = 0;
				// write G1Element headers
				foreach (var g1Element in obj.G1Elements)
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
				foreach (var g1Element in obj.G1Elements)
				{
					objStream.Write(g1Element.ImageData);
				}
			}

			objStream.Flush();

			// now obj is written, we can calculate the few bits of metadata (checksum and length) for the headers

			// s5 header
			uint32_t checksum = 0; // todo: compute this
			var s5Header = new S5Header(objName, checksum)
			{
				SourceGame = SourceGame.Custom
			};
			var attr = AttributeHelper.Get<LocoStructTypeAttribute>(obj.Object.GetType());
			s5Header.ObjectType = attr!.ObjectType;

			// calculate checksum
			var headerFlag = BitConverter.GetBytes(s5Header.Flags).AsSpan()[0..1];
			var asciiName = objName.Take(8).Select(c => (byte)c).ToArray();
			checksum = SawyerStreamUtils.ComputeObjectChecksum(headerFlag, asciiName, objStream.ToArray());
			s5Header = s5Header with { Checksum = checksum };

			var objHeader = new ObjectHeader(SawyerEncoding.Uncompressed, (uint32_t)objStream.Length);

			// actual writing
			var headerStream = new MemoryStream();

			// s5 header
			headerStream.Write(s5Header.Write());

			// obj header
			headerStream.Write(objHeader.Write());

			// loco object itself
			headerStream.Write(objStream.ToArray());

			// stream cleanup
			headerStream.Flush();

			headerStream.Close();
			objStream.Close();

			return headerStream.ToArray();
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

		//public static void WriteToFile(string filepath, ReadOnlySpan<byte> s5Header, ReadOnlySpan<byte> objectHeader, ReadOnlySpan<byte> encodedData)
		//{
		//	var stream = File.Create(filepath);
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
	}
}
