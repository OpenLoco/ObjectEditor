using OpenLocoToolCommon;

namespace OpenLocoTool.DatFileParsing
{
	public class SawyerStreamWriter(ILogger logger)
	{
		public void Save(string filepath, ILocoObject locoObject)
		{
			ArgumentNullException.ThrowIfNull(locoObject);

			logger.Info($"Writing \"{locoObject.S5Header.Name}\" to {filepath}");

			var objBytes = WriteLocoObject(locoObject);

			// hardcode uncompressed as encoding is currently not working
			var encoded = Encode(SawyerEncoding.Uncompressed, objBytes);
			//var encoded = Encode(locoObject.ObjectHeader.Encoding, objBytes);

			WriteToFile(filepath, locoObject.S5Header.Write(), locoObject.ObjectHeader.Write(), encoded);
		}

		public static ReadOnlySpan<byte> WriteLocoObject(ILocoObject obj)
		{
			var objBytes = ByteWriter.WriteLocoStruct(obj.Object);
			var ms = new MemoryStream();
			ms.Write(objBytes);

			//var stringBytes = Bytes(obj.StringTable);
			//ms.Write(stringBytes);

			if (obj.Object is ILocoStructVariableData objV)
			{
				//var variableBytes = objV.Save();
				//ms.Write(variableBytes);
			}

			if (obj.G1Header.NumEntries != 0 && obj.G1Elements.Count != 0)
			{
				//var g1Bytes = Bytes(obj.G1Header);
				//ms.Write(g1Bytes);

				//var g1ElementsBytes = Bytes(obj.G1Elements);
				//ms.Write(g1ElementsBytes);
			}

			ms.Flush();
			ms.Close();

			return ms.ToArray();
		}

		public ReadOnlySpan<byte> Encode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		{
			switch (encoding)
			{
				case SawyerEncoding.Uncompressed:
					return data;
				case SawyerEncoding.RunLengthSingle:
					return EncodeRunLengthSingle(data);
				//case SawyerEncoding.runLengthMulti:
				//	return encodeRunLengthMulti(decodeRunLengthSingle(data));
				//case SawyerEncoding.rotate:
				//	return encodeRotate(data);
				default:
					logger.Error("Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
		}

		public static void WriteToFile(string filepath, ReadOnlySpan<byte> s5Header, ReadOnlySpan<byte> objectHeader, ReadOnlySpan<byte> encodedData)
		{
			var stream = File.Create(filepath);
			stream.Write(s5Header);
			stream.Write(objectHeader);
			stream.Write(encodedData);
			stream.Flush();
			stream.Close();
		}

		// taken from openloco SawyerStreamReader::encodeRunLengthSingle
		// not sure why it doesn't work, but it doesn't work. gets the first 10 or so bytes correct for SIGC3.dat but then fails
		private static Span<byte> EncodeRunLengthSingle(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = [];
			var src = 0; // ptr
			var srcNormStart = 0; // ptr
			var srcEnd = data.Length;
			var count = 0;

			while (src < srcEnd - 1)
			{
				if ((count != 0 && data[src] == data[src + 1]) || count > 125)
				{
					buffer.Add((byte)(count - 1));
					buffer.AddRange(Enumerable.Repeat(data[srcNormStart], count));
					srcNormStart += count;
					count = 0;
				}

				if (data[src] == data[src + 1])
				{
					for (; count < 125 && src + count < srcEnd; count++)
					{
						if (data[src] != data[count])
						{
							break;
						}
					}

					buffer.Add((byte)(257 - count));
					buffer.Add(data[src]);
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

			if (data[src] == data[srcEnd - 1])
			{
				count++;
			}

			if (count != 0)
			{
				buffer.Add((byte)(count - 1));
				buffer.AddRange(Enumerable.Repeat(data[srcNormStart], count));
			}

			// convert to span
			Span<byte> encodedSpan = new byte[buffer.Count];
			var counter = 0;
			foreach (var b in buffer)
			{
				encodedSpan[counter++] = b;
			}

			return encodedSpan;
		}
	}
}
