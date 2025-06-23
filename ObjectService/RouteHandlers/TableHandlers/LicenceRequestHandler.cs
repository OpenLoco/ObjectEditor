using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	[Tags("LicenceRouteHandler")]
	public class LicenceRouteHandler
		: BaseReferenceDataTableRequestHandler<LicenceRouteHandler, DtoLicenceEntry, TblLicence>
		, ITableRequestConfig<DtoLicenceEntry, TblLicence>
	{
		public static string GetBaseRoute()
			=> Routes.Licences;

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
