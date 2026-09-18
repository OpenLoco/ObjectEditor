using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

/// <summary>
/// Database-backed scenario (SC5) file routes. Distinct from <see cref="ScenarioRouteHandler"/>,
/// which enumerates scenario files on disk.
/// </summary>
public class SC5FileRouteHandler : ITableRouteHandler
{
	public string BaseRoute => Routes.SC5Files;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder e)
		=> BaseTableRouteHandler.MapRoutes(this, e, e.ServiceProvider.GetRequiredService<IConfiguration>());

	public void MapAdditionalRoutes(IEndpointRouteBuilder p)
		=> p.MapGroup(Routes.ResourceRoute).MapGet(Routes.File, GetFileAsync);

	async Task<IResult> ListAsync([FromServices] ISC5FileService svc, CancellationToken ct)
		=> Results.Ok(await svc.ListEntriesAsync(ct));

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] ISC5FileService svc, CancellationToken ct)
	{
		var descriptor = await svc.GetDescriptorAsync(id, ct);
		return descriptor != null ? Results.Ok(descriptor) : Results.NotFound();
	}

	async Task<IResult> GetFileAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioService svc, CancellationToken ct)
	{
		var path = await svc.GetScenarioFilePathByIdAsync(id, ct);
		return path != null ? Results.File(path, "application/octet-stream", Path.GetFileName(path)) : Results.NotFound();
	}

	Task<IResult> CreateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] Definitions.DTO.DtoSC5FileDescriptor request, [FromServices] ISC5FileService svc, CancellationToken ct)
	{
		var updated = await svc.UpdateAsync(id, request, ct);
		return updated != null ? Results.Ok(updated) : Results.NotFound();
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] ISC5FileService svc, CancellationToken ct)
	{
		var deleted = await svc.DeleteAsync(id, ct);
		return deleted ? Results.Ok() : Results.NotFound();
	}
}
