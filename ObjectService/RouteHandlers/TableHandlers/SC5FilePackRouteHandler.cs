using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.SourceData;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class SC5FilePackRouteHandler : ITableRouteHandler
	{
		public static string BaseRoute => RoutesV2.SC5FilePacks;
		public static Delegate ListDelegate => ListAsync;
		public static Delegate CreateDelegate => CreateAsync;
		public static Delegate ReadDelegate => ReadAsync;
		public static Delegate UpdateDelegate => UpdateAsync;
		public static Delegate DeleteDelegate => DeleteAsync;

		public static void MapRoutes(IEndpointRouteBuilder endpoints)
			=> BaseTableRouteHandler.MapRoutes<SC5FilePackRouteHandler>(endpoints);

		public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
		{ }

		public static async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> Results.Ok(
				(await db.SC5FilePacks
					.Include(l => l.Licence)
					.ToListAsync())
				.Select(x => x.ToDtoEntry())
				.OrderBy(x => x.Name));

		public static async Task<IResult> CreateAsync(DtoItemPackDescriptor<DtoScenarioEntry> request, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public static async Task<IResult> ReadAsync(DbKey id, LocoDbContext db)
			=> Results.Ok(
				(await db.SC5FilePacks
					.Where(x => x.Id == id)
					.Include(l => l.Licence)
					.Select(x => new ExpandedTblPack<TblSC5FilePack, TblSC5File>(x, x.SC5Files, x.Authors, x.Tags))
					.ToListAsync())
				.Select(x => x.ToDtoDescriptor())
				.OrderBy(x => x.Name));

		public static async Task<IResult> UpdateAsync(DbKey id, DtoItemPackDescriptor<DtoScenarioEntry> request, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public static async Task<IResult> DeleteAsync(DbKey id, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}
}
