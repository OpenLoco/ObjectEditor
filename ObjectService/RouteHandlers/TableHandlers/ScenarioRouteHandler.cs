using Microsoft.AspNetCore.Mvc;
using Definitions.DTO;
using Definitions.Web;

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

	static async Task<IResult> ReadAsync([FromRoute] UniqueObjectId Id)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId Id, DtoScenarioEntry request)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId Id)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}
