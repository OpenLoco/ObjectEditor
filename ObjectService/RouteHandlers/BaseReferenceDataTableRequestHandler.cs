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

		public override async Task<IResult> ReadAsync(int id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ReadAsync(GetTable(db), ToDtoFunc, id, db);

		public override async Task<IResult> UpdateAsync(int id, TDto request, LocoDbContext db)
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

		public override async Task<IResult> DeleteAsync(int id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.DeleteAsync(GetTable(db), ToDtoFunc, id, db);

		public override async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ListAsync(context, GetTable(db), ToDtoFunc);
	}
}
