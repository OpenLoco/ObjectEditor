using Definitions;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

public class CrudRouteHandler<TDto, TRow> : ITableRouteHandler
	where TDto : class, IHasId
	where TRow : class, IHasId
{
	private readonly string _baseRoute;

	public CrudRouteHandler(string baseRoute)
	{
		_baseRoute = baseRoute;
		BaseRoute = baseRoute;
	}

	public string BaseRoute { get; }

	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes(this, endpoints,
			endpoints.ServiceProvider.GetRequiredService<IConfiguration>());

	public void MapAdditionalRoutes(IEndpointRouteBuilder endpoints) { }

	async Task<IResult> ListAsync(HttpContext context, [FromServices] ICrudService<TDto, TRow> service, CancellationToken ct)
	{
		var items = await service.ListAsync(context, ct);
		return Results.Ok(items);
	}

	async Task<IResult> CreateAsync([FromBody] TDto request, [FromServices] ICrudService<TDto, TRow> service, CancellationToken ct)
	{
		if (!service.TryValidateCreate(request, out var error))
		{
			return Results.Problem(error, statusCode: StatusCodes.Status400BadRequest);
		}

		var dto = await service.CreateAsync(request, ct);
		return Results.Created($"{_baseRoute}/{dto.Id}", dto);
	}

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] ICrudService<TDto, TRow> service, CancellationToken ct)
	{
		var dto = await service.ReadAsync(id, ct);
		return dto != null ? Results.Ok(dto) : Results.NotFound();
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] TDto request, [FromServices] ICrudService<TDto, TRow> service, CancellationToken ct)
	{
		var dto = await service.UpdateAsync(id, request, ct);
		return dto != null ? Results.Accepted($"{_baseRoute}/{dto.Id}", dto) : Results.NotFound();
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] ICrudService<TDto, TRow> service, CancellationToken ct)
	{
		var deleted = await service.DeleteAsync(id, ct);
		return deleted ? Results.Ok() : Results.NotFound();
	}
}
