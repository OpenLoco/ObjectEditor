using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ScenarioRouteHandler : ITableRouteHandler
{
	public string BaseRoute => RoutesV2.Scenarios;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes(this, endpoints, endpoints.ServiceProvider.GetRequiredService<IConfiguration>());

	public void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
		=> parentRoute.MapGroup(RoutesV2.ResourceRoute).MapGet(RoutesV2.File, GetScenarioFileAsync);

	Task<IResult> ListAsync([FromServices] IScenarioService svc)
	{
		var items = svc.ListScenarios();
		return Task.FromResult(Results.Ok(items.ToList()));
	}

	Task<IResult> GetScenarioFileAsync([FromRoute] UniqueObjectId id, [FromServices] IScenarioService svc)
	{
		var path = svc.GetScenarioFilePath(id);
		return Task.FromResult(path != null ? Results.File(path, "application/octet-stream", Path.GetFileName(path)) : Results.NotFound());
	}

	Task<IResult> CreateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	Task<IResult> ReadAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	Task<IResult> UpdateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	Task<IResult> DeleteAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}
