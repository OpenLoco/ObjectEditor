using Definitions.DTO.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using System.ComponentModel.DataAnnotations;

namespace ObjectService.Pages.Account;

public sealed class LoginModel : PageModel
{
	private readonly FrontendApiClient _api;
	private readonly ILogger<LoginModel> _logger;

	public LoginModel(FrontendApiClient api, ILogger<LoginModel> logger)
	{
		_api = api;
		_logger = logger;
	}

	[BindProperty]
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Please enter a valid email address")]
	public string Email { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Password is required")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	[BindProperty(SupportsGet = true)]
	public string? ReturnUrl { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		using var client = _api.CreateClient();
		var payload = new DtoLoginRequest(Email.Trim(), Password);

		// Sign in via the Identity API using cookies so the Razor frontend stays authenticated.
		using var cookieResponse = await client.PostAsJsonAsync("/login?useCookies=true", payload);
		if (!cookieResponse.IsSuccessStatusCode)
		{
			ModelState.AddModelError(string.Empty, "Invalid email or password.");
			return Page();
		}

		ForwardSetCookieHeaders(cookieResponse);

		// Also obtain a bearer token for the API calls the frontend makes on the user's behalf.
		_ = await StoreBearerTokenAsync(client, payload);

		_logger.LogInformation("User {Email} logged in", Email);

		if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
		{
			return Redirect(ReturnUrl);
		}

		return RedirectToPage("/Account/Manage");
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

	async Task<string?> StoreBearerTokenAsync(HttpClient client, DtoLoginRequest payload)
	{
		try
		{
			using var response = await client.PostAsJsonAsync("/login?useCookies=false", payload);
			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
			if (tokenResponse?.AccessToken == null)
			{
				return null;
			}

			Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
			{
				HttpOnly = true,
				Secure = Request.IsHttps,
				SameSite = SameSiteMode.Lax,
				MaxAge = TimeSpan.FromHours(1),
			});

			return tokenResponse.AccessToken;
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to obtain bearer token for user {Email}", Email);
			return null;
		}
	}

	sealed record TokenResponse(string TokenType, string AccessToken, long ExpiresIn, string RefreshToken);
}
