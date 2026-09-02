using Definitions.Web;
using ObjectService.RouteHandlers.TableHandlers;

namespace ObjectService.RouteHandlers;

public static class RouteBuilderExtensions
{
	public static IEndpointConventionBuilder MapApiRoutes(this IEndpointRouteBuilder endpoints)
	{
		var v2 = endpoints.MapGroup(RoutesV2.Prefix);
		var config = endpoints.ServiceProvider.GetRequiredService<IConfiguration>();

		// Public read-only routes (guest + authenticated can read)
		var publicGroup = v2.MapGroup(string.Empty);
		MapHandler(new AuthorRouteHandler(), publicGroup, config);
		MapHandler(new TagRouteHandler(), publicGroup, config);
		MapHandler(new LicenceRouteHandler(), publicGroup, config);
		MapHandler(new ObjectRouteHandler(), publicGroup, config);
		MapHandler(new ObjectMissingRouteHandler(), publicGroup, config);
		MapHandler(new ScenarioRouteHandler(), publicGroup, config);
		MapHandler(new SC5FilePackRouteHandler(), publicGroup, config);
		MapHandler(new ObjectPackRouteHandler(), publicGroup, config);

		// Authenticated write routes for reference data (any authenticated user)
		var authGroup = v2.MapGroup(string.Empty).RequireAuthorization();
		MapWriteHandler(new AuthorRouteHandler(), authGroup, config);
		MapWriteHandler(new TagRouteHandler(), authGroup, config);
		MapWriteHandler(new LicenceRouteHandler(), authGroup, config);
		MapWriteHandler(new ObjectMissingRouteHandler(), authGroup, config);
		MapWriteHandler(new ScenarioRouteHandler(), authGroup, config);
		MapWriteHandler(new SC5FilePackRouteHandler(), authGroup, config);
		MapWriteHandler(new ObjectPackRouteHandler(), authGroup, config);

		// Object write routes (create/edit/delete require ownership or admin)
		var ownerGroup = v2.MapGroup(string.Empty).RequireAuthorization("CanEditObject");
		MapWriteHandler(new ObjectRouteHandler(), ownerGroup, config);

		// Admin-only routes (both read and write)
		var adminGroup = v2.MapGroup(string.Empty).RequireAuthorization("AdminOnly");
		MapAllHandler(new UserRouteHandler(), adminGroup, config);
		MapAllHandler(new RoleRouteHandler(), adminGroup, config);

		return v2;
	}

	private static void MapHandler(ITableRouteHandler handler, IEndpointRouteBuilder group, IConfiguration config)
		=> BaseTableRouteHandler.MapRoutes(handler, group, config);

	private static void MapWriteHandler(ITableRouteHandler handler, IEndpointRouteBuilder group, IConfiguration config)
		=> BaseTableRouteHandler.MapWriteRoutes(handler, group, config);

	private static void MapAllHandler(ITableRouteHandler handler, IEndpointRouteBuilder group, IConfiguration config)
		=> BaseTableRouteHandler.MapAllRoutes(handler, group, config);
}
