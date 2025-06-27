using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers
{
	public static class BaseTableRouteHandler
	{
		public static void MapRoutes<THandler>(
			IEndpointRouteBuilder parentRoute
			) where THandler : ITableRouteHandler
		{
			var baseRoute = parentRoute
				.MapGroup(THandler.BaseRoute)
				.WithTags(RouteHelpers.MakeNicePlural(typeof(THandler).Name));

			_ = baseRoute.MapGet(string.Empty, THandler.ListDelegate);

			var resourceRoute = baseRoute.MapGroup(RoutesV2.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, THandler.ReadDelegate);

#if DEBUG
			_ = baseRoute.MapPost(string.Empty, THandler.CreateDelegate); //.RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapPut(string.Empty, THandler.UpdateDelegate); //.RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapDelete(string.Empty, THandler.DeleteDelegate); //.RequireAuthorization(AdminPolicy.Name);
#endif
			THandler.MapAdditionalRoutes(baseRoute);
		}
	}
}
