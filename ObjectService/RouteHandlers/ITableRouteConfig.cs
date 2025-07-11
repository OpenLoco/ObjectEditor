using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Definitions;
using Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public interface ITableRouteConfig<TDto, TRow>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		static abstract string GetBaseRoute();
		static abstract DbSet<TRow> GetTable(LocoDbContext db);
		static abstract TRow ToRowFunc(TDto request);
		static abstract TDto ToDtoFunc(TRow request);
		static abstract void UpdateFunc(TDto request, TRow row);

		static abstract bool TryValidateCreate(TDto request, [FromServices] LocoDbContext db, out IResult? result);
	}
}
