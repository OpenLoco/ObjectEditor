using Microsoft.AspNetCore.Mvc;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;
using OpenLoco.ObjectService;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class ScenarioRequestHandler : BaseRouteHandler<ScenarioRequestHandler> //, ITableRouteHandler
	{
		public override string BaseRoute => Routes.Scenarios;
		public override Delegate ListDelegate => ListAsync;
		public override Delegate CreateDelegate => CreateAsync;

		public override Delegate ReadDelegate => ReadAsync;

		public override Delegate UpdateDelegate => UpdateAsync;

		public override Delegate DeleteDelegate => DeleteAsync;

		//protected override void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute) => throw new NotImplementedException();
		//		public static void MapRoutes(IEndpointRouteBuilder parentRoute)
		//		{
		//			var baseRoute = parentRoute
		//				.MapGroup(BaseRoute)
		//				.WithTags(RouteHelpers.MakeNicePlural(nameof(ScenarioRequestHandler)));

		//			_ = baseRoute.MapGet(string.Empty, ListAsync);

		//			var resourceRoute = baseRoute.MapGroup(Routes.ResourceRoute);
		//			_ = resourceRoute.MapGet(string.Empty, ReadAsync);

		//#if DEBUG
		//			_ = baseRoute.MapPost(string.Empty, CreateAsync); //.RequireAuthorization(AdminPolicy.Name);
		//			_ = resourceRoute.MapPut(string.Empty, UpdateAsync); //.RequireAuthorization(AdminPolicy.Name);
		//			_ = resourceRoute.MapDelete(string.Empty, DeleteAsync); //.RequireAuthorization(AdminPolicy.Name);
		//#endif
		//		}

		static async Task<IResult> ListAsync([FromServices] IServiceProvider sp)
			=> await Task.Run(() =>
			{
				var sfm = sp.GetRequiredService<ServerFolderManager>();
				var scenarioFolder = sfm.ScenariosFolder;
				var files = Directory.GetFiles(scenarioFolder, "*.SC5", SearchOption.AllDirectories);
				var count = 0UL;
				var filenames = files.Select(x => new DtoScenarioEntry(count++, Path.GetRelativePath(scenarioFolder, x)));
				return Results.Ok(filenames.ToList());
			});

		static async Task<IResult> CreateAsync(DtoScenarioEntry request)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		static async Task<IResult> ReadAsync([FromRoute] DbKey Id)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		static async Task<IResult> UpdateAsync([FromRoute] DbKey Id, DtoScenarioEntry request)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		static async Task<IResult> DeleteAsync([FromRoute] DbKey Id)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}
}
