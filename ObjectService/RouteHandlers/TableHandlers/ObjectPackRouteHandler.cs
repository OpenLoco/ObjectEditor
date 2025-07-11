using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.SourceData;
using Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectPackRouteHandler : ITableRouteHandler
{
	public static string BaseRoute => RoutesV2.ObjectPacks;
	public static Delegate ListDelegate => ListAsync;
	public static Delegate CreateDelegate => CreateAsync;
	public static Delegate ReadDelegate => ReadAsync;
	public static Delegate UpdateDelegate => UpdateAsync;
	public static Delegate DeleteDelegate => DeleteAsync;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<ObjectPackRouteHandler>(endpoints);

	public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
	{ }

	public static async Task<IResult> ListAsync(HttpContext context, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.ObjectPacks
				.Include(l => l.Licence)
				.ToListAsync())
			.Select(x => x.ToDtoEntry())
			.OrderBy(x => x.Name));

	public static async Task<IResult> CreateAsync([FromBody] DtoItemPackDescriptor<DtoObjectEntry> request, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.ObjectPacks
				.Where(x => x.Id == id)
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblObjectPack, TblObject>(x, x.Objects, x.Authors, x.Tags))
				.ToListAsync())
			.Select(x => x.ToDtoDescriptor())
			.OrderBy(x => x.Name));

	public static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoItemPackDescriptor<DtoObjectEntry> request, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}
