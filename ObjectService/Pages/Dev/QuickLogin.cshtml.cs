using Definitions.DTO.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using System.Net.Http.Json;

namespace ObjectService.Pages.Dev;

[AllowAnonymous]
public class QuickLoginModel : PageModel
{
	private const string DevUserEmail = "dev@localhost";
	private const string DevPassword = "DevPassword123!@#";

	private readonly FrontendApiClient _api;
	private readonly IWebHostEnvironment _environment;
	private readonly ILogger<QuickLoginModel> _logger;

	public QuickLoginModel(FrontendApiClient api, IWebHostEnvironment environment, ILogger<QuickLoginModel> logger)
	{
		_api = api;
		_environment = environment;
		_logger = logger;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!_environment.IsDevelopment())
		{
			return Forbid();
		}

		using var client = _api.CreateClient();

		// Ensure the dev admin user/role exists (development-only API endpoint).
		using var bootstrapResponse = await client.PostAsync("/dev/quick-login", null);
		if (!bootstrapResponse.IsSuccessStatusCode)
		{
			return BadRequest("Failed to create dev user");
		}

		// Sign in via the Identity API.
		var loginPayload = new DtoLoginRequest(DevUserEmail, DevPassword);
		using var cookieResponse = await client.PostAsJsonAsync("/login?useCookies=true", loginPayload);
		if (!cookieResponse.IsSuccessStatusCode)
		{
			return BadRequest("Failed to sign in dev user");
		}

		ForwardSetCookieHeaders(cookieResponse);
		await StoreBearerTokenAsync(client, loginPayload);

		// Redirect back to the referring page, or to the admin dashboard.
		var returnUrl = Request.Headers.Referer.ToString();
		if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
		{
			returnUrl = "/manage";
		}

		return Redirect(returnUrl);
	}

	void ForwardSetCookieHeaders(HttpResponseMessage response)
	{
		if (response.Headers.TryGetValues("Set-Cookie", out var values))
		{
			foreach (var value in values)
			{
				Response.Headers.Append("Set-Cookie", value);
			}
		}
	}

	async Task StoreBearerTokenAsync(HttpClient client, DtoLoginRequest payload)
	{
		try
		{
			using var response = await client.PostAsJsonAsync("/login?useCookies=false", payload);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}

			var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
			if (tokenResponse?.AccessToken == null)
			{
				return;
			}

			Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = Request.IsHttps,
				SameSite = SameSiteMode.Lax,
				MaxAge = TimeSpan.FromHours(1),
			});
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to obtain bearer token for dev user");
		}
	}

	sealed record TokenResponse(string TokenType, string AccessToken, long ExpiresIn, string RefreshToken);
}
