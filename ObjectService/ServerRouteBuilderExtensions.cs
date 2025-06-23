using Definitions;
using ObjectService.RouteHandlers;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

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

		public static IEndpointConventionBuilder MapCrudRoutes<T, TDto, TRow>(this IEndpointRouteBuilder endpoints)
			where T : ITableRequestHandler<TDto>, ITableRequestConfig<TDto, TRow>
			where TDto : class, IHasId
			where TRow : class, IHasId
		{
			ArgumentNullException.ThrowIfNull(endpoints);

			var baseRoute = endpoints
				.MapGroup(T.GetBaseRoute())
				.WithTags(MakeNicePlural(typeof(T).Name));

			_ = baseRoute.MapGet(string.Empty, T.ListAsync);
			_ = baseRoute.MapPost(string.Empty, T.CreateAsync).RequireAuthorization(AdminPolicy.Name);

			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(string.Empty, T.ReadAsync);
			_ = resourceRoute.MapPut(string.Empty, T.UpdateAsync).RequireAuthorization(AdminPolicy.Name);
			_ = resourceRoute.MapDelete(string.Empty, T.DeleteAsync).RequireAuthorization(AdminPolicy.Name);

			T.MapAdditionalRoutes(baseRoute);

			static string MakeNicePlural(string name)
				=> $"{name.Replace("RouteHandler", string.Empty)}s";

			return baseRoute;
		}
	}
}
