using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Definitions.Database;

namespace ObjectService.Pages.Account;

public sealed class LoginModel : PageModel
{
	private readonly SignInManager<TblUser> _signInManager;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<LoginModel> _logger;

	public LoginModel(
		SignInManager<TblUser> signInManager,
		IHttpClientFactory httpClientFactory,
		ILogger<LoginModel> logger)
	{
		_signInManager = signInManager;
		_httpClientFactory = httpClientFactory;
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

		// Find user by email first
		var user = await _signInManager.UserManager.FindByEmailAsync(Email.Trim());
		if (user == null)
		{
			ModelState.AddModelError(string.Empty, "Invalid email or password.");
			return Page();
		}

		var result = await _signInManager.PasswordSignInAsync(user, Password, isPersistent: false, lockoutOnFailure: true);

		if (!result.Succeeded)
		{
			if (result.IsLockedOut)
			{
				ModelState.AddModelError(string.Empty, "This account has been locked out due to too many failed login attempts. Please try again later.");
				return Page();
			}

			ModelState.AddModelError(string.Empty, "Invalid email or password.");
			return Page();
		}

		_logger.LogInformation("User {Email} logged in", Email);

		// Also obtain a bearer token from the Identity API for API calls
		await StoreBearerTokenAsync();

		if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
		{
			return Redirect(ReturnUrl);
		}

		return RedirectToPage("/Account/Manage");
	}

	private async Task StoreBearerTokenAsync()
	{
		try
		{
			var client = _httpClientFactory.CreateClient();
			client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host}");

			var loginPayload = new { Email = Email.Trim(), Password };
			var response = await client.PostAsJsonAsync("/login?useCookies=false", loginPayload);

			if (response.IsSuccessStatusCode)
			{
				var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
				if (tokenResponse?.AccessToken != null)
				{
					Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
					{
						HttpOnly = true,
						Secure = Request.IsHttps,
						SameSite = SameSiteMode.Lax,
						MaxAge = TimeSpan.FromHours(1),
					});
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, "Failed to obtain bearer token for user {Email}", Email);
		}
	}

	private sealed record TokenResponse(string TokenType, string AccessToken, long ExpiresIn, string RefreshToken);
}
