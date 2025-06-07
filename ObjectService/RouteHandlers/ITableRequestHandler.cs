using Definitions;
using OpenLoco.Definitions.Database;

namespace ObjectService.RouteHandlers
{
	public interface ITableRequestHandler<TDto> where TDto : class, IHasId
	{
		string BaseRoute { get; }

		void MapRoutes(IEndpointRouteBuilder parentRoute);
		void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute);

		Task<IResult> CreateAsync(TDto request, LocoDbContext db);

		Task<IResult> ReadAsync(int id, LocoDbContext db);

		Task<IResult> UpdateAsync(int id, TDto request, LocoDbContext db);

		Task<IResult> DeleteAsync(int id, LocoDbContext db);

		Task<IResult> ListAsync(HttpContext context, LocoDbContext db);
	}
}
