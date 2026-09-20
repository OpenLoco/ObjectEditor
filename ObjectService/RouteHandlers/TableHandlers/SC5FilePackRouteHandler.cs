using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;
using System.Security.Claims;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ScenarioPackRouteHandler : ITableRouteHandler
{
	public string BaseRoute => Routes.ScenarioPacks;
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

	async Task<IResult> ListAsync([FromServices] IScenarioPackService svc, CancellationToken ct) => Results.Ok(await svc.ListEntriesAsync(ct));
	async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] IScenarioPackService svc, CancellationToken ct) => Results.Ok(await svc.GetPackAsync(id, ct));
	async Task<IResult> GetDescriptorAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioPackService svc, CancellationToken ct)
	{
		var descriptor = await svc.GetDescriptorAsync(id, ct);
		return descriptor != null ? Results.Ok(descriptor) : Results.NotFound();
	}

	async Task<IResult> GetPackFileAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioPackService svc, CancellationToken ct)
	{
		var (stream, name) = await svc.GetPackFileAsync(id, ct);
		return stream != null ? Results.File(stream, "application/zip", name) : Results.NotFound();
	}

	async Task<IResult> CreateAsync(
	[FromBody] DtoItemPackDescriptor<DtoScenarioEntry> request,
	HttpContext httpContext,
	[FromServices] IScenarioPackService svc,
	CancellationToken ct)
	{
		var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
		{
			return Results.Unauthorized();
		}

		var created = await svc.CreatePackAsync(request, userId, ct);
		return Results.Created($"{Routes.Prefix}{BaseRoute}/{created.Id}", created);
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoScenarioPackDescriptor request, [FromServices] IScenarioPackService svc, CancellationToken ct)
	{
		var updated = await svc.UpdateAsync(id, request, ct);
		return updated != null ? Results.Ok(updated) : Results.NotFound();
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioPackService svc, CancellationToken ct)
	{
		var deleted = await svc.DeleteAsync(id, ct);
		return deleted ? Results.Ok() : Results.NotFound();
	}
}