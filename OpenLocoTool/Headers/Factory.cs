using System.Text;

namespace OpenLocoTool
{
	public static class Factory
	{
		public static DatFileHeader MakeDatFileHeader(ReadOnlySpan<byte> data)
		{
			var flags = BitConverter.ToUInt32(data[0..4]);
			var name = Encoding.ASCII.GetString(data[4..12]);
			var checksum = BitConverter.ToUInt32(data[12..16]);
			return new DatFileHeader(flags, name, checksum);
		}

		public static ObjHeader MakeObjHeader(ReadOnlySpan<byte> data)
		{
			var encoding = (SawyerEncoding)(data[0]);
			var length = BitConverter.ToUInt32(data[1..5]);
			return new ObjHeader(encoding, length);
		}
	}
}
