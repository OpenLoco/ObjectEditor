using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers;

public class LicenceRouteHandler
	: BaseDataTableRouteHandler<LicenceRouteHandler, DtoLicenceEntry, TblLicence>
	, ITableRouteConfig<DtoLicenceEntry, TblLicence>
{
	public static string GetBaseRoute()
		=> RoutesV2.Licences;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<LicenceRouteHandler>(endpoints);

	public static DbSet<TblLicence> GetTable(LocoDbContext db)
		=> db.Licences;

	public static DtoLicenceEntry ToDtoFunc(TblLicence table)
		=> table.ToDtoEntry();

	public static void UpdateFunc(DtoLicenceEntry request, TblLicence row)
	{
		row.Name = request.Name;
		row.Text = request.Text;
	}

	public static TblLicence ToRowFunc(DtoLicenceEntry request)
		=> request.ToTable();

	public static bool TryValidateCreate([FromBody] DtoLicenceEntry request, [FromServices] LocoDbContext db, out IResult? result)
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
