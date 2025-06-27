using ObjectService.RouteHandlers.TableHandlers;

namespace ObjectService.RouteHandlers
{
	// this is purely for the dependency--injection wrapper in Program.cs
	public static class ServerRouteBuilderExtensions
	{
		public static IEndpointConventionBuilder MapServerRoutes(
			this IEndpointRouteBuilder endpoints,
			string routePrefix)
		{
			var routeGroup = endpoints.MapGroup(routePrefix);
			ArgumentNullException.ThrowIfNull(routeGroup);

			AuthorRouteHandler.MapRoutes(routeGroup);
			TagRouteHandler.MapRoutes(routeGroup);
			LicenceRouteHandler.MapRoutes(routeGroup);
			ObjectRouteHandler.MapRoutes(routeGroup);
			ScenarioRouteHandler.MapRoutes(routeGroup);
			SC5FilePackRouteHandler.MapRoutes(routeGroup);
			ObjectPackRouteHandler.MapRoutes(routeGroup);

			// note, you can also write BaseRouteHandler.MapRoutes<ScenarioRequestHandler>(routeGroup)

			return routeGroup;
		}

		public static IEndpointConventionBuilder MapLegacyRoutes(
			this IEndpointRouteBuilder endpoints,
			string routePrefix)
		{
			var routeGroup = endpoints.MapGroup(routePrefix);
			ArgumentNullException.ThrowIfNull(routeGroup);

			LegacyRouteHandler.MapRoutes(routeGroup);
			return routeGroup;
		}

		public static IEndpointConventionBuilder MapAdminRoutes(
			this IEndpointRouteBuilder endpoints,
			string routePrefix)
		{
			var routeGroup = endpoints.MapGroup(routePrefix);
			ArgumentNullException.ThrowIfNull(routeGroup);

			UserRouteHandler.MapRoutes(routeGroup);
			RoleRouteHandler.MapRoutes(routeGroup);


			return routeGroup;
		}
	}
}
