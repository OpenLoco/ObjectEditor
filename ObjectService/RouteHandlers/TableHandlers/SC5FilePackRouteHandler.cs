using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

public class SC5FilePackRouteHandler : ITableRouteHandler
{
	public string BaseRoute => RoutesV2.SC5FilePacks;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;
	public void MapRoutes(IEndpointRouteBuilder e) => BaseTableRouteHandler.MapRoutes(this, e, e.ServiceProvider.GetRequiredService<IConfiguration>());
	public void MapAdditionalRoutes(IEndpointRouteBuilder p) => p.MapGroup(RoutesV2.ResourceRoute).MapGet(RoutesV2.File, GetPackFileAsync);

	async Task<IResult> ListAsync([FromServices] ISC5FilePackService svc, CancellationToken ct) => Results.Ok(await svc.ListPacksAsync(ct));
	async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] ISC5FilePackService svc, CancellationToken ct) => Results.Ok(await svc.GetPackAsync(id, ct));
	async Task<IResult> GetPackFileAsync([FromRoute] UniqueObjectId id, [FromServices] ISC5FilePackService svc, CancellationToken ct)
	{
		var (stream, name) = await svc.GetPackFileAsync(id, ct);
		return stream != null ? Results.File(stream, "application/zip", name) : Results.NotFound();
	}
	Task<IResult> CreateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	Task<IResult> UpdateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	Task<IResult> DeleteAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}
