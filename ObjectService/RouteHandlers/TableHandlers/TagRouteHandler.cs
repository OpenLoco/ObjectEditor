using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.RouteHandlers.TableHandlers;

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

	public static bool TryValidateCreate([FromBody] DtoTagEntry request, [FromServices] LocoDbContext db, out IResult? result)
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
