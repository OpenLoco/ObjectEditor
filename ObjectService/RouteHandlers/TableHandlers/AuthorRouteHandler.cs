using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers
{
	public class AuthorRouteHandler
		: BaseDataTableRouteHandler<AuthorRouteHandler, DtoAuthorEntry, TblAuthor>
		, ITableRouteConfig<DtoAuthorEntry, TblAuthor>
	{
		public static string GetBaseRoute()
			=> RoutesV2.Authors;

		public static void MapRoutes(IEndpointRouteBuilder endpoints)
			=> BaseTableRouteHandler.MapRoutes<AuthorRouteHandler>(endpoints);

		public static DbSet<TblAuthor> GetTable(LocoDbContext db)
			=> db.Authors;

		public static DtoAuthorEntry ToDtoFunc(TblAuthor table)
			=> table.ToDtoEntry();

		public static void UpdateFunc(DtoAuthorEntry request, TblAuthor row)
			=> row.Name = request.Name;

		public static TblAuthor ToRowFunc(DtoAuthorEntry request)
			=> request.ToTable();

		public static bool TryValidateCreate(DtoAuthorEntry request, LocoDbContext db, out IResult? result)
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
