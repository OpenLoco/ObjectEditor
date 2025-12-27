using Definitions.Database;
using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class IdentityRoutesTest : BaseRouteHandlerTestFixture
{
	public override string BaseRoute => string.Empty;

	protected override Task SeedDataCoreAsync(LocoDbContext db)
	{
		// No seed data needed for identity tests
		return Task.CompletedTask;
	}

	[Test]
	public override async Task ListAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	public override async Task PostAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	public override async Task GetAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	public override async Task PutAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	public override async Task DeleteAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	public async Task Register_ShouldSucceed()
	{
		// Arrange
		var registerRequest = new DtoRegisterRequest(
			Email: "test@example.com",
			UserName: "testuser",
			Password: "TestPassword123!"
		);

		// Act
		var response = await HttpClient!.PostAsJsonAsync("/register", registerRequest);

		// Assert
		Assert.That(response.IsSuccessStatusCode, Is.True);
	}

	[Test]
	public async Task Register_WithInvalidEmail_ShouldFail()
	{
		// Arrange
		var registerRequest = new DtoRegisterRequest(
			Email: "invalid-email",
			UserName: "testuser",
			Password: "TestPassword123!"
		);

		// Act
		var response = await HttpClient!.PostAsJsonAsync("/register", registerRequest);

		// Assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task Login_WithValidCredentials_ShouldSucceed()
	{
		// Arrange - First register a user
		var registerRequest = new DtoRegisterRequest(
			Email: "login@example.com",
			UserName: "loginuser",
			Password: "TestPassword123!"
		);
		await HttpClient!.PostAsJsonAsync("/register", registerRequest);

		var loginRequest = new
		{
			Email = "login@example.com",
			Password = "TestPassword123!"
		};

		// Act
		var response = await HttpClient!.PostAsJsonAsync("/login?useCookies=false", loginRequest);

		// Assert
		Assert.That(response.IsSuccessStatusCode, Is.True);
		var result = await response.Content.ReadAsStringAsync();
		Assert.That(result, Is.Not.Empty);
	}

	[Test]
	public async Task Login_WithInvalidCredentials_ShouldFail()
	{
		// Arrange
		var loginRequest = new
		{
			Email = "nonexistent@example.com",
			Password = "WrongPassword123!"
		};

		// Act
		var response = await HttpClient!.PostAsJsonAsync("/login?useCookies=false", loginRequest);

		// Assert
		Assert.That(response.IsSuccessStatusCode, Is.False);
	}

	[Test]
	public async Task Users_WithoutAuthentication_ShouldReturnUnauthorized()
	{
		// Act
		var response = await HttpClient!.GetAsync($"{RoutesV2.Prefix}{RoutesV2.Users}");

		// Assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task Roles_WithoutAuthentication_ShouldReturnUnauthorized()
	{
		// Act
		var response = await HttpClient!.GetAsync($"{RoutesV2.Prefix}{RoutesV2.Roles}");

		// Assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}
}
