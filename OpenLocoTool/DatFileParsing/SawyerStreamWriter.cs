using System.Text;
using OpenLocoTool.Headers;
using OpenLocoToolCommon;

namespace OpenLocoTool.DatFileParsing
{
	public static class SawyerStreamWriter
	{
		public static void Save(string filepath, ILocoObject locoObject, ILogger? logger = null)
		{
			ArgumentNullException.ThrowIfNull(locoObject);

			logger?.Info($"Writing \"{locoObject.S5Header.Name}\" to {filepath}");

			var objBytes = WriteLocoObject(locoObject);

			var stream = File.Create(filepath);
			stream.Write(objBytes);
			stream.Flush();
			stream.Close();
		}

		public static ReadOnlySpan<byte> WriteLocoObject(ILocoObject obj)
		{
			var ms = new MemoryStream();

			ms.Write(obj.S5Header.Write());
			ms.Write((obj.ObjectHeader with { Encoding = SawyerEncoding.Uncompressed }).Write());

			var objBytes = ByteWriter.WriteLocoStruct(obj.Object);
			ms.Write(objBytes);

			// string table
			foreach (var ste in obj.StringTable.table)
			{
				foreach (var language in ste.Value)
				{
					ms.WriteByte((byte)language.Key);

					var strBytes = Encoding.ASCII.GetBytes(language.Value);
					ms.Write(strBytes, 0, strBytes.Length);
					ms.WriteByte((byte)'\0');
				}

				ms.WriteByte(0xff);
			}

			// variable data
			if (obj.Object is ILocoStructVariableData objV)
			{
				var variableBytes = objV.Save();
				ms.Write(variableBytes);
			}

			// graphics data
			if (obj.G1Header != null && obj.G1Elements != null && obj.G1Header.NumEntries != 0 && obj.G1Elements.Count != 0)
			{
				// write G1Header
				ms.Write(BitConverter.GetBytes(obj.G1Header.NumEntries));
				ms.Write(BitConverter.GetBytes(obj.G1Elements.Sum(x => G1Element32.StructLength + x.ImageData.Length)));

				var idx = 0;
				// write G1Elements
				foreach (var g1Element in obj.G1Elements)
				{
					// we need to update the offsets of the image data
					// and we're not going to compress the data on save, so make sure the RLECompressed flag is not set
					var offset = idx < 1 ? 0 : obj.G1Elements[idx - 1].Offset + (uint)obj.G1Elements[idx - 1].ImageData.Length;
					var newElement = g1Element with
					{
						Offset = offset,
						Flags = g1Element.Flags & ~G1ElementFlags.IsRLECompressed
					};
					ms.Write(newElement.Write());
					idx++;
				}

				// write G1Elements ImageData
				foreach (var g1Element in obj.G1Elements)
				{
					// we're not going to compress the data on save, so make sure the RLECompressed flag is not set
					ms.Write(g1Element.ImageData);
				}
			}

			// calculate size and write the size in obj header offset 18
			//var length = ms.Position - 21;
			//ms.Position = 17; // this is the offset of the length unit32_t in the whole object
			//ms.Write(BitConverter.GetBytes(length), 0, 4);

			ms.Flush();
			ms.Close();

			return ms.ToArray();
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
