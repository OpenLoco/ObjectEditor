namespace ObjectService.RouteHandlers
{
	public interface ITableRouteHandler
	{
		static abstract string BaseRoute { get; }
		static abstract Delegate ListDelegate { get; }
		static abstract Delegate CreateDelegate { get; }
		static abstract Delegate ReadDelegate { get; }
		static abstract Delegate UpdateDelegate { get; }
		static abstract Delegate DeleteDelegate { get; }

		static virtual void MapRoutes(IEndpointRouteBuilder endpoints) { }

		static virtual void MapAdditionalRoutes(IEndpointRouteBuilder endpoints) { }
	}
}
