using System.Numerics;

namespace OpenLoco.ObjectEditor.DatFileParsing
{
	public static class SawyerStreamUtils
	{
		public static uint ComputeObjectChecksum(ReadOnlySpan<byte> headerFlagByte, ReadOnlySpan<byte> name, ReadOnlySpan<byte> data)
		{
			static uint32_t ComputeChecksum(ReadOnlySpan<byte> data, uint32_t seed)
			{
				var checksum = seed;
				foreach (var d in data)
				{
					checksum = BitOperations.RotateLeft(checksum ^ d, 11);
				}

				return checksum;
			}

			const uint32_t objectChecksumMagic = 0xF369A75B;
			var checksum = ComputeChecksum(headerFlagByte, objectChecksumMagic);
			checksum = ComputeChecksum(name, checksum);
			checksum = ComputeChecksum(data, checksum);
			return checksum;
		}
	}
}
