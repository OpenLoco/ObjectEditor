using Definitions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.RouteHandlers;

public interface ITableRouteConfig<TContext, TDto, TRow>
	where TContext : DbContext
	where TDto : class, IHasId
	where TRow : class, IHasId
{
	static abstract string GetBaseRoute();
	static abstract DbSet<TRow> GetTable(TContext db);
	static abstract TRow ToRowFunc(TDto request);
	static abstract TDto ToDtoFunc(TRow request);
	static abstract void UpdateFunc(TDto request, TRow row);

	static abstract bool TryValidateCreate(TDto request, [FromServices] TContext db, out IResult? result);
}
