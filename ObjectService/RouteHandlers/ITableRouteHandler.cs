namespace ObjectService.RouteHandlers;

public interface ITableRouteHandler
{
	string BaseRoute { get; }
	Delegate ListDelegate { get; }
	Delegate CreateDelegate { get; }
	Delegate ReadDelegate { get; }
	Delegate UpdateDelegate { get; }
	Delegate DeleteDelegate { get; }

	/// <summary>
	/// Display name used to group this handler's endpoints in the generated API reference
	/// (Scalar/OpenAPI). Defaults to the pluralised handler type name; override it for handlers whose
	/// name is already plural or uncountable (e.g. <c>Music</c>, <c>Graphics</c>) so the tag is not
	/// double-pluralised (previously <c>Graphicss</c>, <c>Musics</c>).
	/// </summary>
	string TagName => RouteHelpers.MakeNicePlural(GetType().Name);

	void MapRoutes(IEndpointRouteBuilder endpoints);

	void MapAdditionalRoutes(IEndpointRouteBuilder endpoints);
}
