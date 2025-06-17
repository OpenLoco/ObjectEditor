using Definitions.Database.Identity;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.DTO.Identity;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class RoleRequestHandler : BaseReferenceDataTableRequestHandler<DtoRoleEntry, TblUserRole>
	{
		public override string BaseRoute
			=> Routes.Users;

		protected override DbSet<TblUserRole> GetTable(LocoDbContext db)
			=> db.Roles;

		protected override void UpdateFunc(DtoRoleEntry request, TblUserRole row)
			=> row.Name = request.Name;

		protected override TblUserRole ToRowFunc(DtoRoleEntry request)
			=> request.ToTable();

		protected override DtoRoleEntry ToDtoFunc(TblUserRole table)
			=> table.ToDtoEntry();

		protected override bool TryValidateCreate(DtoRoleEntry request, LocoDbContext db, out IResult? result)
		{
			if (string.IsNullOrWhiteSpace(request.Name))
			{
				result = Results.BadRequest("Cannot add an empty or whitespace-only username.");
				return false;
			}

			result = null;
			return true;
		}
	}
}
