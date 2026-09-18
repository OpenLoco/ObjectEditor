namespace Definitions.Web;

public readonly record struct ApiEndpointGroup(string Route, string Prefix = Routes.Prefix);
