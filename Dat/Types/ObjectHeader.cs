using Dat.Data;
using Dat.FileParsing;
using System.ComponentModel;

namespace Dat.Types;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x05)]
public class ObjectHeader(SawyerEncoding encoding, uint dataLength)
{
	public SawyerEncoding Encoding { get; set; } = encoding;
	public uint32_t DataLength { get; set; } = dataLength;

	public const int StructLength = 0x05;

	public bool IsValid()
		=> (int)Encoding is >= 0 and < Limits.kMaxSawyerEncodings;

	public static ObjectHeader Read(ReadOnlySpan<byte> data)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(data.Length, StructLength);

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

	public static readonly ObjectHeader NullHeader = new(SawyerEncoding.Uncompressed, 0);
}
