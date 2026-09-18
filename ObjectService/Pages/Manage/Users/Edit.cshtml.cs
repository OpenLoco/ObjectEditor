using Definitions;
using Definitions.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ObjectService.Frontend;
using ObjectService.RouteHandlers.TableHandlers;
using System.Security.Claims;

namespace ObjectService.Pages.Manage.Users;

[Authorize(Policy = "AdminOnly")]
public sealed class EditModel : PageModel
{
	readonly FrontendApiClient _api;

	public EditModel(FrontendApiClient api)
	{
		_api = api;
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
		return UserDetail == null ? NotFound() : Page();
	}

	// ── POST: Update display name ──

	public async Task<IActionResult> OnPostUpdateDisplayNameAsync()
	{
		using var client = _api.CreateClient();
		if (await Client.GetUserDetailAsync(client, UserId) == null)
		{
			return NotFound();
		}

		if (string.IsNullOrWhiteSpace(NewDisplayName))
		{
			ErrorMessage = "Display name cannot be empty.";
			await LoadUserAsync(UserId);
			return Page();
		}

		var result = await Client.SetUserDisplayNameAsync(client, UserId, NewDisplayName.Trim());
		if (result != null)
		{
			SuccessMessage = $"Display name updated to \"{NewDisplayName.Trim()}\".";
		}
		else
		{
			ErrorMessage = "Failed to update display name.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle role membership ──

	public async Task<IActionResult> OnPostToggleRoleAsync()
	{
		using var client = _api.CreateClient();
		var detail = await Client.GetUserDetailAsync(client, UserId);
		if (detail == null)
		{
			return NotFound();
		}

		if (string.IsNullOrEmpty(RoleToToggle))
		{
			await LoadUserAsync(UserId);
			return Page();
		}

		var isInRole = detail.Roles.Contains(RoleToToggle);
		var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (isInRole && RoleToToggle == "Admin" && UserId.ToString() == currentUserId)
		{
			ErrorMessage = "You cannot remove yourself from the Admin role.";
			await LoadUserAsync(UserId);
			return Page();
		}

		var updated = await Client.ToggleUserRoleAsync(client, UserId, RoleToToggle);
		if (updated != null)
		{
			SuccessMessage = isInRole
				? $"Removed \"{detail.UserName}\" from role \"{RoleToToggle}\"."
				: $"Added \"{detail.UserName}\" to role \"{RoleToToggle}\".";
		}
		else
		{
			ErrorMessage = "Failed to toggle role.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle permission claim ──

	public async Task<IActionResult> OnPostTogglePermissionAsync()
	{
		using var client = _api.CreateClient();
		var detail = await Client.GetUserDetailAsync(client, UserId);
		if (detail == null)
		{
			return NotFound();
		}

		if (string.IsNullOrEmpty(PermissionToToggle))
		{
			await LoadUserAsync(UserId);
			return Page();
		}

		var hasClaim = detail.PermissionClaims.Contains(PermissionToToggle);
		var updated = await Client.ToggleUserClaimAsync(client, UserId, PermissionToToggle);
		if (updated != null)
		{
			SuccessMessage = hasClaim
				? $"Revoked permission \"{PermissionToToggle}\"."
				: $"Granted permission \"{PermissionToToggle}\".";
		}
		else
		{
			ErrorMessage = "Failed to toggle permission.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Force password reset ──

	public async Task<IActionResult> OnPostForcePasswordResetAsync()
	{
		using var client = _api.CreateClient();
		var result = await Client.ForceUserPasswordResetAsync(client, UserId);
		if (result != null)
		{
			SuccessMessage = $"Password reset token generated: {result.Token}";
		}
		else
		{
			ErrorMessage = "Failed to generate a password reset token.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle email confirmation ──

	public async Task<IActionResult> OnPostToggleEmailConfirmedAsync()
	{
		using var client = _api.CreateClient();
		var updated = await Client.ToggleUserEmailConfirmedAsync(client, UserId);
		if (updated != null)
		{
			SuccessMessage = updated.EmailConfirmed
				? $"Email for \"{updated.UserName}\" has been confirmed."
				: $"Email confirmation for \"{updated.UserName}\" has been revoked.";
		}
		else
		{
			ErrorMessage = "Failed to toggle email confirmation.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Toggle lockout ──

	public async Task<IActionResult> OnPostToggleLockoutAsync()
	{
		using var client = _api.CreateClient();
		var updated = await Client.ToggleUserLockoutAsync(client, UserId);
		if (updated != null)
		{
			SuccessMessage = updated.IsLockedOut
				? $"\"{updated.UserName}\" has been locked out."
				: $"\"{updated.UserName}\" has been unlocked.";
		}
		else
		{
			ErrorMessage = "Failed to toggle lockout.";
		}

		await LoadUserAsync(UserId);
		return Page();
	}

	// ── POST: Delete user ──

	public async Task<IActionResult> OnPostDeleteUserAsync()
	{
		using var client = _api.CreateClient();
		var detail = await Client.GetUserDetailAsync(client, UserId);
		if (detail == null)
		{
			return NotFound();
		}

		var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (UserId.ToString() == currentUserId)
		{
			ErrorMessage = "You cannot delete your own account.";
			await LoadUserAsync(UserId);
			return Page();
		}

		var deleted = await Client.DeleteUserAsync(client, UserId);
		if (deleted)
		{
			SuccessMessage = $"User \"{detail.UserName}\" has been deleted.";
			return RedirectToPage("/Manage/Users/Index");
		}

		ErrorMessage = "Failed to delete user.";
		await LoadUserAsync(UserId);
		return Page();
	}

	// ── Helpers ──

	async Task LoadUserAsync(UniqueObjectId id)
	{
		using var client = _api.CreateClient();
		var detail = await Client.GetUserDetailAsync(client, id);
		if (detail == null)
		{
			return;
		}

		UserId = detail.Id;

		UserDetail = new UserDetailViewModel(
			detail.Id,
			detail.UserName,
			detail.Email,
			detail.EmailConfirmed,
			detail.IsLockedOut,
			[.. detail.Roles],
			detail.AssociatedAuthorId,
			detail.AssociatedAuthorName);

		AllRoles = [.. (await Client.GetRolesAsync(client))
			.OrderBy(r => r.Name)
			.Select(r => new RoleViewModel(r.Id, r.Name))];

		PermissionClaims = [.. UserRouteHandler.KnownPermissions.Select(p =>
			new UserClaimViewModel(
				Permission: p,
				HasClaim: detail.PermissionClaims.Contains(p)))];
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
