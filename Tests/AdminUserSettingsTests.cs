using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ObjectService.Identity;

namespace Tests;

/// <summary>
/// Verifies the system admin password is never defaulted in code: without an explicit
/// <c>AdminUser:Password</c> the admin is not bootstrapped, and the email/username fall back to the
/// built-in display defaults.
/// </summary>
[TestFixture]
public class AdminUserSettingsTests
{
	static IConfiguration BuildConfig(params (string Key, string? Value)[] values)
		=> new ConfigurationBuilder()
			.AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>(v.Key, v.Value)))
			.Build();

	[Test]
	public void FromConfiguration_ReturnsNullWhenPasswordIsMissing()
	{
		var settings = AdminUserSettings.FromConfiguration(BuildConfig(("AdminUser:Email", "admin@example.com")));

		Assert.That(settings, Is.Null);
	}

	[TestCase("")]
	[TestCase("   ")]
	public void FromConfiguration_TreatsBlankPasswordAsMissing(string password)
	{
		Assert.That(AdminUserSettings.FromConfiguration(BuildConfig(("AdminUser:Password", password))), Is.Null);
	}

	[Test]
	public void FromConfiguration_UsesConfiguredValues()
	{
		var settings = AdminUserSettings.FromConfiguration(BuildConfig(
			("AdminUser:Email", "admin@example.com"),
			("AdminUser:Username", "ConfiguredAdmin"),
			("AdminUser:Password", "s3cret!")));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(settings, Is.Not.Null);
			Assert.That(settings!.Email, Is.EqualTo("admin@example.com"));
			Assert.That(settings.UserName, Is.EqualTo("ConfiguredAdmin"));
			Assert.That(settings.Password, Is.EqualTo("s3cret!"));
		}
	}

	[Test]
	public void FromConfiguration_FallsBackToDefaultIdentityWhenOnlyThePasswordIsSet()
	{
		var settings = AdminUserSettings.FromConfiguration(BuildConfig(("AdminUser:Password", "s3cret!")));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(settings, Is.Not.Null);
			Assert.That(settings!.Email, Is.EqualTo(AdminUserSettings.DefaultEmail));
			Assert.That(settings.UserName, Is.EqualTo(AdminUserSettings.DefaultUserName));
		}
	}

	[Test]
	public void CreateDevelopmentFallback_ProducesAPasswordThatMeetsTheIdentityPolicy()
	{
		var settings = AdminUserSettings.CreateDevelopmentFallback();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(settings.Email, Is.EqualTo(AdminUserSettings.DefaultEmail));
			Assert.That(settings.UserName, Is.EqualTo(AdminUserSettings.DefaultUserName));
			Assert.That(settings.Password.Length, Is.GreaterThanOrEqualTo(12));
			Assert.That(settings.Password.Any(char.IsDigit), Is.True);
			Assert.That(settings.Password.Any(char.IsLower), Is.True);
			Assert.That(settings.Password.Any(char.IsUpper), Is.True);
			Assert.That(settings.Password.Any(c => !char.IsLetterOrDigit(c)), Is.True);
		}
	}
}