using System.Text;
using System.Xml.Linq;
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
			var ms = new MemoryStream();

			// s5 header
			uint32_t checksum = 0; // todo: compute this
			var s5Header = new S5Header(objName, checksum);
			s5Header.SourceGame = SourceGame.Custom;
			var attr = AttributeHelper.Get<LocoStructTypeAttribute>(obj.Object.GetType());
			s5Header.ObjectType = attr!.ObjectType;

			ms.Write(s5Header.Write());

			// obj header
			ms.WriteByte((byte)SawyerEncoding.Uncompressed);
			uint32_t objectSize = 0; // todo: this is the size of the object in bytes, we need to calculate it
			ms.Write(BitConverter.GetBytes(objectSize));

			// obj
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
			if (obj.G1Elements != null && obj.G1Elements.Count != 0)
			{
				// write G1Header
				ms.Write(BitConverter.GetBytes(obj.G1Elements.Count));
				ms.Write(BitConverter.GetBytes(obj.G1Elements.Sum(x => G1Element32.StructLength + x.ImageData.Length)));

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

					ms.Write(newElement.Write());
					offsetBytesIntoImageData += g1Element.ImageData.Length;
				}

				// write G1Elements ImageData
				foreach (var g1Element in obj.G1Elements)
				{
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
