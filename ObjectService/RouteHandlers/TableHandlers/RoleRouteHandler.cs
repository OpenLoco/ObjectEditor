using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO.Identity;
using Definitions.DTO.Mappers;
using Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers;

public class RoleRouteHandler
	: BaseDataTableRouteHandler<RoleRouteHandler, DtoRoleEntry, TblUserRole>
	, ITableRouteConfig<DtoRoleEntry, TblUserRole>
{
	public static string GetBaseRoute()
		=> RoutesV2.Roles;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<RoleRouteHandler>(endpoints);

	public static DbSet<TblUserRole> GetTable(LocoDbContext db)
		=> db.Roles;

	public static DtoRoleEntry ToDtoFunc(TblUserRole table)
		=> table.ToDtoEntry();

	public static void UpdateFunc(DtoRoleEntry request, TblUserRole row)
		=> row.Name = request.Name;

	public static TblUserRole ToRowFunc(DtoRoleEntry request)
		=> request.ToTable();

	public static bool TryValidateCreate(DtoRoleEntry request, [FromServices] LocoDbContext db, out IResult? result)
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
