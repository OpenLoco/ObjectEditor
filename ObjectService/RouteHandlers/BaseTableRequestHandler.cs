using Definitions;
using Definitions.Database.Identity;
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

			_ = baseRoute.MapPost(string.Empty, CreateAsync).RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapPut(string.Empty, UpdateAsync).RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapDelete(string.Empty, DeleteAsync).RequireAuthorization(AdminPolicy.Name);

			MapAdditionalRoutes(baseRoute);

			static string MakeNicePlural(string name)
				=> $"{name.Replace("RequestHandler", string.Empty)}s";
		}

		public virtual void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) { }
		public abstract Task<IResult> CreateAsync(TDto request, LocoDbContext db);
		public abstract Task<IResult> ReadAsync(DbKey id, LocoDbContext db);
		public abstract Task<IResult> UpdateAsync(DbKey id, TDto request, LocoDbContext db);
		public abstract Task<IResult> DeleteAsync(DbKey id, LocoDbContext db);
		public abstract Task<IResult> ListAsync(HttpContext context, LocoDbContext db);
	}
}
