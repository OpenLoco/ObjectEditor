using System.Text;
using OpenLocoTool.Objects;

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

		public static ClimateObject MakeClimateObject(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";
			var firstSeason = data[2];

			var seasonLength = new uint8_t[4];
			seasonLength[0] = data[3];
			seasonLength[1] = data[4];
			seasonLength[2] = data[5];
			seasonLength[3] = data[6];

			var winterSnowLine = data[7];
			var summerSnowLine = data[8];
			uint8_t pad_09 = 0;

			return new ClimateObject(name, firstSeason, seasonLength, winterSnowLine, summerSnowLine, pad_09);
		}
	}
}
