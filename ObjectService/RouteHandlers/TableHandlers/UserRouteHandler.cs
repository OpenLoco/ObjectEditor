using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO.Identity;
using OpenLoco.Definitions.DTO.Mappers;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class UserRouteHandler
		: BaseDataTableRouteHandler<UserRouteHandler, DtoUserEntry, TblUser>
		, ITableRouteConfig<DtoUserEntry, TblUser>
	{
		public static string GetBaseRoute()
			=> RoutesV2.Users;

		public static void MapRoutes(IEndpointRouteBuilder endpoints)
			=> BaseTableRouteHandler.MapRoutes<UserRouteHandler>(endpoints);

		public static DbSet<TblUser> GetTable(LocoDbContext db)
			=> db.Users;

		public static DtoUserEntry ToDtoFunc(TblUser table)
			=> table.ToDtoEntry();

		public static void UpdateFunc(DtoUserEntry request, TblUser row)
			=> row.UserName = request.UserName;

		public static TblUser ToRowFunc(DtoUserEntry request)
			=> request.ToTable();

		public static bool TryValidateCreate(DtoUserEntry request, LocoDbContext db, out IResult? result)
		{
			if (string.IsNullOrWhiteSpace(request.UserName))
			{
				result = Results.BadRequest("Cannot add an empty or whitespace-only name.");
				return false;
			}
			result = null;
			return true;
		}
	}
}
