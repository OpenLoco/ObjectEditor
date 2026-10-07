using Definitions.Database;
using Definitions.DTO.Identity;
using NUnit.Framework;
using ObjectService.Identity;
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
	public async Task Login_AsSystemAdminByUsername_ShouldSucceed()
	{
		// The Identity /login endpoint authenticates by user name (the request field is confusingly
		// named "Email"). The system admin's user name is the configured AdminUser:Username, which
		// defaults to "LeftofZen" and is what the dev quick-login uses.
		var loginRequest = new { Email = "LeftofZen", Password = "TestAdminPassword123!@#" };

		var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", loginRequest);

		Assert.That(response.IsSuccessStatusCode, Is.True);
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

		// act - PUT /v2/identity/manage/profile updates the signed-in user's display name
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

	[Test]
	public async Task OpenApi_SelfServiceAccountRoutes_AreGroupedUnderTheIdentityTag()
	{
		// The self-service account routes live under /v2/identity/manage (not /v2/users, which is
		// database administration). They must be grouped under the "Identity" tag.
		var json = await HttpClient!.GetStringAsync("/openapi/v1.json");
		using var document = System.Text.Json.JsonDocument.Parse(json);
		var paths = document.RootElement.GetProperty("paths");

		using (Assert.EnterMultipleScope())
		{
			foreach (var (path, method) in new[]
			{
				("/v2/identity/manage/profile", "put"),
				("/v2/identity/manage/account", "delete"),
			})
			{
				Assert.That(paths.TryGetProperty(path, out var pathItem), Is.True, $"expected {path} in the OpenAPI document");
				Assert.That(pathItem.TryGetProperty(method, out var operation), Is.True, $"expected a {method} operation on {path}");
				Assert.That(operation.TryGetProperty("tags", out var tags), Is.True, $"expected the {method} operation on {path} to be tagged");
				Assert.That(tags.EnumerateArray().Select(t => t.GetString()), Does.Contain("Identity"));
			}
		}
	}

	[Test]
	public async Task OpenApi_DeclaresBearerSchemeAndMarksOnlyAuthorizedOperations()
	{
		var json = await HttpClient!.GetStringAsync("/openapi/v1.json");
		using var document = System.Text.Json.JsonDocument.Parse(json);

		// The document must declare the "Bearer" scheme that Scalar's AddPreferredSecuritySchemes uses.
		var components = document.RootElement.GetProperty("components");
		var securitySchemes = components.GetProperty("securitySchemes");
		Assert.That(securitySchemes.TryGetProperty("Bearer", out var bearer), Is.True, "expected a 'Bearer' security scheme");
		Assert.That(bearer.GetProperty("scheme").GetString(), Is.EqualTo("bearer"));

		var paths = document.RootElement.GetProperty("paths");

		using (Assert.EnterMultipleScope())
		{
			// /v2/identity/manage/profile is mapped with RequireAuthorization, so it must advertise the scheme.
			var usersMePut = paths.GetProperty("/v2/identity/manage/profile").GetProperty("put");
			Assert.That(HasBearerSecurity(usersMePut), Is.True);
			Assert.That(usersMePut.GetProperty("security").GetArrayLength(), Is.EqualTo(1), "the security requirement must not be duplicated");

			// GET /v2/objects is a public read, so it must not.
			Assert.That(HasBearerSecurity(paths.GetProperty("/v2/objects").GetProperty("get")), Is.False);
		}
	}

	static bool HasBearerSecurity(System.Text.Json.JsonElement operation)
	{
		if (!operation.TryGetProperty("security", out var security)
			|| security.ValueKind != System.Text.Json.JsonValueKind.Array)
		{
			return false;
		}

		return security
			.EnumerateArray()
			.Any(requirement => requirement.EnumerateObject().Any(property => property.Name == BearerSecuritySchemeTransformer.SchemeName));
	}

	[Test]
	public async Task OpenApi_IdentityRoutes_AreGroupedUnderTheIdentityTag()
	{
		var json = await HttpClient!.GetStringAsync("/openapi/v1.json");
		using var document = System.Text.Json.JsonDocument.Parse(json);

		var register = document.RootElement.GetProperty("paths").GetProperty("/v2/identity/register").GetProperty("post");
		Assert.That(register.TryGetProperty("tags", out var tags), Is.True);
		Assert.That(tags.EnumerateArray().Select(t => t.GetString()), Does.Contain("Identity"));
	}

	[Test]
	public async Task Register_UsesSuppliedUserName()
	{
		// The framework's register endpoint ignores UserName and uses the email; ours must honour it.
		var registerRequest = new DtoRegisterRequest("named@example.com", "chosenname", "TestPassword123!");
		var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);
		Assert.That(response.IsSuccessStatusCode, Is.True, await response.Content.ReadAsStringAsync());

		// the account must be usable for login (login is by email)
		var loginResponse = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", new { Email = "named@example.com", Password = "TestPassword123!" });
		Assert.That(loginResponse.IsSuccessStatusCode, Is.True);

		var loginResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
		var accessToken = loginResult.GetProperty("accessToken").GetString();
		HttpClient!.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

		var users = await Definitions.Web.Client.GetUsersAsync(HttpClient!);
		var created = users.Single(u => u.Email == "named@example.com");
		Assert.That(created.UserName, Is.EqualTo("chosenname"));
	}

	[Test]
	public async Task AdminCanToggleEmailConfirmedAndForcePasswordResetForAnotherUser()
	{
		// arrange - a normal user to administer
		var registerRequest = new DtoRegisterRequest("adminops@example.com", "adminopsuser", "TestPassword123!");
		var registerResponse = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityRegister}", registerRequest);
		Assert.That(registerResponse.IsSuccessStatusCode, Is.True);

		// sign in as the system admin. The AdminOnly policy deliberately excludes the dev-bypass scheme,
		// so the identity endpoints require a real admin login.
		var adminLogin = new { Email = "LeftofZen", Password = "TestAdminPassword123!@#" };
		var loginResponse = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.IdentityLogin}?useCookies=false", adminLogin);
		Assert.That(loginResponse.IsSuccessStatusCode, Is.True);

		var loginResult = await loginResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
		var accessToken = loginResult.GetProperty("accessToken").GetString();
		HttpClient!.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

		// Identity's built-in register endpoint uses the email as the user name.
		var users = (await Definitions.Web.Client.GetUsersAsync(HttpClient!)).ToList();
		var target = users.SingleOrDefault(u => u.Email == "adminops@example.com");
		Assert.That(target, Is.Not.Null, $"registered user not found. Users: {string.Join(", ", users.Select(u => u.Email))}");

		// act + assert - email confirmation toggles both ways
		var confirmed = await Definitions.Web.Client.ToggleUserEmailConfirmedAsync(HttpClient!, target.Id);
		Assert.That(confirmed, Is.Not.Null);
		Assert.That(confirmed!.EmailConfirmed, Is.True);

		var revoked = await Definitions.Web.Client.ToggleUserEmailConfirmedAsync(HttpClient!, target.Id);
		Assert.That(revoked, Is.Not.Null);
		Assert.That(revoked!.EmailConfirmed, Is.False);

		// act + assert - an admin can mint a password-reset token for another user
		var reset = await Definitions.Web.Client.ForceUserPasswordResetAsync(HttpClient!, target.Id);
		Assert.That(reset, Is.Not.Null);
		Assert.That(reset!.Token, Is.Not.Null.And.Not.Empty);
	}
}
