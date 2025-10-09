using System.Numerics;

namespace Dat.FileParsing;

public static class SawyerStreamUtils
{
	public static uint ComputeObjectChecksum(ReadOnlySpan<byte> headerFlagByte, ReadOnlySpan<byte> name, ReadOnlySpan<byte> data)
	{
		if (name.Length != 8)
		{
			throw new ArgumentOutOfRangeException(nameof(name), "Name must be exactly 8 bytes long.");
		}

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
		var checksum = ComputeChecksum(headerFlagByte, objectChecksumMagic); // 1295935387
		checksum = ComputeChecksum(name, checksum); // 2991070967
		checksum = ComputeChecksum(data, checksum); // 1733551639
		return checksum;
	}

	// returns paths relative to the input directory
	public static IEnumerable<string> GetDatFilesInDirectory(string directory)
		=> Directory
			.GetFiles(directory, "*", SearchOption.AllDirectories)
			.Where(x => Path.GetExtension(x).Equals(".dat", StringComparison.OrdinalIgnoreCase))
			.Select(x => Path.GetRelativePath(directory, x));
}
