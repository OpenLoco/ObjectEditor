using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO.Identity;
using Definitions.DTO.Mappers;
using Definitions.Web;

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

		public static bool TryValidateCreate([FromBody] DtoUserEntry request, [FromServices] LocoDbContext db, out IResult? result)
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
