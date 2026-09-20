using Definitions.DTO;
using Definitions.Web;
using ObjectService.RouteHandlers.TableHandlers;

namespace ObjectService.RouteHandlers;

public static class RouteBuilderExtensions
{
	public static IEndpointConventionBuilder MapApiRoutes(this IEndpointRouteBuilder endpoints)
	{
		var v2 = endpoints.MapGroup(Routes.Prefix);
		var config = endpoints.ServiceProvider.GetRequiredService<IConfiguration>();

		// Public read-only routes (guest + authenticated can read)
		var publicGroup = v2.MapGroup(string.Empty);
		MapHandler(new AuthorRouteHandler(), publicGroup, config);
		MapHandler(new TagRouteHandler(), publicGroup, config);
		MapHandler(new LicenceRouteHandler(), publicGroup, config);
		MapHandler(new ObjectRouteHandler(), publicGroup, config);
		MapHandler(new ObjectMissingRouteHandler(), publicGroup, config);
		MapHandler(new ScenarioRouteHandler(), publicGroup, config);
		MapHandler(new ScenarioPackRouteHandler(), publicGroup, config);
		MapHandler(new MusicRouteHandler(), publicGroup, config);
		MapHandler(new SoundEffectsRouteHandler(), publicGroup, config);
		MapHandler(new TutorialsRouteHandler(), publicGroup, config);
		MapHandler(new GraphicsRouteHandler(), publicGroup, config);
		MapHandler(new ObjectPackRouteHandler(), publicGroup, config);

		// Public capability route. Clients (e.g. the Object Editor) query this to discover whether
		// the server is in read-only mode before attempting writes that would otherwise be rejected.
		_ = publicGroup.MapGet(Routes.Server + Routes.Status, (IConfiguration config) =>
			Results.Ok(new DtoServerStatus(
				config.GetValue<bool?>("ObjectService:FrontendReadOnly") ?? false,
				config.GetValue<bool?>("ObjectService:BackendReadOnly") ?? false)));

		// Authenticated write routes for general data (any authenticated user)
		var authGroup = v2.MapGroup(string.Empty).RequireAuthorization();
		MapWriteHandler(new ObjectMissingRouteHandler(), authGroup, config);
		MapWriteHandler(new ScenarioRouteHandler(), authGroup, config);
		MapWriteHandler(new MusicRouteHandler(), authGroup, config);
		MapWriteHandler(new SoundEffectsRouteHandler(), authGroup, config);
		MapWriteHandler(new TutorialsRouteHandler(), authGroup, config);
		MapWriteHandler(new GraphicsRouteHandler(), authGroup, config);
		// Pack write routes require the relevant pack permission (create reuses the object-pack
		// create permission for scenario packs as well).
		MapWriteHandler(new ScenarioPackRouteHandler(), authGroup, config, "CanCreateObjectPacks", "CanModifyScenarioPacks");
		MapWriteHandler(new ObjectPackRouteHandler(), authGroup, config, "CanCreateObjectPacks", "CanModifyObjectPacks");
		// Curator write routes for metadata (requires Curator policy or Admin)
		var curatorGroup = v2.MapGroup(string.Empty).RequireAuthorization("Curator");
		MapWriteHandler(new AuthorRouteHandler(), curatorGroup, config);
		MapWriteHandler(new TagRouteHandler(), curatorGroup, config);
		MapWriteHandler(new LicenceRouteHandler(), curatorGroup, config);

		// Object write routes (create/edit/delete require ownership or admin)
		var ownerGroup = v2.MapGroup(string.Empty).RequireAuthorization("CanEditObject");
		MapWriteHandler(new ObjectRouteHandler(), ownerGroup, config);

		// Identity routes: reads need any authenticated user, writes are admin-only
		MapHandler(new UserRouteHandler(), authGroup, config);
		MapHandler(new RoleRouteHandler(), authGroup, config);
		var adminGroup = v2.MapGroup(string.Empty).RequireAuthorization("AdminOnly");
		MapWriteHandler(new UserRouteHandler(), adminGroup, config);
		MapWriteHandler(new RoleRouteHandler(), adminGroup, config);

		return v2;
	}

	private static void MapHandler(ITableRouteHandler handler, IEndpointRouteBuilder group, IConfiguration config)
		=> BaseTableRouteHandler.MapRoutes(handler, group, config);

	private static void MapWriteHandler(ITableRouteHandler handler, IEndpointRouteBuilder group, IConfiguration config, string? createPolicy = null, string? modifyPolicy = null)
		=> BaseTableRouteHandler.MapWriteRoutes(handler, group, config, createPolicy, modifyPolicy);
}
