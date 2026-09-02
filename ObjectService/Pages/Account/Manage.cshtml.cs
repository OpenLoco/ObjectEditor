using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.ObjectModels.Types;

namespace ObjectService.Pages.Account;

[Authorize]
public sealed class ManageModel : PageModel
{
	private readonly UserManager<TblUser> _userManager;
	private readonly SignInManager<TblUser> _signInManager;
	private readonly LocoDbContext _db;

	public ManageModel(
		UserManager<TblUser> userManager,
		SignInManager<TblUser> signInManager,
		LocoDbContext db)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_db = db;
	}

	public string Username { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public bool AccountDeleted { get; set; }
	public List<OwnedObjectViewModel> OwnedObjects { get; set; } = [];

	public async Task<IActionResult> OnGetAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return RedirectToPage("/Account/Login");
		}

		Username = user.UserName ?? string.Empty;
		Email = user.Email ?? string.Empty;

		OwnedObjects = await _db.Objects
			.Where(x => x.OwnerUserId == user.Id)
			.Include(x => x.DatObjects)
			.OrderByDescending(x => x.UploadedDate)
			.Select(x => new OwnedObjectViewModel(
				x.Id,
				x.Name,
				x.ObjectType,
				x.ObjectSource,
				x.UploadedDate,
				x.DatObjects.OrderBy(d => d.DatName).Select(d => d.DatName).FirstOrDefault() ?? x.Name))
			.ToListAsync();

		return Page();
	}

	public async Task<IActionResult> OnPostDeleteAccountAsync()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return RedirectToPage("/Index");
		}

		// Clear ownership references so objects remain but are no longer associated
		var ownedObjects = await _db.Objects.Where(x => x.OwnerUserId == user.Id).ToListAsync();
		foreach (var obj in ownedObjects)
		{
			obj.OwnerUserId = null;
		}

		await _db.SaveChangesAsync();

		// Delete the user
		await _userManager.DeleteAsync(user);
		await _signInManager.SignOutAsync();
		Response.Cookies.Delete("access_token");

		AccountDeleted = true;
		return Page();
	}

	public async Task<IActionResult> OnPostLogoutAsync()
	{
		await _signInManager.SignOutAsync();
		Response.Cookies.Delete("access_token");
		return RedirectToPage("/Index");
	}

	public sealed record OwnedObjectViewModel(
		UniqueObjectId Id,
		string InternalName,
		ObjectType ObjectType,
		ObjectSource ObjectSource,
		DateOnly UploadedDate,
		string DisplayName);
}