using ObjectService.RouteHandlers.TableHandlers;

namespace OpenLoco.ObjectService
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

			new AuthorRequestHandler().MapRoutes(routeGroup);
			new TagRequestHandler().MapRoutes(routeGroup);
			new LicenceRequestHandler().MapRoutes(routeGroup);
			ObjectRequestHandler.MapRoutes(routeGroup);
			new ScenarioRequestHandler().MapRoutes(routeGroup);
			new SC5FilePackRequestHandler().MapRoutes(routeGroup);
			new ObjectPackRequestHandler().MapRoutes(routeGroup);

			// StringTable
			// DatLookup(?)

			new LegacyRouteHandler().MapRoutes(routeGroup);

			// server/db management
#if DEBUG
			new UserRequestHandler().MapRoutes(routeGroup);
			new RoleRequestHandler().MapRoutes(routeGroup);
#endif

			return routeGroup;
		}
	}
}
