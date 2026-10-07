using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectService.Services;
using System.Security.Claims;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectPackRouteHandler : ITableRouteHandler
{
	public string BaseRoute => Routes.ObjectPacks;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;
	public void MapRoutes(IEndpointRouteBuilder e) => BaseTableRouteHandler.MapRoutes(this, e, e.ServiceProvider.GetRequiredService<IConfiguration>());
	public void MapAdditionalRoutes(IEndpointRouteBuilder p)
	{
		var resourceRoute = p.MapGroup(Routes.ResourceRoute);
		_ = resourceRoute.MapGet(Routes.File, GetPackFileAsync);
		_ = resourceRoute.MapGet(Routes.Descriptor, GetDescriptorAsync);
	}

	async Task<IResult> ListAsync([FromServices] IObjectPackService svc, CancellationToken ct) => Results.Ok(await svc.ListEntriesAsync(ct));
	async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] IObjectPackService svc, CancellationToken ct)
	{
		var pack = await svc.GetPackAsync(id, ct);
		return pack != null ? Results.Ok(pack) : Results.NotFound();
	}
	async Task<IResult> GetDescriptorAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectPackService svc, CancellationToken ct)
	{
		var descriptor = await svc.GetDescriptorAsync(id, ct);
		return descriptor != null ? Results.Ok(descriptor) : Results.NotFound();
	}

	async Task<IResult> GetPackFileAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectPackService svc, CancellationToken ct)
	{
		var (stream, name) = await svc.GetPackFileAsync(id, ct);
		return stream != null ? Results.File(stream, "application/zip", name) : Results.NotFound();
	}

	async Task<IResult> CreateAsync(
	[FromBody] DtoItemPackDescriptor<DtoObjectEntry> request,
	HttpContext httpContext,
	[FromServices] IObjectPackService svc,
	CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.Name))
		{
			return Results.Problem("Name required", statusCode: StatusCodes.Status400BadRequest);
		}

		var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
		{
			return Results.Unauthorized();
		}

		try
		{
			var created = await svc.CreatePackAsync(request, userId, ct);
			return Results.Created($"{Routes.Prefix}{BaseRoute}/{created.Id}", created);
		}
		catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueConstraintViolation(ex))
		{
			return Results.Problem("A pack with the same name already exists.", statusCode: StatusCodes.Status409Conflict);
		}
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoObjectPackDescriptor request, [FromServices] IObjectPackService svc, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.Name))
		{
			return Results.Problem("Name required", statusCode: StatusCodes.Status400BadRequest);
		}

		try
		{
			var updated = await svc.UpdateAsync(id, request, ct);
			return updated != null ? Results.Ok(updated) : Results.NotFound();
		}
		catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueConstraintViolation(ex))
		{
			return Results.Problem("A pack with the same name already exists.", statusCode: StatusCodes.Status409Conflict);
		}
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectPackService svc, CancellationToken ct)
	{
		var deleted = await svc.DeleteAsync(id, ct);
		return deleted ? Results.Ok() : Results.NotFound();
	}
}