using Microsoft.Extensions.Options;
using ObjectService.TableHandlers;
using OpenLoco.Dat;

namespace OpenLoco.ObjectService
{
	public class Server
	{
		public Server(ServerSettings settings)
		{
			Settings = settings;
			ServerFolderManager = new ServerFolderManager(Settings.RootFolder)!;
			ScenarioHandler = new(ServerFolderManager.ScenariosFolder);
			var pm = new PaletteMap(Settings.PaletteMapFile);
			ObjectHandler = new(ServerFolderManager, pm);
			LegacyRouteHandler = new(ServerFolderManager, pm);
		}

		public Server(IOptions<ServerSettings> options) : this(options.Value)
		{ }

		ServerSettings Settings { get; init; }

		ServerFolderManager ServerFolderManager { get; init; }

		public AuthorRequestHandler AuthorHandler { get; init; } = new();
		public TagRequestHandler TagHandler { get; init; } = new();
		public LicenceRequestHandler LicenceHandler { get; init; } = new();
		public ObjectPackRequestHandler ObjectPackHandler { get; init; } = new();
		public SC5FilePackRequestHandler SC5FilePackHandler { get; init; } = new();
		public ScenarioRequestHandler ScenarioHandler { get; init; }
		public ObjectRequestHandler ObjectHandler { get; init; }

		public LegacyRouteHandler LegacyRouteHandler { get; init; }

		public IEndpointRouteBuilder MapRoutes(IEndpointRouteBuilder routeGroup)
		{
			AuthorHandler.MapRoutes(routeGroup);
			TagHandler.MapRoutes(routeGroup);
			LicenceHandler.MapRoutes(routeGroup);
			ObjectHandler.MapRoutes(routeGroup);
			ScenarioHandler.MapRoutes(routeGroup);
			SC5FilePackHandler.MapRoutes(routeGroup);
			ObjectPackHandler.MapRoutes(routeGroup);

			LegacyRouteHandler.MapRoutes(routeGroup);

			return routeGroup;
		}
	}
}
