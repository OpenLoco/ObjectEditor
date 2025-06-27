using Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers
{
	public abstract class BaseRouteHandler<THandler>
	{
		public abstract string BaseRoute { get; }
		public abstract Delegate ListDelegate { get; }
		public abstract Delegate CreateDelegate { get; }
		public abstract Delegate ReadDelegate { get; }
		public abstract Delegate UpdateDelegate { get; }
		public abstract Delegate DeleteDelegate { get; }

		protected virtual void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) { }

		public void MapRoutes(IEndpointRouteBuilder parentRoute)
		{
			var baseRoute = parentRoute
				.MapGroup(BaseRoute)
				.WithTags(RouteHelpers.MakeNicePlural(typeof(THandler).Name));

			_ = baseRoute.MapGet(string.Empty, ListDelegate);

			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, ReadDelegate);

#if DEBUG
			_ = baseRoute.MapPost(string.Empty, CreateDelegate); //.RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapPut(string.Empty, UpdateDelegate); //.RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapDelete(string.Empty, DeleteDelegate); //.RequireAuthorization(AdminPolicy.Name);
#endif
			MapAdditionalRoutes(parentRoute);
		}
	}

	public abstract class BaseTableRequestHandler<TDto> : ITableRequestHandler<TDto>
		where TDto : class, IHasId
	{
		public abstract string BaseRoute { get; }

		public virtual void MapRoutes(IEndpointRouteBuilder parentRoute)
		{
			var baseRoute = parentRoute
				.MapGroup(BaseRoute)
				.WithTags(MakeNicePlural(GetType().Name));

			_ = baseRoute.MapGet(string.Empty, ListAsync);

			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, ReadAsync);

#if DEBUG
			_ = baseRoute.MapPost(string.Empty, CreateAsync); //.RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapPut(string.Empty, UpdateAsync); //.RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapDelete(string.Empty, DeleteAsync); //.RequireAuthorization(AdminPolicy.Name);
#endif

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
