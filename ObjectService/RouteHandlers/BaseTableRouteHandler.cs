using Definitions.Web;

namespace ObjectService.RouteHandlers;

public static class BaseTableRouteHandler
{
public static void MapRoutes<THandler>(
IEndpointRouteBuilder parentRoute
) where THandler : ITableRouteHandler
{
var baseRoute = parentRoute
.MapGroup(THandler.BaseRoute)
.WithTags(RouteHelpers.MakeNicePlural(typeof(THandler).Name));

_ = baseRoute.MapGet(string.Empty, THandler.ListDelegate);

var resourceRoute = baseRoute.MapGroup(RoutesV2.ResourceRoute);
_ = resourceRoute.MapGet(string.Empty, THandler.ReadDelegate);

var config = parentRoute.ServiceProvider.GetRequiredService<IConfiguration>();
var enableWriteRoutes = config.GetValue<bool?>("ObjectService:EnableWriteRoutes") ?? false;

if (enableWriteRoutes)
{
_ = baseRoute.MapPost(string.Empty, THandler.CreateDelegate);
_ = resourceRoute.MapPut(string.Empty, THandler.UpdateDelegate);
_ = resourceRoute.MapDelete(string.Empty, THandler.DeleteDelegate);
}

THandler.MapAdditionalRoutes(baseRoute);
}
}
