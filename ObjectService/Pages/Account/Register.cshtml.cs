using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace ObjectService.Pages.Account;

public sealed class RegisterModel : PageModel
{
	private readonly FrontendApiClient _api;
	private readonly ILogger<RegisterModel> _logger;

	public RegisterModel(FrontendApiClient api, ILogger<RegisterModel> logger)
	{
		_api = api;
		_logger = logger;
	}

	[BindProperty]
	[Required(ErrorMessage = "Username is required")]
	[MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
	public string UserName { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Please enter a valid email address")]
	public string Email { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Password is required")]
	[MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
	[DataType(DataType.Password)]
	public string Password { get; set; } = string.Empty;

	[BindProperty]
	[Required(ErrorMessage = "Please confirm your password")]
	[Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
	[DataType(DataType.Password)]
	public string ConfirmPassword { get; set; } = string.Empty;

	public bool RegistrationSuccess { get; set; }

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		using var client = _api.CreateClient();

		// Register through the Identity API.
		var registerPayload = new DtoRegisterRequest(Email.Trim(), UserName.Trim(), Password);
		using var registerResponse = await client.PostAsJsonAsync("/register", registerPayload);
		if (!registerResponse.IsSuccessStatusCode)
		{
			var error = await registerResponse.Content.ReadAsStringAsync();
			ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(error) ? "Registration failed." : error);
			return Page();
		}

		_logger.LogInformation("User {UserName} registered successfully", UserName);

		// Sign the new user in (+ bearer token for subsequent API calls).
		var loginPayload = new DtoLoginRequest(Email.Trim(), Password);
		using var cookieResponse = await client.PostAsJsonAsync("/login?useCookies=true", loginPayload);
		if (cookieResponse.IsSuccessStatusCode)
		{
			ForwardSetCookieHeaders(cookieResponse);
		}

		var accessToken = await StoreBearerTokenAsync(client, loginPayload);

		// The framework /register endpoint uses the email as the username, so set the
		// chosen username explicitly for the newly signed-in user.
		if (accessToken != null)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			_ = await Client.SetCurrentUserDisplayNameAsync(client, UserName.Trim());
		}

		RegistrationSuccess = true;
		return Page();
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
			_logger.LogWarning(ex, "Failed to obtain bearer token after registration for user {Email}", Email);
			return null;
		}
	}

	sealed record TokenResponse(string TokenType, string AccessToken, long ExpiresIn, string RefreshToken);
}
