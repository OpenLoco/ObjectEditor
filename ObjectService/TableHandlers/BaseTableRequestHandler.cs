using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

namespace ObjectService.TableHandlers
{
	public abstract class BaseTableRequestHandler<TDto, TRow>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		public void MapRoutes(RouteGroupBuilder routeGroup)
		{
			_ = routeGroup.MapPost(Routes.MakePostRoute(BaseRoute), CreateAsync);
			_ = routeGroup.MapGet(Routes.MakeGetRoute(BaseRoute), ReadAsync);
			_ = routeGroup.MapPut(Routes.MakePutRoute(BaseRoute), UpdateAsync);
			_ = routeGroup.MapDelete(Routes.MakeDeleteRoute(BaseRoute), DeleteAsync);
			_ = routeGroup.MapGet(Routes.MakeListRoute(BaseRoute), ListAsync);
		}

		protected abstract DbSet<TRow> GetTable(LocoDb db);

		protected abstract string BaseRoute { get; }

		protected abstract void UpdateFunc(TDto request, TRow row);

		protected abstract TRow ToRowFunc(TDto request);
		protected abstract TDto ToDtoFunc(TRow request);

		protected abstract bool TryValidateCreate(TDto request, LocoDb db, out IResult result);

		public virtual async Task<IResult> CreateAsync(TDto request, LocoDb db)
			=> !TryValidateCreate(request, db, out var result)
				? result
				: await TableRequestHandlerImpl.CreateAsync(request, GetTable(db), BaseRoute, ToRowFunc, ToDtoFunc, db);

		public virtual async Task<IResult> ReadAsync(int id, LocoDb db)
			=> await TableRequestHandlerImpl.ReadById(id, GetTable(db), ToDtoFunc, db);

		public virtual async Task<IResult> UpdateAsync(TDto request, LocoDb db)
			=> await TableRequestHandlerImpl.UpdateById(request, GetTable(db), UpdateFunc, db);

		public virtual async Task<IResult> DeleteAsync(int id, LocoDb db)
			=> await TableRequestHandlerImpl.DeleteById(id, GetTable(db), db);

		public virtual async Task<IResult> ListAsync(LocoDb db)
			=> await TableRequestHandlerImpl.List(GetTable(db), ToDtoFunc, db);
	}
}
