using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
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
			=> row.Name = request.Name;

		public static TblLicence ToRowFunc(DtoLicenceEntry request)
			=> request.ToTable();

		public static bool TryValidateCreate(DtoLicenceEntry request, LocoDbContext db, out IResult? result)
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
