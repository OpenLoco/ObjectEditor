using ObjectService.RouteHandlers.TableHandlers;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers
{
	public static class V1RouteBuilderExtensions
	{
		public static IEndpointConventionBuilder MapV1Routes(this IEndpointRouteBuilder endpoints)
		{
			var routeGroup = endpoints.MapGroup(RoutesV1.Prefix);
			ArgumentNullException.ThrowIfNull(routeGroup);

			LegacyRouteHandler.MapRoutes(routeGroup);
			return routeGroup;
		}
	}

	public static class V2RouteBuilderExtensions
	{
		public static IEndpointConventionBuilder MapV2Routes(this IEndpointRouteBuilder endpoints)
		{
			var v2 = endpoints.MapGroup(RoutesV2.Prefix);
			_ = v2.MapServerRoutes();

#if DEBUG
			// not ready for prime time yet
			_ = v2.MapAdminRoutes().RequireAuthorization();
#endif

			return v2;
		}

		static IEndpointConventionBuilder MapServerRoutes(this IEndpointRouteBuilder endpoints)
		{
			var routeGroup = endpoints.MapGroup(string.Empty);
			ArgumentNullException.ThrowIfNull(routeGroup);

			AuthorRouteHandler.MapRoutes(routeGroup);
			TagRouteHandler.MapRoutes(routeGroup);
			LicenceRouteHandler.MapRoutes(routeGroup);
			ObjectRouteHandler.MapRoutes(routeGroup);
			ScenarioRouteHandler.MapRoutes(routeGroup);
			SC5FilePackRouteHandler.MapRoutes(routeGroup);
			ObjectPackRouteHandler.MapRoutes(routeGroup);

			return routeGroup;
		}

		static IEndpointConventionBuilder MapAdminRoutes(this IEndpointRouteBuilder endpoints)
		{
			var routeGroup = endpoints.MapGroup(string.Empty);
			ArgumentNullException.ThrowIfNull(routeGroup);

			UserRouteHandler.MapRoutes(routeGroup);
			RoleRouteHandler.MapRoutes(routeGroup);

			return routeGroup;
		}
	}
}
