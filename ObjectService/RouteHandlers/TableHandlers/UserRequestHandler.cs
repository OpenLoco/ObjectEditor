using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.DTO.Identity;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class UserRequestHandler : BaseReferenceDataTableRequestHandler<DtoUserEntry, TblUser>
	{
		public override string BaseRoute
			=> Routes.Users;

		protected override DbSet<TblUser> GetTable(LocoDbContext db)
			=> db.Users;

		protected override void UpdateFunc(DtoUserEntry request, TblUser row)
			=> row.UserName = request.UserName;

		protected override TblUser ToRowFunc(DtoUserEntry request)
			=> request.ToTable();

		protected override DtoUserEntry ToDtoFunc(TblUser table)
			=> table.ToDtoEntry();

		protected override bool TryValidateCreate(DtoUserEntry request, LocoDbContext db, out IResult? result)
		{
			if (string.IsNullOrWhiteSpace(request.UserName))
			{
				result = Results.BadRequest("Cannot add an empty or whitespace-only username.");
				return false;
			}

			result = null;
			return true;
		}
	}
}
