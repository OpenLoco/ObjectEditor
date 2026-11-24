using Definitions.DTO.Identity;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

/// <summary>
/// Database-administration routes for user records, mounted under <c>/v2/users</c>. All work is
/// delegated to <see cref="IUserService"/>; this type only maps HTTP concerns (routing, request
/// bodies) onto it. Self-service account routes for the signed-in user live under
/// <c>/v2/identity/manage</c> (<see cref="IdentityManageRouteHandler"/>).
/// </summary>
public class UserRouteHandler : ITableRouteHandler
{
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
		var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
		_ = resourceRoute.MapGet(Routes.Detail, GetDetailAsync).RequireAuthorization("AdminOnly");

		// The admin maintenance POSTs are registered here (rather than from the write path) so that
		// identity management stays available when ObjectService:BackendReadOnly disables game-data writes.
		_ = resourceRoute.MapPost(Routes.Roles, ToggleRoleAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.ClaimsSubRoute, ToggleClaimAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.Lockout, ToggleLockoutAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.EmailConfirmed, ToggleEmailConfirmedAsync).RequireAuthorization("AdminOnly");
		_ = resourceRoute.MapPost(Routes.PasswordReset, ForcePasswordResetAsync).RequireAuthorization("AdminOnly");
	}

	async Task<IResult> ListAsync([FromServices] IUserService svc, CancellationToken ct)
		=> Results.Ok(await svc.ListAsync(ct));

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] IUserService svc, CancellationToken ct)
	{
		var user = await svc.GetAsync(id, ct);
		return user != null ? Results.Ok(user) : Results.NotFound();
	}

	Task<IResult> CreateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoUserEntry request, [FromServices] IUserService svc, CancellationToken ct)
		=> ToResult(await svc.UpdateDisplayNameAsync(id, request.UserName, ct));

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] IUserService svc, CancellationToken ct)
		=> ToResult(await svc.DeleteAsync(id, ct));

	async Task<IResult> GetDetailAsync([FromRoute] UniqueObjectId id, [FromServices] IUserService svc, CancellationToken ct)
	{
		var detail = await svc.GetDetailAsync(id, ct);
		return detail != null ? Results.Ok(detail) : Results.NotFound();
	}

	async Task<IResult> ToggleRoleAsync([FromRoute] UniqueObjectId id, [FromBody] DtoUserRoleRequest request, [FromServices] IUserService svc, CancellationToken ct)
		=> ToResult(await svc.ToggleRoleAsync(id, request.Role, ct));

	async Task<IResult> ToggleClaimAsync([FromRoute] UniqueObjectId id, [FromBody] DtoUserClaimRequest request, [FromServices] IUserService svc, CancellationToken ct)
		=> ToResult(await svc.ToggleClaimAsync(id, request.Claim, ct));

	async Task<IResult> ToggleLockoutAsync([FromRoute] UniqueObjectId id, [FromServices] IUserService svc, CancellationToken ct)
		=> ToResult(await svc.ToggleLockoutAsync(id, ct));

	async Task<IResult> ToggleEmailConfirmedAsync([FromRoute] UniqueObjectId id, [FromServices] IUserService svc, CancellationToken ct)
		=> ToResult(await svc.ToggleEmailConfirmedAsync(id, ct));

	async Task<IResult> ForcePasswordResetAsync([FromRoute] UniqueObjectId id, [FromServices] IUserService svc, CancellationToken ct)
	{
		var token = await svc.ForcePasswordResetAsync(id, ct);
		return token != null ? Results.Ok(token) : Results.NotFound();
	}

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
