using Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers
{
	public abstract class BaseTableRequestHandler<TDto> : ITableRequestHandler<TDto>
		where TDto : class, IHasId
	{
		public abstract string BaseRoute { get; }

		public void MapRoutes(IEndpointRouteBuilder parentRoute)
		{
			var baseRoute = parentRoute
				.MapGroup(BaseRoute)
				.WithTags(MakeNicePlural(GetType().Name));

			_ = baseRoute.MapGet(string.Empty, ListAsync);

			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, ReadAsync);

			// todo: do not enable until user permissions are implemented. for now, enable for testing
#if DEBUG
			_ = baseRoute.MapPost(string.Empty, CreateAsync);
			_ = resourceRoute.MapPut(string.Empty, UpdateAsync);
			_ = resourceRoute.MapDelete(string.Empty, DeleteAsync);
#endif

			MapAdditionalRoutes(baseRoute);

			static string MakeNicePlural(string name)
				=> $"{name.Replace("RequestHandler", string.Empty)}s";
		}

		public virtual void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) { }
		public abstract Task<IResult> CreateAsync(TDto request, LocoDbContext db);
		public abstract Task<IResult> ReadAsync(int id, LocoDbContext db);
		public abstract Task<IResult> UpdateAsync(int id, TDto request, LocoDbContext db);
		public abstract Task<IResult> DeleteAsync(int id, LocoDbContext db);
		public abstract Task<IResult> ListAsync(HttpContext context, LocoDbContext db);
	}
}
