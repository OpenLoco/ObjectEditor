namespace OpenLoco.ObjectService
{
	// this is purely for the dependency--injection wrapper in Program.cs
	public static class ServerRouteBuilderExtensions
	{
		public static IEndpointRouteBuilder MapServerRoutes(
			this IEndpointRouteBuilder endpoints,
			Server server)
		{
			ArgumentNullException.ThrowIfNull(endpoints);
			return server.MapRoutes(endpoints);
		}
	}
}
