using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Definitions.Database;

namespace ObjectService.Pages.Dev;

[AllowAnonymous]
public class QuickLoginModel : PageModel
{
	private readonly SignInManager<TblUser> _signInManager;
	private readonly UserManager<TblUser> _userManager;
	private readonly RoleManager<TblUserRole> _roleManager;
	private readonly IWebHostEnvironment _environment;

	public QuickLoginModel(
		SignInManager<TblUser> signInManager,
		UserManager<TblUser> userManager,
		RoleManager<TblUserRole> roleManager,
		IWebHostEnvironment environment)
	{
		_signInManager = signInManager;
		_userManager = userManager;
		_roleManager = roleManager;
		_environment = environment;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!_environment.IsDevelopment())
		{
			return Forbid();
		}

		const string devUserEmail = "dev@localhost";
		const string devUserName = "DevAdmin";
		const string devPassword = "DevPassword123!@#";

		var user = await _userManager.FindByEmailAsync(devUserEmail);
		if (user == null)
		{
			user = new TblUser
			{
				UserName = devUserName,
				Email = devUserEmail,
				EmailConfirmed = true,
			};

			var result = await _userManager.CreateAsync(user, devPassword);
			if (!result.Succeeded)
			{
				return BadRequest("Failed to create dev user");
			}
		}

		// Ensure Admin role exists and user has it
		if (!await _roleManager.RoleExistsAsync("Admin"))
		{
			await _roleManager.CreateAsync(new TblUserRole { Name = "Admin" });
		}

		if (!await _userManager.IsInRoleAsync(user, "Admin"))
		{
			await _userManager.AddToRoleAsync(user, "Admin");
		}

		// Sign in the user
		await _signInManager.SignInAsync(user, isPersistent: true);

		// Redirect back to the referring page, or to the admin dashboard
		var returnUrl = Request.Headers.Referer.ToString();
		if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
		{
			returnUrl = "/Admin";
		}

		return Redirect(returnUrl);
	}
}
