namespace ObjectService.Identity;

/// <summary>
/// Resolves the single system admin account for the lifetime of the process. It is the one source of
/// truth for the admin identity used by <see cref="DatabaseInitializer"/> at startup and by the
/// development-only quick-login page.
/// <para>
/// A configured <c>AdminUser:Password</c> (user-secrets or environment variable) always wins. In the
/// Development environment only, when none is configured, a random throwaway password is generated
/// once so the admin is still bootstrapped and the <b>Dev Login</b> button works with no setup.
/// Outside Development a missing password means no admin is bootstrapped, exactly as before.
/// </para>
/// </summary>
public sealed class AdminUserProvider
{
	/// <summary>The resolved admin settings, or <see langword="null"/> when the admin is not configured.</summary>
	public AdminUserSettings? Settings { get; }

	/// <summary>
	/// <see langword="true"/> when <see cref="Settings"/> uses a generated throwaway password rather
	/// than an explicitly configured one.
	/// </summary>
	public bool UsesGeneratedPassword { get; }

	public AdminUserProvider(IConfiguration configuration, IHostEnvironment environment)
	{
		Settings = AdminUserSettings.FromConfiguration(configuration);
		if (Settings is not null)
		{
			return;
		}

		if (environment.IsDevelopment())
		{
			Settings = AdminUserSettings.CreateDevelopmentFallback();
			UsesGeneratedPassword = true;
		}
	}
}
