using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ScenarioRouteHandler : ITableRouteHandler
{
	public static string BaseRoute => RoutesV2.Scenarios;
	public static Delegate ListDelegate => ListAsync;
	public static Delegate CreateDelegate => CreateAsync;
	public static Delegate ReadDelegate => ReadAsync;
	public static Delegate UpdateDelegate => UpdateAsync;
	public static Delegate DeleteDelegate => DeleteAsync;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<ScenarioRouteHandler>(endpoints);

	public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
	{
		var resourceRoute = parentRoute.MapGroup(RoutesV2.ResourceRoute);
		_ = resourceRoute.MapGet(RoutesV2.File, GetScenarioFileAsync);
	}

	static string[] GetSortedScenarioFiles(string scenarioFolder)
		=> [.. Directory.GetFiles(scenarioFolder, "*.SC5", SearchOption.AllDirectories).OrderBy(x => x)];

	static async Task<IResult> ListAsync([FromServices] IServiceProvider sp)
		=> await Task.Run(() =>
		{
			var sfm = sp.GetRequiredService<ServerFolderManager>();
			var scenarioFolder = sfm.ScenariosFolder;
			var files = GetSortedScenarioFiles(scenarioFolder);
			var count = 0UL;
			var filenames = files.Select(x => new DtoScenarioEntry(count++, Path.GetRelativePath(scenarioFolder, x)));
			return Results.Ok(filenames.ToList());
		});

	static async Task<IResult> GetScenarioFileAsync([FromRoute] UniqueObjectId id, [FromServices] IServiceProvider sp)
		=> await Task.Run(() =>
		{
			var sfm = sp.GetRequiredService<ServerFolderManager>();
			var files = GetSortedScenarioFiles(sfm.ScenariosFolder);

			if (id >= (ulong)files.Length)
			{
				return Results.NotFound();
			}

			var path = files[(int)id];
			const string contentType = "application/octet-stream";
			return Results.File(path, contentType, Path.GetFileName(path));
		});

	static async Task<IResult> CreateAsync(DtoScenarioEntry request)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static async Task<IResult> ReadAsync([FromRoute] UniqueObjectId Id)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId Id, DtoScenarioEntry request)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId Id)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}
