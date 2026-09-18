using Definitions.Database;
using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectService.Identity;
using ObjectService.Services;
using System.Security.Claims;

namespace ObjectService.RouteHandlers.TableHandlers;

/// <summary>
/// Identity-backed user management routes. Unlike the generic CRUD handlers, these
/// go through <see cref="UserManager{TUser}"/>/<see cref="RoleManager{TRole}"/> so
/// roles, claims and lockout state are handled correctly.
/// </summary>
public class UserRouteHandler : ITableRouteHandler
{
	public static readonly string[] KnownPermissions =
	[
		LocoPermissions.ObjectPacksCreate,
		LocoPermissions.TagsManage,
		LocoPermissions.LicenceManage,
		LocoPermissions.AuthorManage,
		LocoPermissions.DisplayNameChange,
	];

	public string BaseRoute => Routes.Users;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder e)
		=> BaseTableRouteHandler.MapRoutes(this, e, e.ServiceProvider.GetRequiredService<IConfiguration>());

	public void MapAdditionalRoutes(IEndpointRouteBuilder baseRoute)
	{
		_ = baseRoute.MapDelete(Routes.Me, DeleteCurrentUserAsync);
		_ = baseRoute.MapPut(Routes.Me, UpdateCurrentUserAsync);

		var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
		_ = resourceRoute.MapGet(Routes.Detail, GetDetailAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.RolesSubRoute, ToggleRoleAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.ClaimsSubRoute, ToggleClaimAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.Lockout, ToggleLockoutAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.EmailConfirmed, ToggleEmailConfirmedAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.PasswordReset, ForcePasswordResetAsync).RequireAuthorization("AdminOnly");
	}

	async Task<IResult> ListAsync([FromServices] LocoDbContext db, CancellationToken ct)
	{
		var roleNameById = await db.Roles.ToDictionaryAsync(r => r.Id, r => r.Name ?? string.Empty, ct);
		var userRoles = await db.UserRoles.ToListAsync(ct);

		var users = await db.Users.OrderBy(u => u.UserName).ToListAsync(ct);
		var result = users.Select(u => new DtoUserListEntry(
			u.Id,
			u.UserName ?? string.Empty,
			u.Email ?? string.Empty,
			[.. userRoles.Where(ur => ur.UserId == u.Id && roleNameById.ContainsKey(ur.RoleId)).Select(ur => roleNameById[ur.RoleId])]));

		return Results.Ok(result);
	}

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, CancellationToken ct)
	{
		var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
		return user != null ? Results.Ok(new DtoUserEntry(user.Id, user.UserName ?? string.Empty)) : Results.NotFound();
	}

	Task<IResult> CreateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoUserEntry request, [FromServices] UserManager<TblUser> userManager, CancellationToken ct)
	{
		var user = await userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return Results.NotFound();
		}

		if (string.IsNullOrWhiteSpace(request.UserName))
		{
			return Results.Problem("UserName required", statusCode: StatusCodes.Status400BadRequest);
		}

		var result = await userManager.SetUserNameAsync(user, request.UserName.Trim());
		if (!result.Succeeded)
		{
			return Results.Problem(string.Join("; ", result.Errors.Select(e => e.Description)), statusCode: StatusCodes.Status400BadRequest);
		}

		return Results.Ok(new DtoUserEntry(user.Id, user.UserName ?? string.Empty));
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] UserManager<TblUser> userManager, CancellationToken ct)
	{
		var user = await userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return Results.NotFound();
		}

		await ClearOwnershipAsync(db, user.Id, ct);
		var result = await userManager.DeleteAsync(user);
		return result.Succeeded
			? Results.Ok()
			: Results.Problem(string.Join("; ", result.Errors.Select(e => e.Description)), statusCode: StatusCodes.Status400BadRequest);
	}

	async Task<IResult> DeleteCurrentUserAsync(
		HttpContext httpContext,
		[FromServices] LocoDbContext db,
		[FromServices] UserManager<TblUser> userManager,
		CancellationToken ct)
	{
		var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
		{
			return Results.Unauthorized();
		}

		var user = await userManager.FindByIdAsync(userId.ToString());
		if (user == null)
		{
			return Results.NotFound();
		}

		await ClearOwnershipAsync(db, user.Id, ct);
		var result = await userManager.DeleteAsync(user);
		return result.Succeeded
			? Results.Ok()
			: Results.Problem(string.Join("; ", result.Errors.Select(e => e.Description)), statusCode: StatusCodes.Status400BadRequest);
	}

	async Task<IResult> UpdateCurrentUserAsync(HttpContext httpContext, [FromBody] DtoUserEntry request, [FromServices] UserManager<TblUser> userManager)
	{
		var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
		{
			return Results.Unauthorized();
		}

		var user = await userManager.FindByIdAsync(userId.ToString());
		if (user == null)
		{
			return Results.NotFound();
		}

		if (string.IsNullOrWhiteSpace(request.UserName))
		{
			return Results.Problem("UserName required", statusCode: StatusCodes.Status400BadRequest);
		}

		var result = await userManager.SetUserNameAsync(user, request.UserName.Trim());
		return result.Succeeded
			? Results.Ok(new DtoUserEntry(user.Id, user.UserName ?? string.Empty))
			: Results.Problem(string.Join("; ", result.Errors.Select(e => e.Description)), statusCode: StatusCodes.Status400BadRequest);
	}

	async Task<IResult> GetDetailAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] UserManager<TblUser> userManager, CancellationToken ct)
	{
		var user = await db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == id, ct);
		if (user == null)
		{
			return Results.NotFound();
		}

		return Results.Ok(await BuildDetailAsync(user, userManager));
	}

	async Task<IResult> ToggleRoleAsync([FromRoute] UniqueObjectId id, [FromBody] DtoUserRoleRequest request, [FromServices] LocoDbContext db, [FromServices] UserManager<TblUser> userManager, CancellationToken ct)
	{
		var user = await db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == id, ct);
		if (user == null)
		{
			return Results.NotFound();
		}

		if (string.IsNullOrWhiteSpace(request.Role))
		{
			return Results.Problem("Role required", statusCode: StatusCodes.Status400BadRequest);
		}

		if (await userManager.IsInRoleAsync(user, request.Role))
		{
			_ = await userManager.RemoveFromRoleAsync(user, request.Role);
		}
		else
		{
			_ = await userManager.AddToRoleAsync(user, request.Role);
		}

		return Results.Ok(await BuildDetailAsync(user, userManager));
	}

	async Task<IResult> ToggleClaimAsync([FromRoute] UniqueObjectId id, [FromBody] DtoUserClaimRequest request, [FromServices] LocoDbContext db, [FromServices] UserManager<TblUser> userManager, CancellationToken ct)
	{
		var user = await db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == id, ct);
		if (user == null)
		{
			return Results.NotFound();
		}

		if (string.IsNullOrWhiteSpace(request.Claim))
		{
			return Results.Problem("Claim required", statusCode: StatusCodes.Status400BadRequest);
		}

		var existing = (await userManager.GetClaimsAsync(user))
			.FirstOrDefault(c => c.Type == LocoPermissions.ClaimType && c.Value == request.Claim);

		if (existing != null)
		{
			_ = await userManager.RemoveClaimAsync(user, existing);
		}
		else
		{
			_ = await userManager.AddClaimAsync(user, new Claim(LocoPermissions.ClaimType, request.Claim));
		}

		return Results.Ok(await BuildDetailAsync(user, userManager));
	}

	async Task<IResult> ToggleLockoutAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] UserManager<TblUser> userManager, CancellationToken ct)
	{
		var user = await db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == id, ct);
		if (user == null)
		{
			return Results.NotFound();
		}

		if (await userManager.IsLockedOutAsync(user))
		{
			_ = await userManager.SetLockoutEndDateAsync(user, null);
		}
		else
		{
			_ = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
		}

		user = await db.Users.Include(u => u.AssociatedAuthor).FirstAsync(u => u.Id == id, ct);
		return Results.Ok(await BuildDetailAsync(user, userManager));
	}

	async Task<IResult> ToggleEmailConfirmedAsync(
		[FromRoute] UniqueObjectId id,
		[FromServices] LocoDbContext db,
		[FromServices] UserManager<TblUser> userManager,
		[FromServices] IUserStore<TblUser> userStore,
		CancellationToken ct)
	{
		var user = await db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == id, ct);
		if (user == null)
		{
			return Results.NotFound();
		}

		if (!await userManager.IsEmailConfirmedAsync(user))
		{
			var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
			_ = await userManager.ConfirmEmailAsync(user, token);
		}
		else if (userStore is IUserEmailStore<TblUser> emailStore)
		{
			await emailStore.SetEmailConfirmedAsync(user, false, ct);
			_ = await db.SaveChangesAsync(ct);
		}

		return Results.Ok(await BuildDetailAsync(user, userManager));
	}

	async Task<IResult> ForcePasswordResetAsync([FromRoute] UniqueObjectId id, [FromServices] UserManager<TblUser> userManager)
	{
		var user = await userManager.FindByIdAsync(id.ToString());
		if (user == null)
		{
			return Results.NotFound();
		}

		var token = await userManager.GeneratePasswordResetTokenAsync(user);
		return Results.Ok(new DtoPasswordResetTokenResponse(token));
	}

	static async Task<DtoUserDetailDescriptor> BuildDetailAsync(TblUser user, UserManager<TblUser> userManager)
	{
		var roles = await userManager.GetRolesAsync(user);
		var claims = await userManager.GetClaimsAsync(user);
		var isLockedOut = await userManager.IsLockedOutAsync(user);

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

	static async Task ClearOwnershipAsync(LocoDbContext db, UniqueObjectId userId, CancellationToken ct)
	{
		var ownedObjects = await db.Objects.Where(o => o.OwnerUserId == userId).ToListAsync(ct);
		foreach (var obj in ownedObjects)
		{
			obj.OwnerUserId = null;
		}

		_ = await db.SaveChangesAsync(ct);
	}
}
