using Common;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectMissingRouteHandler
	: BaseDataTableRouteHandler<ObjectMissingRouteHandler, DtoObjectMissingEntry, TblObjectMissing>
	, ITableRouteConfig<DtoObjectMissingEntry, TblObjectMissing>
{
	public static string GetBaseRoute()
	=> RoutesV2.Missing;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<ObjectMissingRouteHandler>(endpoints);

	public static DbSet<TblObjectMissing> GetTable(LocoDbContext db)
		=> db.ObjectsMissing;

	public static DtoObjectMissingEntry ToDtoFunc(TblObjectMissing table)
		=> table.ToDtoEntry();

	public static void UpdateFunc(DtoObjectMissingEntry request, TblObjectMissing row)
	{
		row.DatName = request.DatName;
		row.DatChecksum = request.DatChecksum;
		row.ObjectType = request.ObjectType;
	}

	public static TblObjectMissing ToRowFunc(DtoObjectMissingEntry request)
		=> request.ToTable();

	public static bool TryValidateCreate([FromBody] DtoObjectMissingEntry request, [FromServices] LocoDbContext db, out IResult? result)
	{
		if (string.IsNullOrWhiteSpace(request.DatName))
		{
			result = Results.BadRequest("Cannot add an empty or whitespace-only DatName.");
			return false;
		}

		if (request.DatChecksum == 0)
		{
			result = Results.BadRequest("Cannot set a DatChecksum to 0");
			return false;
		}

		if (!Enum.IsDefined(request.ObjectType))
		{
			result = Results.BadRequest($"Invalid ObjectType: {request.ObjectType}");
			return false;
		}

		result = null;
		return true;
	}
}
