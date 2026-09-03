using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using ObjectService.Identity;

namespace ObjectService.Pages.Manage.Users;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	private readonly LocoDbContext _db;
	private readonly UserManager<TblUser> _userManager;
	private readonly RoleManager<TblUserRole> _roleManager;

	public EditModel(
		LocoDbContext db,
		UserManager<TblUser> userManager,
		RoleManager<TblUserRole> roleManager)
	{
		_db = db;
		_userManager = userManager;
		_roleManager = roleManager;
	}

	// ── View data ──

	public UserDetailViewModel? UserDetail { get; set; }
	public List<RoleViewModel> AllRoles { get; set; } = [];
	public List<UserClaimViewModel> PermissionClaims { get; set; } = [];

	[TempData]
	public string? SuccessMessage { get; set; }

	[TempData]
	public string? ErrorMessage { get; set; }

	// ── Form inputs (bound per-handler) ──

	[BindProperty]
	public UniqueObjectId UserId { get; set; }

	[BindProperty]
	public string? NewDisplayName { get; set; }

	[BindProperty]
	public string? RoleToToggle { get; set; }

	[BindProperty]
	public string? PermissionToToggle { get; set; }

	// ── GET ──

	public async Task<IActionResult> OnGetAsync(UniqueObjectId id)
	{
		await LoadUserAsync(id);
		if (UserDetail == null)
			return NotFound();

		return Page();
	}
// ── POST: Update display name ──

	public async Task<IActionResult> OnPostUpdateDisplayNameAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		if (string.IsNullOrWhiteSpace(NewDisplayName))
		{
			ErrorMessage = "Display name cannot be empty.";
			await LoadUserAsync(UserId);
			return Page();
		}

		var result = await _userManager.SetUserNameAsync(user, NewDisplayName.Trim());
		if (result.Succeeded)
		{
			SuccessMessage = $"Display name updated to \"{NewDisplayName.Trim()}\".";
		}
		else
		{
			ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle role membership ──

	public async Task<IActionResult> OnPostToggleRoleAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		if (string.IsNullOrEmpty(RoleToToggle))
		{
			await LoadUserAsync(UserId);
			return Page();
		}

		if (await _userManager.IsInRoleAsync(user, RoleToToggle))
		{
			var currentUserId = _userManager.GetUserId(User);
			if (RoleToToggle == "Admin" && user.Id.ToString() == currentUserId)
			{
				ErrorMessage = "You cannot remove yourself from the Admin role.";
				await LoadUserAsync(UserId);
				return Page();
			}

			await _userManager.RemoveFromRoleAsync(user, RoleToToggle);
			SuccessMessage = $"Removed \"{user.UserName}\" from role \"{RoleToToggle}\".";
		}
		else
		{
			await _userManager.AddToRoleAsync(user, RoleToToggle);
			SuccessMessage = $"Added \"{user.UserName}\" to role \"{RoleToToggle}\".";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle user permission claim ──

	public async Task<IActionResult> OnPostTogglePermissionAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		if (string.IsNullOrEmpty(PermissionToToggle))
		{
			await LoadUserAsync(UserId);
			return Page();
		}

		var existingClaims = await _userManager.GetClaimsAsync(user);
		var existing = existingClaims.FirstOrDefault(c =>
			c.Type == LocoPermissions.ClaimType && c.Value == PermissionToToggle);

		if (existing != null)
		{
			await _userManager.RemoveClaimAsync(user, existing);
			SuccessMessage = $"Revoked \"{PermissionToToggle}\" from \"{user.UserName}\".";
		}
		else
		{
			await _userManager.AddClaimAsync(user,
				new Claim(LocoPermissions.ClaimType, PermissionToToggle));
			SuccessMessage = $"Granted \"{PermissionToToggle}\" to \"{user.UserName}\".";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Force password reset ──

	public async Task<IActionResult> OnPostForcePasswordResetAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		var token = await _userManager.GeneratePasswordResetTokenAsync(user);
		SuccessMessage = $"Password reset token: {token}";

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle email confirmation ──

	public async Task<IActionResult> OnPostToggleEmailConfirmedAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		if (user.EmailConfirmed)
		{
			user.EmailConfirmed = false;
			await _userManager.UpdateAsync(user);
			SuccessMessage = $"Email confirmation revoked for \"{user.UserName}\".";
		}
		else
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var result = await _userManager.ConfirmEmailAsync(user, token);
			if (result.Succeeded)
			{
				SuccessMessage = $"Email confirmed for \"{user.UserName}\".";
			}
			else
			{
				ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
			}
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle lockout ──

	public async Task<IActionResult> OnPostToggleLockoutAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		if (await _userManager.IsLockedOutAsync(user))
		{
			await _userManager.SetLockoutEndDateAsync(user, null);
			SuccessMessage = $"\"{user.UserName}\" has been unlocked.";
		}
		else
		{
			await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
			SuccessMessage = $"\"{user.UserName}\" has been locked out.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Delete user ──

	public async Task<IActionResult> OnPostDeleteUserAsync()
	{
		var user = await _userManager.FindByIdAsync(UserId.ToString());
		if (user == null) return NotFound();

		var currentUserId = _userManager.GetUserId(User);
		if (user.Id.ToString() == currentUserId)
		{
			ErrorMessage = "You cannot delete your own account.";
			await LoadUserAsync(UserId);
			return Page();
		}

		var userName = user.UserName;

		var ownedObjects = await _db.Objects.Where(o => o.OwnerUserId == user.Id).ToListAsync();
		foreach (var obj in ownedObjects)
			obj.OwnerUserId = null;
		await _db.SaveChangesAsync();

		var result = await _userManager.DeleteAsync(user);
		if (result.Succeeded)
		{
			SuccessMessage = $"User \"{userName}\" has been deleted.";
			return RedirectToPage("/Manage/Users/Index");
		}

		ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
		await LoadUserAsync(UserId);
		return Page();
	}
// ── Helpers ──

	private async Task LoadUserAsync(UniqueObjectId id)
	{
		var user = await _db.Users
			.Include(u => u.AssociatedAuthor)
			.FirstOrDefaultAsync(u => u.Id == id);

		if (user == null) return;

		UserId = user.Id;

		var roles = await _userManager.GetRolesAsync(user);
		var claims = await _userManager.GetClaimsAsync(user);
		var isLockedOut = await _userManager.IsLockedOutAsync(user);

		UserDetail = new UserDetailViewModel(
			user.Id,
			user.UserName ?? string.Empty,
			user.Email ?? string.Empty,
			user.EmailConfirmed,
			isLockedOut,
			roles.ToList(),
			user.AssociatedAuthorId,
			user.AssociatedAuthor?.Name);

		AllRoles = await _roleManager.Roles
			.OrderBy(r => r.Name)
			.Select(r => new RoleViewModel(r.Id, r.Name ?? string.Empty))
			.ToListAsync();

		var knownPermissions = new[]
		{
			LocoPermissions.ObjectPacksCreate,
			LocoPermissions.TagsManage,
			LocoPermissions.LicenceManage,
			LocoPermissions.AuthorManage,
			LocoPermissions.DisplayNameChange,
		};

		PermissionClaims = knownPermissions.Select(p =>
			new UserClaimViewModel(
				Permission: p,
				HasClaim: claims.Any(c => c.Type == LocoPermissions.ClaimType && c.Value == p)))
			.ToList();
	}

	// ── View models ──

	public sealed record UserDetailViewModel(
		UniqueObjectId Id,
		string UserName,
		string Email,
		bool EmailConfirmed,
		bool IsLockedOut,
		List<string> Roles,
		UniqueObjectId? AssociatedAuthorId,
		string? AssociatedAuthorName);

	public sealed record RoleViewModel(UniqueObjectId Id, string Name);

	public sealed record UserClaimViewModel(string Permission, bool HasClaim);
}