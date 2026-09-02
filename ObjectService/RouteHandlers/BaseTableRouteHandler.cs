using Definitions.Web;

namespace ObjectService.RouteHandlers;

public static class BaseTableRouteHandler
{
	/// <summary>
	/// Maps only the read (GET) routes (list + get-by-id) for a handler.
	/// Use this for publicly-accessible read routes.
	/// </summary>
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

		handler.MapAdditionalRoutes(baseRoute);
	}

	/// <summary>
	/// Maps only the write (POST/PUT/DELETE) routes for a handler,
	/// for use when write routes need a different authorization group than read routes.
	/// </summary>
	public static void MapWriteRoutes(
		ITableRouteHandler handler,
		IEndpointRouteBuilder parentRoute,
		IConfiguration config)
	{
		var enableWriteRoutes = config.GetValue<bool?>("ObjectService:EnableWriteRoutes") ?? false;

		if (!enableWriteRoutes)
		{
			return;
		}

		var baseRoute = parentRoute
			.MapGroup(handler.BaseRoute)
			.WithTags(RouteHelpers.MakeNicePlural(handler.GetType().Name));

		_ = baseRoute.MapPost(string.Empty, handler.CreateDelegate);

		var resourceRoute = baseRoute.MapGroup(RoutesV2.ResourceRoute);
		_ = resourceRoute.MapPut(string.Empty, handler.UpdateDelegate);
		_ = resourceRoute.MapDelete(string.Empty, handler.DeleteDelegate);
	}

	/// <summary>
	/// Maps all routes (both read and write) for a handler.
	/// Use this for handlers where read and write share the same authorization level (e.g. admin-only).
	/// </summary>
	public static void MapAllRoutes(
		ITableRouteHandler handler,
		IEndpointRouteBuilder parentRoute,
		IConfiguration config)
	{
		MapRoutes(handler, parentRoute, config);
		MapWriteRoutes(handler, parentRoute, config);
	}
}
