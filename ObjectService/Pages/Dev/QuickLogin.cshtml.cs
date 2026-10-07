using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using ObjectService.Identity;

namespace ObjectService.Pages.Dev;

/// <summary>
/// Development-only convenience that signs the system admin in without the developer typing
/// credentials. It is the only UI authentication shortcut; the API has its own
/// (<see cref="DevAuthenticationHandler"/>) gated by <c>ObjectService:DisableAuthentication</c>.
/// </summary>
[AllowAnonymous]
public class QuickLoginModel : PageModel
{
	private readonly FrontendApiClient _api;
	private readonly IWebHostEnvironment _environment;
	private readonly AdminUserProvider _admin;
	private readonly ILogger<QuickLoginModel> _logger;

	public QuickLoginModel(FrontendApiClient api, IWebHostEnvironment environment, AdminUserProvider admin, ILogger<QuickLoginModel> logger)
	{
		_api = api;
		_environment = environment;
		_admin = admin;
		_logger = logger;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!_environment.IsDevelopment())
		{
			return Forbid();
		}

		// The system admin is resolved once for the process (see AdminUserProvider). The password comes
		// from configuration (user-secrets / environment) or, when none is set, from the throwaway
		// password generated at startup — either way no credential is committed or typed by the user.
		var admin = _admin.Settings;
		if (admin is null)
		{
			return BadRequest("Dev quick-login is not available: the system admin is not configured.");
		}

		using var client = _api.CreateClient();

		// The Identity /login endpoint resolves the supplied value as the USERNAME — it calls
		// PasswordSignInAsync(login.Email, ...), so the field is named "Email" but is matched against
		// UserName. Sign in with the admin's user name.
		var loginPayload = new DtoLoginRequest(admin.UserName, admin.Password);
		using var cookieResponse = await client.PostAsJsonAsync($"{Routes.Prefix}{Routes.IdentityLogin}?useCookies=true", loginPayload);
		if (!cookieResponse.IsSuccessStatusCode)
		{
			var body = await cookieResponse.Content.ReadAsStringAsync();
			_logger.LogWarning(
				"Dev quick-login failed to sign in {Email} ({StatusCode}): {Body}",
				admin.Email, (int)cookieResponse.StatusCode, body);
			return BadRequest("Failed to sign in dev user");
		}

		ForwardSetCookieHeaders(cookieResponse);
		await StoreBearerTokenAsync(client, loginPayload);

		// Redirect back to the referring page when it points at this host, otherwise to the account page
		// (the Referer header is an absolute URL, so it must be reduced to a local path first).
		var returnUrl = Request.Headers.Referer.ToString();
		if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var referer)
			&& string.Equals(referer.Authority, Request.Host.Value, StringComparison.OrdinalIgnoreCase))
		{
			returnUrl = referer.PathAndQuery;
		}

		if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
		{
			returnUrl = "/account/manage";
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
			using var response = await client.PostAsJsonAsync($"{Routes.Prefix}{Routes.IdentityLogin}?useCookies=false", payload);
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
