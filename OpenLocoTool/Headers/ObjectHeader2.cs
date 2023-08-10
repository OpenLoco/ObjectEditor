using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Headers
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record ObjectHeader2(SawyerEncoding Encoding, uint32_t Length) : ILocoStruct
	{
		//public ObjectType ObjectType => throw new NotImplementedException();

		public static int ObjectStructSize => 0x5;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var encoding = (SawyerEncoding)data[0];
			var length = BitConverter.ToUInt32(data[1..5]);
			return new ObjectHeader2(encoding, length);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[ObjectStructSize];

			var checksum = BitConverter.GetBytes(Length);

			span[0] = (byte)Encoding;
			checksum.CopyTo(span, 1);

			return span;
		}

	}
}
