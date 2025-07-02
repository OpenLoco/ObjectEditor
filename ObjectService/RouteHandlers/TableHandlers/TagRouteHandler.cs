using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.DTO.Mappers;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class TagRouteHandler
		: BaseDataTableRouteHandler<TagRouteHandler, DtoTagEntry, TblTag>
		, ITableRouteConfig<DtoTagEntry, TblTag>
	{
		public static string GetBaseRoute()
			=> RoutesV2.Tags;

		public static void MapRoutes(IEndpointRouteBuilder endpoints)
			=> BaseTableRouteHandler.MapRoutes<TagRouteHandler>(endpoints);

		public static DbSet<TblTag> GetTable(LocoDbContext db)
			=> db.Tags;

		public static DtoTagEntry ToDtoFunc(TblTag table)
			=> table.ToDtoEntry();

		public static void UpdateFunc(DtoTagEntry request, TblTag row)
			=> row.Name = request.Name;

		public static TblTag ToRowFunc(DtoTagEntry request)
			=> request.ToTable();

		public static bool TryValidateCreate(DtoTagEntry request, LocoDbContext db, out IResult? result)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				result = Results.BadRequest("Cannot add an empty or whitespace-only name.");
				return false;
			}
			result = null;
			return true;
		}
	}
}
