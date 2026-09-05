namespace ObjectService.RouteHandlers;

public interface ITableRouteHandler
{
	string BaseRoute { get; }
	Delegate ListDelegate { get; }
	Delegate CreateDelegate { get; }
	Delegate ReadDelegate { get; }
	Delegate UpdateDelegate { get; }
	Delegate DeleteDelegate { get; }

	void MapRoutes(IEndpointRouteBuilder endpoints);

	void MapAdditionalRoutes(IEndpointRouteBuilder endpoints);
}
