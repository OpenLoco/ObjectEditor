using Definitions;
using OpenLoco.Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public abstract class BaseReferenceDataTableRequestHandler<THandler, TDto, TRow> : ITableRequestHandler<TDto>
		where TDto : class, IHasId
		where TRow : class, IHasId
		where THandler : ITableRequestConfig<TDto, TRow>
	{
		public static async Task<IResult> CreateAsync(TDto request, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.CreateAsync(
				THandler.GetTable(db),
				THandler.ToDtoFunc,
				THandler.ToRowFunc,
				request,
				() => (THandler.TryValidateCreate(request, db, out var result), result),
				THandler.GetBaseRoute(),
				db);

		public static async Task<IResult> ReadAsync(DbKey id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ReadAsync(THandler.GetTable(db), THandler.ToDtoFunc, id, db);

		public static async Task<IResult> UpdateAsync(DbKey id, TDto request, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.UpdateAsync(
				THandler.GetTable(db),
				THandler.ToDtoFunc,
				THandler.ToRowFunc,
				request,
				() => (THandler.TryValidateCreate(request, db, out var result), result),
				THandler.GetBaseRoute(),
				id,
				db,
				THandler.UpdateFunc);

		public static async Task<IResult> DeleteAsync(DbKey id, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.DeleteAsync(THandler.GetTable(db), THandler.ToDtoFunc, id, db);

		public static async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> await BaseReferenceDataTableRequestHandlerImpl.ListAsync(context, THandler.GetTable(db), THandler.ToDtoFunc);

	}
}
