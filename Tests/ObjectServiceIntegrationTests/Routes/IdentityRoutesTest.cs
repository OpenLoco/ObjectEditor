using Definitions.Database;
using Definitions.DTO.Identity;
using NUnit.Framework;
using ObjectService.Tests.Integration;
using System.Net;
using System.Net.Http.Json;

namespace Tests.ObjectServiceIntegrationTests.Routes;

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
	[Ignore("Not applicable for identity endpoints")]
	public override async Task ListAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	[Ignore("Not applicable for identity endpoints")]
	public override async Task PostAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	[Ignore("Not applicable for identity endpoints")]
	public override async Task GetAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	[Ignore("Not applicable for identity endpoints")]
	public override async Task PutAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	[Ignore("Not applicable for identity endpoints")]
	public override async Task DeleteAsync()
	{
		// Not applicable for identity endpoints
		await Task.CompletedTask;
	}

	[Test]
	public async Task Register_ShouldSucceed()
	{
		// arrange
		var registerRequest = new DtoRegisterRequest(
			Email: "test@example.com",
			UserName: "testuser",
			Password: "TestPassword123!"
		);

		// act
		var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);

		// assert
		Assert.That(response.IsSuccessStatusCode, Is.True);
	}

	[Test]
	public async Task Register_WithInvalidEmail_ShouldFail()
	{
		// arrange
		var registerRequest = new DtoRegisterRequest(
			Email: "invalid-email",
			UserName: "testuser",
			Password: "TestPassword123!"
		);

		// act
		var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);

		// assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task Login_WithValidCredentials_ShouldSucceed()
	{
		// arrange - First register a user
		var registerRequest = new DtoRegisterRequest(
			Email: "login@example.com",
			UserName: "loginuser",
			Password: "TestPassword123!"
		);
		_ = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);

		var loginRequest = new
		{
			Email = "login@example.com",
			Password = "TestPassword123!"
		};

		// act
		var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", loginRequest);

		// assert
		Assert.That(response.IsSuccessStatusCode, Is.True);
		var result = await response.Content.ReadAsStringAsync();
		Assert.That(result, Is.Not.Empty);
	}

	[Test]
	public async Task Login_WithInvalidCredentials_ShouldFail()
	{
		// arrange
		var loginRequest = new
		{
			Email = "nonexistent@example.com",
			Password = "WrongPassword123!"
		};

		// act
		var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", loginRequest);

		// assert
		Assert.That(response.IsSuccessStatusCode, Is.False);
	}

	[Test]
	public async Task Users_WithoutAuthentication_ShouldReturnUnauthorized()
	{
		// act
		var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Users}");

		// assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task Roles_WithoutAuthentication_ShouldReturnUnauthorized()
	{
		// act
		var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Roles}");

		// assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
	}

	[Test]
	public async Task Users_WithAuthentication_ShouldSucceed()
	{
		// arrange - Register a user
		var registerRequest = new DtoRegisterRequest(
			Email: "authtest@example.com",
			UserName: "authuser",
			Password: "TestPassword123!"
		);
		_ = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);

		// Login to get bearer token
		var loginRequest = new
		{
			Email = "authtest@example.com",
			Password = "TestPassword123!"
		};
		var loginResponse = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", loginRequest);
		Assert.That(loginResponse.IsSuccessStatusCode, Is.True);

		var loginResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
		var accessToken = loginResult.GetProperty("accessToken").GetString();
		Assert.That(accessToken, Is.Not.Null.And.Not.Empty);

		// Add token to Authorization header
		HttpClient!.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

		// act - Call protected endpoint with valid bearer token
		var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Users}");

		// assert - Should succeed with valid authentication
		Assert.That(response.IsSuccessStatusCode, Is.True);
	}
[Test]
	public async Task UpdateCurrentUserDisplayName_WithAuthentication_ShouldSucceed()
	{
		// arrange - register and sign in
		var registerRequest = new DtoRegisterRequest(
			Email: "medisplay@example.com",
			UserName: "medisplayuser",
			Password: "TestPassword123!");
		_ = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);

		var loginRequest = new { Email = "medisplay@example.com", Password = "TestPassword123!" };
		var loginResponse = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", loginRequest);
		Assert.That(loginResponse.IsSuccessStatusCode, Is.True);

		var loginResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
		var accessToken = loginResult.GetProperty("accessToken").GetString();
		HttpClient!.DefaultRequestHeaders.Authorization =
			new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

		// act - PUT /v2/users/me is registered from the write path (WS14)
		var updated = await Definitions.Web.Client.SetCurrentUserDisplayNameAsync(HttpClient!, "RenamedDisplayUser");

		// assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(updated, Is.Not.Null);
			Assert.That(updated!.UserName, Is.EqualTo("RenamedDisplayUser"));

			// /v2/identity/manage/info is reachable (WS8). Note: the framework's InfoResponse only
			// returns email/confirmation, so the DTO deliberately has no username field.
			var info = await HttpClient!.GetFromJsonAsync<DtoInfoResponse>($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityManageInfo}");
			Assert.That(info, Is.Not.Null);
			Assert.That(info!.Email, Is.EqualTo("medisplay@example.com"));
		}
	}
}
