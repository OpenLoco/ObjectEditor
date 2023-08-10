using OpenLocoToolCommon;

namespace OpenLocoTool.DatFileParsing
{
	public class SawyerStreamWriter
	{
		private readonly ILogger Logger;

		public SawyerStreamWriter(ILogger logger)
			=> Logger = logger;

		public void Save(string filepath, ILocoObject locoObject)
		{
			if (locoObject == null)
			{
				throw new ArgumentNullException(nameof(locoObject));
			}

			Logger.Log(LogLevel.Info, $"[NOT IMPLEMENTED] Writing \"{locoObject.ObjectHeader.Name}\" to {filepath}");

			//var encoded = Encode(locoObject.ObjectHeader2.Encoding, locoObject.Object.Write());
			//WriteToFile(filepath, locoObject.ObjectHeader.Write(), locoObject.ObjectHeader2.Write(), encoded);
		}

		public ReadOnlySpan<byte> Encode(SawyerEncoding encoding, ReadOnlySpan<byte> data)
		{
			switch (encoding)
			{
				case SawyerEncoding.uncompressed:
					return data;
				case SawyerEncoding.runLengthSingle:
					return encodeRunLengthSingle(data);
				//case SawyerEncoding.runLengthMulti:
				//	return encodeRunLengthMulti(decodeRunLengthSingle(data));
				//case SawyerEncoding.rotate:
				//	return encodeRotate(data);
				default:
					Logger.Log(LogLevel.Error, "Unknown chunk encoding scheme");
					throw new InvalidDataException("Unknown encoding");
			}
		}

		public void WriteToFile(string filepath, ReadOnlySpan<byte> objectHeader1, ReadOnlySpan<byte> objectHeader2, ReadOnlySpan<byte> data)
		{
			var stream = File.Create(filepath);
			stream.Write(objectHeader1);
			stream.Write(objectHeader2);
			stream.Write(data);
			stream.Flush();
			stream.Close();
		}

		// taken from openloco SawyerStreamReader::encodeRunLengthSingle
		private Span<byte> encodeRunLengthSingle(ReadOnlySpan<byte> data)
		{
			List<byte> buffer = new();
			var src = 0; // ptr
			var srcNormStart = 0; // ptr
			var srcEnd = data.Length;
			var count = 0;

			for (var i = 0; i < data.Length; ++i)
			{
				if (count != 0 && data[src] == data[src + 1] || count > 125)
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
						if (data[src] == data[count])
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
