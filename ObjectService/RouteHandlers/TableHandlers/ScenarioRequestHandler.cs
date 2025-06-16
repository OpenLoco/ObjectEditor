using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class ScenarioRequestHandler : BaseTableRequestHandler<DtoScenarioEntry>
	{
		public ScenarioRequestHandler(string scenarioFolderOnDisk)
			=> ScenarioFolderOnDisk = scenarioFolderOnDisk;

		public string ScenarioFolderOnDisk { get; init; }

		public override string BaseRoute
			=> Routes.Scenarios;

		public override async Task<IResult> CreateAsync(DtoScenarioEntry request, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> ReadAsync(int id, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> UpdateAsync(int id, DtoScenarioEntry request, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> DeleteAsync(int id, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
			=> await Task.Run(() =>
			{
				var files = Directory.GetFiles(ScenarioFolderOnDisk, "*.SC5", SearchOption.AllDirectories);
				var count = 0;
				var filenames = files.Select(x => new DtoScenarioEntry(count++, Guid.Empty, Path.GetRelativePath(ScenarioFolderOnDisk, x)));
				return Results.Ok(filenames.ToList());
			});
	}
}
