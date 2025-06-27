using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Definitions.Database;

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

		static abstract bool TryValidateCreate(TDto request, LocoDbContext db, out IResult? result);
	}
}
