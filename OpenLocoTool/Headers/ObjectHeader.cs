using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using Zenith.Core;

namespace OpenLocoTool.Headers
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x05)]
	public record ObjectHeader(SawyerEncoding Encoding, uint32_t DataLength)
	{
		public static int StructLength => 0x05;

		public static ObjectHeader Read(ReadOnlySpan<byte> data)
		{
			Verify.Equals(data.Length, StructLength);

			var encoding = (SawyerEncoding)data[0];
			var dataLength = BitConverter.ToUInt32(data[1..5]);
			return new ObjectHeader(encoding, dataLength);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[StructLength];
			span[0] = (byte)Encoding;
			BitConverter.GetBytes(StructLength).CopyTo(span, 1);
			return span;
		}
	}
}
