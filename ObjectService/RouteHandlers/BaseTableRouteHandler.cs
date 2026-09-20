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

		var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
		_ = resourceRoute.MapGet(string.Empty, handler.ReadDelegate);

		handler.MapAdditionalRoutes(baseRoute);
	}

	/// <summary>
	/// Maps only the write (POST/PUT/DELETE) routes for a handler,
	/// for use when write routes need a different authorization group than read routes.
	/// <paramref name="createPolicy"/> and <paramref name="modifyPolicy"/> are additional policies
	/// applied to the POST and PUT/DELETE endpoints respectively (on top of the parent group's policy).
	/// </summary>
	public static void MapWriteRoutes(
		ITableRouteHandler handler,
		IEndpointRouteBuilder parentRoute,
		IConfiguration config,
		string? createPolicy = null,
		string? modifyPolicy = null)
	{
		var backendReadOnly = config.GetValue<bool?>("ObjectService:BackendReadOnly") ?? false;

		if (backendReadOnly)
		{
			return;
		}

		var baseRoute = parentRoute
			.MapGroup(handler.BaseRoute)
			.WithTags(RouteHelpers.MakeNicePlural(handler.GetType().Name));

		var createEndpoint = baseRoute.MapPost(string.Empty, handler.CreateDelegate);
		if (createPolicy is not null)
		{
			_ = createEndpoint.RequireAuthorization(createPolicy);
		}

		var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);

		var updateEndpoint = resourceRoute.MapPut(string.Empty, handler.UpdateDelegate);
		if (modifyPolicy is not null)
		{
			_ = updateEndpoint.RequireAuthorization(modifyPolicy);
		}

		var deleteEndpoint = resourceRoute.MapDelete(string.Empty, handler.DeleteDelegate);
		if (modifyPolicy is not null)
		{
			_ = deleteEndpoint.RequireAuthorization(modifyPolicy);
		}
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
