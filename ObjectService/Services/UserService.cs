using Definitions.Database;
using Definitions.DTO.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ObjectService.Identity;
using System.Security.Claims;

namespace ObjectService.Services;

/// <summary>
/// The outcome of a user-management operation that can fail with a client-facing message,
/// mirroring the shape used by <see cref="ObjectQueryService"/>'s <c>UploadResult</c>.
/// </summary>
public record UserOperationResult<T>(bool Success, T? Value, string? ErrorMessage, int StatusCode)
{
	public static UserOperationResult<T> Ok(T value) => new(true, value, null, 200);
	public static UserOperationResult<T> NotFound() => new(false, default, null, 404);
	public static UserOperationResult<T> Problem(string? errorMessage, int statusCode = 400) => new(false, default, errorMessage, statusCode);
}

/// <summary>
/// Identity-backed user management that backs the <c>/v2/users</c> routes. Unlike the plain
/// CRUD services this goes through <see cref="UserManager{TUser}"/> (and
/// <see cref="IUserStore{TUser}"/> for email confirmation) so roles, claims and lockout
/// state are handled correctly.
/// </summary>
public interface IUserService
{
	Task<IEnumerable<DtoUserListEntry>> ListAsync(CancellationToken ct);
	Task<DtoUserEntry?> GetAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoUserDetailDescriptor?> GetDetailAsync(UniqueObjectId id, CancellationToken ct);
	Task<UserOperationResult<DtoUserEntry>> UpdateDisplayNameAsync(UniqueObjectId id, string? displayName, CancellationToken ct);
	Task<UserOperationResult<bool>> DeleteAsync(UniqueObjectId id, CancellationToken ct);
	Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleRoleAsync(UniqueObjectId id, string? role, CancellationToken ct);
	Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleClaimAsync(UniqueObjectId id, string? claim, CancellationToken ct);
	Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleLockoutAsync(UniqueObjectId id, CancellationToken ct);
	Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleEmailConfirmedAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoPasswordResetTokenResponse?> ForcePasswordResetAsync(UniqueObjectId id, CancellationToken ct);
}

public class UserService : IUserService
{
	private readonly LocoDbContext _db;
	private readonly UserManager<TblUser> _userManager;
	private readonly IUserStore<TblUser> _userStore;

	public UserService(LocoDbContext db, UserManager<TblUser> userManager, IUserStore<TblUser> userStore)
	{
		_db = db;
		_userManager = userManager;
		_userStore = userStore;
	}

	public async Task<IEnumerable<DtoUserListEntry>> ListAsync(CancellationToken ct)
	{
		var roleNameById = await _db.Roles.ToDictionaryAsync(r => r.Id, r => r.Name ?? string.Empty, ct);
		var userRoles = await _db.UserRoles.ToListAsync(ct);

		var users = await _db.Users.OrderBy(u => u.UserName).ToListAsync(ct);
		return users.Select(u => new DtoUserListEntry(
			u.Id,
			u.UserName ?? string.Empty,
			u.Email ?? string.Empty,
			[.. userRoles.Where(ur => ur.UserId == u.Id && roleNameById.ContainsKey(ur.RoleId)).Select(ur => roleNameById[ur.RoleId])]));
	}

	public async Task<DtoUserEntry?> GetAsync(UniqueObjectId id, CancellationToken ct)
	{
		var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
		return user != null ? new DtoUserEntry(user.Id, user.UserName ?? string.Empty) : null;
	}

	public async Task<DtoUserDetailDescriptor?> GetDetailAsync(UniqueObjectId id, CancellationToken ct)
	{
		var user = await FindWithAuthorAsync(id, ct);
		return user == null ? null : await BuildDetailAsync(user);
	}

	public async Task<UserOperationResult<DtoUserEntry>> UpdateDisplayNameAsync(UniqueObjectId id, string? displayName, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(displayName))
		{
			return UserOperationResult<DtoUserEntry>.Problem("UserName required");
		}

		var user = await _userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return UserOperationResult<DtoUserEntry>.NotFound();
		}

		var result = await _userManager.SetUserNameAsync(user, displayName.Trim());
		return result.Succeeded
			? UserOperationResult<DtoUserEntry>.Ok(new DtoUserEntry(user.Id, user.UserName ?? string.Empty))
			: UserOperationResult<DtoUserEntry>.Problem(JoinErrors(result));
	}

	public async Task<UserOperationResult<bool>> DeleteAsync(UniqueObjectId id, CancellationToken ct)
	{
		var user = await _userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return UserOperationResult<bool>.NotFound();
		}

		await ClearOwnershipAsync(user.Id, ct);
		var result = await _userManager.DeleteAsync(user);
		return result.Succeeded
			? UserOperationResult<bool>.Ok(true)
			: UserOperationResult<bool>.Problem(JoinErrors(result));
	}

	public async Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleRoleAsync(UniqueObjectId id, string? role, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(role))
		{
			return UserOperationResult<DtoUserDetailDescriptor>.Problem("Role required");
		}

		var user = await FindWithAuthorAsync(id, ct);
		if (user == null)
		{
			return UserOperationResult<DtoUserDetailDescriptor>.NotFound();
		}

		if (await _userManager.IsInRoleAsync(user, role))
		{
			_ = await _userManager.RemoveFromRoleAsync(user, role);
		}
		else
		{
			_ = await _userManager.AddToRoleAsync(user, role);
		}

		return UserOperationResult<DtoUserDetailDescriptor>.Ok(await BuildDetailAsync(user));
	}

	public async Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleClaimAsync(UniqueObjectId id, string? claim, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(claim))
		{
			return UserOperationResult<DtoUserDetailDescriptor>.Problem("Claim required");
		}

		var user = await FindWithAuthorAsync(id, ct);
		if (user == null)
		{
			return UserOperationResult<DtoUserDetailDescriptor>.NotFound();
		}

		var existing = (await _userManager.GetClaimsAsync(user))
			.FirstOrDefault(c => c.Type == LocoPermissions.ClaimType && c.Value == claim);

		if (existing != null)
		{
			_ = await _userManager.RemoveClaimAsync(user, existing);
		}
		else
		{
			_ = await _userManager.AddClaimAsync(user, new Claim(LocoPermissions.ClaimType, claim));
		}

		return UserOperationResult<DtoUserDetailDescriptor>.Ok(await BuildDetailAsync(user));
	}

	public async Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleLockoutAsync(UniqueObjectId id, CancellationToken ct)
	{
		var user = await FindWithAuthorAsync(id, ct);
		if (user == null)
		{
			return UserOperationResult<DtoUserDetailDescriptor>.NotFound();
		}

		if (await _userManager.IsLockedOutAsync(user))
		{
			_ = await _userManager.SetLockoutEndDateAsync(user, null);
		}
		else
		{
			_ = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
		}

		user = await FindWithAuthorAsync(id, ct) ?? user;
		return UserOperationResult<DtoUserDetailDescriptor>.Ok(await BuildDetailAsync(user));
	}

	public async Task<UserOperationResult<DtoUserDetailDescriptor>> ToggleEmailConfirmedAsync(UniqueObjectId id, CancellationToken ct)
	{
		var user = await FindWithAuthorAsync(id, ct);
		if (user == null)
		{
			return UserOperationResult<DtoUserDetailDescriptor>.NotFound();
		}

		if (!await _userManager.IsEmailConfirmedAsync(user))
		{
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			_ = await _userManager.ConfirmEmailAsync(user, token);
		}
		else if (_userStore is IUserEmailStore<TblUser> emailStore)
		{
			await emailStore.SetEmailConfirmedAsync(user, false, ct);
			_ = await _db.SaveChangesAsync(ct);
		}

		return UserOperationResult<DtoUserDetailDescriptor>.Ok(await BuildDetailAsync(user));
	}

	public async Task<DtoPasswordResetTokenResponse?> ForcePasswordResetAsync(UniqueObjectId id, CancellationToken ct)
	{
		var user = await _userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return null;
		}

		var token = await _userManager.GeneratePasswordResetTokenAsync(user);
		return new DtoPasswordResetTokenResponse(token);
	}

	async Task<TblUser?> FindWithAuthorAsync(UniqueObjectId id, CancellationToken ct)
		=> await _db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == id, ct);

	async Task ClearOwnershipAsync(UniqueObjectId userId, CancellationToken ct)
	{
		var ownedObjects = await _db.Objects.Where(o => o.OwnerUserId == userId).ToListAsync(ct);
		foreach (var obj in ownedObjects)
		{
			obj.OwnerUserId = null;
		}

		_ = await _db.SaveChangesAsync(ct);
	}

	async Task<DtoUserDetailDescriptor> BuildDetailAsync(TblUser user)
	{
		var roles = await _userManager.GetRolesAsync(user);
		var claims = await _userManager.GetClaimsAsync(user);
		var isLockedOut = await _userManager.IsLockedOutAsync(user);

		return new DtoUserDetailDescriptor(
			user.Id,
			user.UserName ?? string.Empty,
			user.Email ?? string.Empty,
			user.EmailConfirmed,
			isLockedOut,
			[.. roles],
			[.. claims.Where(c => c.Type == LocoPermissions.ClaimType).Select(c => c.Value)],
			user.AssociatedAuthorId,
			user.AssociatedAuthor?.Name);
	}

	static string JoinErrors(IdentityResult result)
		=> string.Join("; ", result.Errors.Select(e => e.Description));
}
