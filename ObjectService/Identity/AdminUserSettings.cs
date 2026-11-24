using System.Security.Cryptography;

namespace ObjectService.Identity;

/// <summary>
/// The system admin account bootstrapped by <see cref="DatabaseInitializer"/>. Use
/// <see cref="AdminUserProvider"/> to resolve the account for the process.
/// <para>
/// A configured password is deliberately never committed to source control and never defaulted in
/// code: <see cref="FromConfiguration"/> returns <see langword="null"/> when <c>AdminUser:Password</c>
/// is not set, so deployments must supply it through user-secrets, environment variables or
/// configuration. The one exception is the Development environment, where
/// <see cref="CreateDevelopmentFallback"/> mints a random throwaway password so the admin exists and
/// the Dev Login button works with no setup (see <see cref="AdminUserProvider"/>).
/// </para>
/// </summary>
/// <param name="Email">Admin email address.</param>
/// <param name="UserName">Admin user name.</param>
/// <param name="Password">Admin password (never a committed code default).</param>
public sealed record AdminUserSettings(string Email, string UserName, string Password)
{
	public const string DefaultEmail = "leftofzen@openloco.io";
	public const string DefaultUserName = "LeftofZen";

	/// <summary>
	/// Reads the admin settings from configuration. Returns <see langword="null"/> when no password is
	/// configured, so callers can skip bootstrapping the admin rather than falling back to a default.
	/// </summary>
	public static AdminUserSettings? FromConfiguration(IConfiguration config)
	{
		var password = config["AdminUser:Password"];
		if (string.IsNullOrWhiteSpace(password))
		{
			return null;
		}

		return new AdminUserSettings(
			config["AdminUser:Email"] ?? DefaultEmail,
			config["AdminUser:Username"] ?? DefaultUserName,
			password);
	}

	/// <summary>
	/// Creates a Development-only admin whose password is a cryptographically-random throwaway. The
	/// value is never logged, persisted or returned to a client — it exists only so the admin can be
	/// bootstrapped and the dev quick-login can complete a normal Identity login round-trip without the
	/// developer supplying credentials.
	/// </summary>
	public static AdminUserSettings CreateDevelopmentFallback()
		=> new(DefaultEmail, DefaultUserName, GenerateThrowawayPassword());

	/// <summary>
	/// Generates a random password that always satisfies the configured Identity password policy
	/// (at least one digit, lower-case letter, upper-case letter and non-alphanumeric character).
	/// </summary>
	static string GenerateThrowawayPassword()
	{
		Span<byte> bytes = stackalloc byte[24];
		RandomNumberGenerator.Fill(bytes);

		// A fixed prefix guarantees one of each required character class while the random Base64
		// suffix supplies the entropy. Identity requires a non-alphanumeric character, not a
		// URL-safe one, so the standard alphabet is fine.
		return $"Aa1!{Convert.ToBase64String(bytes)}";
	}
}