using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;
using System.Security.Claims;

namespace ObjectService.RouteHandlers;

/// <summary>
/// Self-service account routes for the signed-in user, mounted under <c>/v2/identity/manage</c>
/// alongside ASP.NET Identity's own manage endpoints (<c>manage/info</c>, <c>manage/2fa</c>).
/// These act on the caller's own account, so they are identity concerns and deliberately live here
/// rather than in the admin-only <c>/v2/users</c> database-administration area.
/// </summary>
public static class IdentityManageRouteHandler
{
	/// <summary>
	/// Maps the self-service account routes onto the identity route group (which is already mounted
	/// under <c>/v2/identity</c> and carries the "Identity" tag).
	/// </summary>
	public static void MapRoutes(IEndpointRouteBuilder identityGroup)
	{
		// Always require an authenticated user: these routes act on the caller's own account.
		var manage = identityGroup
			.MapGroup(Routes.Manage)
			.RequireAuthorization();

		_ = manage.MapPut(Routes.Profile, UpdateDisplayNameAsync);
		_ = manage.MapDelete(Routes.Account, DeleteAccountAsync);
	}

	static async Task<IResult> UpdateDisplayNameAsync(HttpContext httpContext, [FromBody] DtoUserEntry request, [FromServices] IUserService svc, CancellationToken ct)
	{
		var userId = GetCurrentUserId(httpContext);
		return userId.HasValue
			? ToResult(await svc.UpdateDisplayNameAsync(userId.Value, request.UserName, ct))
			: Results.Unauthorized();
	}

	static async Task<IResult> DeleteAccountAsync(HttpContext httpContext, [FromServices] IUserService svc, CancellationToken ct)
	{
		var userId = GetCurrentUserId(httpContext);
		return userId.HasValue
			? ToResult(await svc.DeleteAsync(userId.Value, ct))
			: Results.Unauthorized();
	}

	static UniqueObjectId? GetCurrentUserId(HttpContext context)
		=> ulong.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

	static IResult ToResult<T>(UserOperationResult<T> result)
	{
		if (result.Success)
		{
			return Results.Ok(result.Value);
		}

		return result.StatusCode == StatusCodes.Status404NotFound
			? Results.NotFound()
			: Results.Problem(result.ErrorMessage, statusCode: result.StatusCode);
	}
}