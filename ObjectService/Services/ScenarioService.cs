using Definitions.Database;
using Definitions.DTO;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

public interface IScenarioService
{
	IEnumerable<DtoScenarioEntry> ListScenarios();
	string? GetScenarioFilePath(ulong index);
	Task<string?> GetScenarioFilePathByIdAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoScenarioDescriptor?> GetScenarioAsync(UniqueObjectId id, CancellationToken ct);
}

public class ScenarioService : IScenarioService
{
	private readonly ServerFolderManager _sfm;
	private readonly LocoDbContext _db;

	public ScenarioService(ServerFolderManager sfm, LocoDbContext db)
	{
		_sfm = sfm;
		_db = db;
	}

	public IEnumerable<DtoScenarioEntry> ListScenarios()
	{
		var files = GetSortedScenarioFiles(_sfm.ScenariosFolder);
		return files
			.Select((file, index) => new DtoScenarioEntry((ulong)index, Path.GetRelativePath(_sfm.ScenariosFolder, file)))
			.ToArray();
	}

	public string? GetScenarioFilePath(ulong index)
	{
		var files = GetSortedScenarioFiles(_sfm.ScenariosFolder);
		return index < (ulong)files.Length ? files[(int)index] : null;
	}

	public async Task<string?> GetScenarioFilePathByIdAsync(UniqueObjectId id, CancellationToken ct)
	{
		var scenario = await _db.SC5Files
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.Id == id, ct);

		if (scenario is null || string.IsNullOrWhiteSpace(scenario.Name))
		{
			return null;
		}

		if (!RouteHelpers.TryGetSafeRelativePathUnderRoot(_sfm.ScenariosFolder, scenario.Name, out var fullPath, out _))
		{
			return null;
		}

		return File.Exists(fullPath) ? fullPath : null;
	}

	public async Task<DtoScenarioDescriptor?> GetScenarioAsync(UniqueObjectId id, CancellationToken ct)
	{
		var scenario = await _db.SC5Files
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.Id == id, ct);

		return scenario is null
			? null
			: new DtoScenarioDescriptor(scenario.Id, scenario.Name, scenario.Description);
	}

	private static string[] GetSortedScenarioFiles(string folder)
	=> [.. Directory
.GetFiles(folder, "*.SC5", SearchOption.AllDirectories)
.OrderBy(x => Path.GetRelativePath(folder, x), StringComparer.Ordinal)];
}
