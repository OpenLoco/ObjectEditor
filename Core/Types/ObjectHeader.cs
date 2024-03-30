using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using Zenith.Core;

namespace OpenLoco.ObjectEditor.Headers
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x05)]
	public record ObjectHeader(SawyerEncoding Encoding, uint32_t DataLength)
	{
		public const int StructLength = 0x05;

		public static ObjectHeader Read(ReadOnlySpan<byte> data)
		{
			Verify.AreEqual(data.Length, StructLength);

			var encoding = (SawyerEncoding)data[0];
			var dataLength = BitConverter.ToUInt32(data[1..5]);
			return new ObjectHeader(encoding, dataLength);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[StructLength];
			span[0] = (byte)Encoding;

			BitConverter.GetBytes(DataLength).CopyTo(span, 1);

			return span;
		}
	}
}
