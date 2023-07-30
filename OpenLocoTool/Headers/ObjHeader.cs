using System.ComponentModel;

namespace OpenLocoTool
{
	// size = 0x5
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record ObjHeader(SawyerEncoding Encoding, uint32_t Length)
	{
		public static ObjHeader Read(ReadOnlySpan<byte> data)
		{
			var encoding = (SawyerEncoding)data[0];
			var length = BitConverter.ToUInt32(data[1..5]);
			return new ObjHeader(encoding, length);
		}
	}
}
