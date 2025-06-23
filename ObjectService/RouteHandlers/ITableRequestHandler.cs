using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public interface ITableRequestHandler<TDto>
		where TDto : class, IHasId
	{
		//static abstract string BaseRoute { get; }

		//static abstract void MapRoutes(IEndpointRouteBuilder parentRoute);

		static virtual void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) { }

		static abstract Task<IResult> CreateAsync(TDto request, LocoDbContext db);

		static abstract Task<IResult> ReadAsync(DbKey id, LocoDbContext db);

		static abstract Task<IResult> UpdateAsync(DbKey id, TDto request, LocoDbContext db);

		static abstract Task<IResult> DeleteAsync(DbKey id, LocoDbContext db);

		static abstract Task<IResult> ListAsync(HttpContext context, LocoDbContext db);
	}

	public interface ITableRequestConfig<TDto, TRow>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		static abstract string GetBaseRoute();
		static abstract DbSet<TRow> GetTable(LocoDbContext db);
		static abstract TRow ToRowFunc(TDto request);
		static abstract TDto ToDtoFunc(TRow request);
		static abstract void UpdateFunc(TDto request, TRow row);

		static abstract bool TryValidateCreate(TDto request, LocoDbContext db, out IResult? result);
	}
}
