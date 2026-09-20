using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

/// <summary>
/// Database-backed scenario routes (<c>/v2/scenarios</c>). Scenario metadata is stored in the
/// database, while the scenario files themselves live on disk under the Scenarios folder - a
/// database row may or may not still have a corresponding file.
/// </summary>
public class ScenarioRouteHandler : ITableRouteHandler
{
	public string BaseRoute => Routes.Scenarios;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes(this, endpoints, endpoints.ServiceProvider.GetRequiredService<IConfiguration>());

	public void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
		=> parentRoute.MapGroup(Routes.ResourceRoute).MapGet(Routes.File, GetScenarioFileAsync);

	async Task<IResult> ListAsync([FromServices] IScenarioService svc, CancellationToken ct)
		=> Results.Ok(await svc.ListEntriesAsync(ct));

	async Task<IResult> GetScenarioFileAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioService svc, CancellationToken ct)
	{
		var path = await svc.GetScenarioFilePathByIdAsync(id, ct);
		return path != null ? Results.File(path, "application/octet-stream", Path.GetFileName(path)) : Results.NotFound();
	}

	Task<IResult> CreateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioService svc, CancellationToken ct)
	{
		var scenario = await svc.GetScenarioAsync(id, ct);
		return scenario != null ? Results.Ok(scenario) : Results.NotFound();
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoScenarioDescriptor request, [FromServices] IScenarioService svc, CancellationToken ct)
	{
		var updated = await svc.UpdateAsync(id, request, ct);
		return updated != null ? Results.Ok(updated) : Results.NotFound();
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioService svc, CancellationToken ct)
	{
		var deleted = await svc.DeleteAsync(id, ct);
		return deleted ? Results.Ok() : Results.NotFound();
	}
}
