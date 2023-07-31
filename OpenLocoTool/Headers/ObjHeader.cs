using System.ComponentModel;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record ObjHeader(SawyerEncoding Encoding, uint32_t Length) : ILocoSubObject
	{
		public int BinarySize => 0x5;

		public static ObjHeader Read(ReadOnlySpan<byte> data)
		{
			var encoding = (SawyerEncoding)data[0];
			var length = BitConverter.ToUInt32(data[1..5]);
			return new ObjHeader(encoding, length);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[BinarySize];

			var checksum = BitConverter.GetBytes(Length);

			span[0] = (byte)Encoding;
			checksum.CopyTo(span, 1);

			return span;
		}

	}
}
