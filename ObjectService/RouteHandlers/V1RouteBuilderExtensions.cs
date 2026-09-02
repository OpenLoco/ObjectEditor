using Definitions.Web;
using ObjectService.RouteHandlers.TableHandlers;

namespace ObjectService.RouteHandlers;

public static class RouteBuilderExtensions
{
	public static IEndpointConventionBuilder MapApiRoutes(this IEndpointRouteBuilder endpoints)
	{
		var v2 = endpoints.MapGroup(RoutesV2.Prefix);
		var config = endpoints.ServiceProvider.GetRequiredService<IConfiguration>();

		var serverGroup = v2.MapGroup(string.Empty);
		MapHandler(new AuthorRouteHandler(), serverGroup, config);
		MapHandler(new TagRouteHandler(), serverGroup, config);
		MapHandler(new LicenceRouteHandler(), serverGroup, config);
		MapHandler(new ObjectRouteHandler(), serverGroup, config);
		MapHandler(new ObjectMissingRouteHandler(), serverGroup, config);
		MapHandler(new ScenarioRouteHandler(), serverGroup, config);
		MapHandler(new SC5FilePackRouteHandler(), serverGroup, config);
		MapHandler(new ObjectPackRouteHandler(), serverGroup, config);

		var adminGroup = v2.MapGroup(string.Empty).RequireAuthorization();
		MapHandler(new UserRouteHandler(), adminGroup, config);
		MapHandler(new RoleRouteHandler(), adminGroup, config);

		return v2;
	}

	private static void MapHandler(ITableRouteHandler handler, IEndpointRouteBuilder group, IConfiguration config)
		=> BaseTableRouteHandler.MapRoutes(handler, group, config);
}
