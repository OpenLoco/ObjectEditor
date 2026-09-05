using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Definitions.Database;

namespace ObjectService.Pages.Account;

public sealed class RegisterModel : PageModel
{
	private readonly UserManager<TblUser> _userManager;
	private readonly SignInManager<TblUser> _signInManager;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<RegisterModel> _logger;

	public RegisterModel(
		UserManager<TblUser> userManager,
		SignInManager<TblUser> signInManager,
		IHttpClientFactory httpClientFactory,
		ILogger<RegisterModel> logger)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_httpClientFactory = httpClientFactory;
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

		var user = new TblUser
		{
			UserName = UserName.Trim(),
			Email = Email.Trim(),
		};

		var result = await _userManager.CreateAsync(user, Password);

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}

		_logger.LogInformation("User {UserName} registered successfully", UserName);

		// Sign the user in after registration
		await _signInManager.SignInAsync(user, isPersistent: false);

		// Obtain a bearer token for API calls
		await StoreBearerTokenAsync();

		RegistrationSuccess = true;
		return Page();
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
			_logger.LogWarning(ex, "Failed to obtain bearer token after registration for user {Email}", Email);
		}
	}

	private sealed record TokenResponse(string TokenType, string AccessToken, long ExpiresIn, string RefreshToken);
}