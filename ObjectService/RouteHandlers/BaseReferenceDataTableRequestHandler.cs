using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public abstract class BaseReferenceDataTableRequestHandler<TDto, TRow> : BaseTableRequestHandler<TDto>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		protected abstract DbSet<TRow> GetTable(LocoDbContext db);
		protected abstract TRow ToRowFunc(TDto request);
		protected abstract TDto ToDtoFunc(TRow request);
		protected abstract void UpdateFunc(TDto request, TRow row);

		protected abstract bool TryValidateCreate(TDto request, LocoDbContext db, out IResult? result);

		public override async Task<IResult> CreateAsync(TDto request, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.CreateAsync(
				GetTable(db),
				ToDtoFunc,
				ToRowFunc,
				request,
				() => (TryValidateCreate(request, db, out var result), result),
				BaseRoute,
				db);

		public override async Task<IResult> ReadAsync(DbKey id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ReadAsync(GetTable(db), ToDtoFunc, id, db);

		public override async Task<IResult> UpdateAsync(DbKey id, TDto request, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.UpdateAsync(
				GetTable(db),
				ToDtoFunc,
				ToRowFunc,
				request,
				() => (TryValidateCreate(request, db, out var result), result),
				BaseRoute,
				id,
				db,
				UpdateFunc);

		public override async Task<IResult> DeleteAsync(DbKey id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.DeleteAsync(GetTable(db), ToDtoFunc, id, db);

		public override async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ListAsync(context, GetTable(db), ToDtoFunc);
	}

	//[Tags("RoleRequestHandler")]
	//public class RoleRequestHandler : BaseReferenceDataTableRequestHandler<DtoRoleCreate, TblUserRole>
	//{
	//	public override string BaseRoute
	//		=> Routes.Roles;

	//	protected override DbSet<TblUserRole> GetTable(LocoDbContext db)
	//		=> db.Roles;

	//	protected override void UpdateFunc(DtoRoleCreate request, TblUserRole row)
	//		=> row.Name = request.Name;

	//	protected override TblUserRole ToRowFunc(DtoRoleCreate request)
	//		=> request.ToTable();

	//	protected override DtoRoleCreate ToDtoFunc(TblUserRole table)
	//		=> table.ToDtoEntry();

	//	protected override bool TryValidateCreate(DtoRoleCreate request, LocoDbContext db, out IResult? result)
	//	{
	//		if (string.IsNullOrWhiteSpace(request.Name))
	//		{
	//			result = Results.BadRequest("Cannot add an empty or whitespace-only name.");
	//			return false;
	//		}

	//		result = null;
	//		return true;
	//	}
	//}
}
