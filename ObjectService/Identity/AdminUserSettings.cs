namespace ObjectService.Identity;

/// <summary>
/// Configuration for the system admin account bootstrapped by <see cref="DatabaseInitializer"/>.
/// <para>
/// The password is deliberately never defaulted in code. <see cref="FromConfiguration"/> returns
/// <see langword="null"/> when <c>AdminUser:Password</c> is not configured, in which case the admin
/// account is not bootstrapped at all — deployments must supply it through user-secrets, environment
/// variables or configuration.
/// </para>
/// </summary>
/// <param name="Email">Admin email address.</param>
/// <param name="UserName">Admin user name.</param>
/// <param name="Password">Admin password (never a code default).</param>
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
}