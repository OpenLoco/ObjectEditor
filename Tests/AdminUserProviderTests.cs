using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using ObjectService.Identity;

namespace Tests;

/// <summary>
/// Verifies <see cref="AdminUserProvider"/> resolves a configured password in any environment, and
/// falls back to a generated throwaway password in Development only.
/// </summary>
[TestFixture]
public class AdminUserProviderTests
{
	sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
	{
		public string EnvironmentName { get; set; } = environmentName;
		public string ApplicationName { get; set; } = "Tests";
		public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
		public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
	}

	static IConfiguration BuildConfig(params (string Key, string? Value)[] values)
		=> new ConfigurationBuilder()
			.AddInMemoryCollection(values.Select(v => new KeyValuePair<string, string?>(v.Key, v.Value)))
			.Build();

	[Test]
	public void ConfiguredPassword_IsUsedInAnyEnvironment()
	{
		var provider = new AdminUserProvider(
			BuildConfig(("AdminUser:Password", "Configured1!")),
			new FakeHostEnvironment(Environments.Production));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(provider.Settings, Is.Not.Null);
			Assert.That(provider.Settings!.Password, Is.EqualTo("Configured1!"));
			Assert.That(provider.UsesGeneratedPassword, Is.False);
		}
	}

	[Test]
	public void MissingPassword_InDevelopment_GeneratesThrowawayPassword()
	{
		var provider = new AdminUserProvider(BuildConfig(), new FakeHostEnvironment(Environments.Development));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(provider.Settings, Is.Not.Null);
			Assert.That(provider.Settings!.Email, Is.EqualTo(AdminUserSettings.DefaultEmail));
			Assert.That(provider.Settings.UserName, Is.EqualTo(AdminUserSettings.DefaultUserName));
			Assert.That(provider.UsesGeneratedPassword, Is.True);
			Assert.That(provider.Settings.Password, Is.Not.Empty);
		}
	}

	[Test]
	public void MissingPassword_OutsideDevelopment_ReturnsNull()
	{
		var provider = new AdminUserProvider(BuildConfig(), new FakeHostEnvironment(Environments.Production));

		using (Assert.EnterMultipleScope())
		{
			Assert.That(provider.Settings, Is.Null);
			Assert.That(provider.UsesGeneratedPassword, Is.False);
		}
	}
}
