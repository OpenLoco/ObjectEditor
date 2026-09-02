using Definitions.Web;

namespace ObjectService.RouteHandlers;

public static class BaseTableRouteHandler
{
	public static void MapRoutes(
	ITableRouteHandler handler,
	IEndpointRouteBuilder parentRoute,
	IConfiguration config)
	{
		var baseRoute = parentRoute
		.MapGroup(handler.BaseRoute)
		.WithTags(RouteHelpers.MakeNicePlural(handler.GetType().Name));

		_ = baseRoute.MapGet(string.Empty, handler.ListDelegate);

		var resourceRoute = baseRoute.MapGroup(RoutesV2.ResourceRoute);
		_ = resourceRoute.MapGet(string.Empty, handler.ReadDelegate);

		var enableWriteRoutes = config.GetValue<bool?>("ObjectService:EnableWriteRoutes") ?? false;

		if (enableWriteRoutes)
		{
			_ = baseRoute.MapPost(string.Empty, handler.CreateDelegate);
			_ = resourceRoute.MapPut(string.Empty, handler.UpdateDelegate);
			_ = resourceRoute.MapDelete(string.Empty, handler.DeleteDelegate);
		}

		handler.MapAdditionalRoutes(baseRoute);
	}
}
