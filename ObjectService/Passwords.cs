using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace OpenLoco.ObjectService
{
	public static class Passwords
	{
		static byte[] GenerateSalt(int sizeInBytes)
		{
			using (var rng = RandomNumberGenerator.Create())
			{
				var salt = new byte[sizeInBytes];
				rng.GetBytes(salt);
				return salt;
			}
		}

		public static (string Hash, string Salt) HashPassword(string password)
		{
			// Generate a random salt
			var salt = GenerateSalt(64);

			// Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
			var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100000,
				numBytesRequested: 256 / 8));

			return (hashed, Convert.ToBase64String(salt));
		}

		public static bool VerifyPassword(string password, string hashedPassword, string salt)
		{
			// Re-hash the provided password with the stored salt
			var newHash = HashPassword(password, Convert.FromBase64String(salt)).Hash;

			// Compare the re-hashed password with the stored hash
			return newHash == hashedPassword;
		}

		private static (string Hash, string Salt) HashPassword(string password, byte[] salt)
		{
			// Similar to the public HashPassword method, but takes a byte[] salt
			var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 100000,
				numBytesRequested: 256 / 8));

			return (hashed, Convert.ToBase64String(salt));

		}
	}
}
