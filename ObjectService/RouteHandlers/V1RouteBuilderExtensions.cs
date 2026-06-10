using Definitions.Web;
using ObjectService.RouteHandlers.TableHandlers;

namespace ObjectService.RouteHandlers;

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
