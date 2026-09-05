using Definitions.DTO;

namespace ObjectService.Services;

public interface IScenarioService
{
	IEnumerable<DtoScenarioEntry> ListScenarios();
	string? GetScenarioFilePath(ulong index);
}

public class ScenarioService : IScenarioService
{
	private readonly ServerFolderManager _sfm;

	public ScenarioService(ServerFolderManager sfm)
	{
		_sfm = sfm;
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

	private static string[] GetSortedScenarioFiles(string folder)
	=> [.. Directory
.GetFiles(folder, "*.SC5", SearchOption.AllDirectories)
.OrderBy(x => Path.GetRelativePath(folder, x), StringComparer.Ordinal)];
}
