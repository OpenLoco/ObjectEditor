using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;
using System.Security.Claims;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectPackRouteHandler : ITableRouteHandler
{
public string BaseRoute => RoutesV2.ObjectPacks;
public Delegate ListDelegate => ListAsync;
public Delegate CreateDelegate => CreateAsync;
public Delegate ReadDelegate => ReadAsync;
public Delegate UpdateDelegate => UpdateAsync;
public Delegate DeleteDelegate => DeleteAsync;
public void MapRoutes(IEndpointRouteBuilder e) => BaseTableRouteHandler.MapRoutes(this, e, e.ServiceProvider.GetRequiredService<IConfiguration>());
public void MapAdditionalRoutes(IEndpointRouteBuilder p) => p.MapGroup(RoutesV2.ResourceRoute).MapGet(RoutesV2.File, GetPackFileAsync);

async Task<IResult> ListAsync([FromServices] IObjectPackService svc, CancellationToken ct) => Results.Ok(await svc.ListPacksAsync(ct));
async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] IObjectPackService svc, CancellationToken ct) => Results.Ok(await svc.GetPackAsync(id, ct));
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
var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
{
return Results.Unauthorized();
}

var created = await svc.CreatePackAsync(request, userId, ct);
return Results.Created($"{RoutesV2.Prefix}{BaseRoute}/{created.Id}", created);
}

Task<IResult> UpdateAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
Task<IResult> DeleteAsync() => Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}