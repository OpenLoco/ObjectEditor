using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

/// <summary>
/// Database-backed scenario queries that back the public <c>/v2/scenarios</c> routes. Scenario
/// metadata lives in the <c>Scenarios</c> table; the files themselves live on disk under the
/// Scenarios folder and may or may not still exist there.
/// </summary>
public interface IScenarioService
{
	Task<IEnumerable<DtoScenarioListEntry>> ListEntriesAsync(CancellationToken ct);
	Task<DtoScenarioDescriptor?> GetScenarioAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoScenarioDescriptor?> UpdateAsync(UniqueObjectId id, DtoScenarioDescriptor request, CancellationToken ct);
	Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct);
	Task<string?> GetScenarioFilePathByIdAsync(UniqueObjectId id, CancellationToken ct);
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

	public async Task<IEnumerable<DtoScenarioListEntry>> ListEntriesAsync(CancellationToken ct)
	{
		var files = await Query().ToListAsync(ct).ConfigureAwait(false);

		return files
			.Select(f => new DtoScenarioListEntry(
				f.Id,
				f.Name,
				f.Description,
				f.UploadedDate,
				f.ObjectSource,
				f.Licence?.ToDtoEntry(),
				f.Authors.Count,
				f.Tags.Count,
				f.ScenarioPacks.Count))
			.OrderBy(f => f.Name)
			.ToArray();
	}

	public async Task<DtoScenarioDescriptor?> GetScenarioAsync(UniqueObjectId id, CancellationToken ct)
	{
		var scenario = await Query()
			.Where(s => s.Id == id)
			.FirstOrDefaultAsync(ct)
			.ConfigureAwait(false);

		return scenario is null ? null : ToDescriptor(scenario);
	}

	public async Task<DtoScenarioDescriptor?> UpdateAsync(UniqueObjectId id, DtoScenarioDescriptor request, CancellationToken ct)
	{
		var scenario = await Query()
			.Where(s => s.Id == id)
			.FirstOrDefaultAsync(ct)
			.ConfigureAwait(false);

		if (scenario is null)
		{
			return null;
		}

		scenario.Name = request.Name;
		scenario.Description = request.Description;
		scenario.ObjectSource = request.ObjectSource;
		scenario.CreatedDate = request.CreatedDate;
		scenario.ModifiedDate = request.ModifiedDate;

		scenario.Licence = request.Licence is null
			? null
			: await _db.Licences.FindAsync([request.Licence.Id], ct).ConfigureAwait(false);

		scenario.Authors.Clear();
		var authorIds = request.Authors.Select(a => a.Id).ToList();
		if (authorIds.Count > 0)
		{
			foreach (var author in await _db.Authors.Where(a => authorIds.Contains(a.Id)).ToListAsync(ct).ConfigureAwait(false))
			{
				scenario.Authors.Add(author);
			}
		}

		scenario.Tags.Clear();
		var tagIds = request.Tags.Select(t => t.Id).ToList();
		if (tagIds.Count > 0)
		{
			foreach (var tag in await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct).ConfigureAwait(false))
			{
				scenario.Tags.Add(tag);
			}
		}

		scenario.ScenarioPacks.Clear();
		var packIds = request.ScenarioPacks.Select(p => p.Id).ToList();
		if (packIds.Count > 0)
		{
			foreach (var pack in await _db.ScenarioPacks.Where(p => packIds.Contains(p.Id)).ToListAsync(ct).ConfigureAwait(false))
			{
				scenario.ScenarioPacks.Add(pack);
			}
		}

		_ = await _db.SaveChangesAsync(ct).ConfigureAwait(false);
		return ToDescriptor(scenario);
	}

	public async Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct)
	{
		var scenario = await _db.Scenarios.FindAsync([id], ct).ConfigureAwait(false);
		if (scenario is null)
		{
			return false;
		}

		// Park the scenario file under Scenarios/Removed before dropping the row, so it is not
		// re-imported by the next reconciliation.
		if (!string.IsNullOrWhiteSpace(scenario.Name)
			&& RouteHelpers.TryGetSafeRelativePathUnderRoot(_sfm.ScenariosFolder, scenario.Name, out var fullPath, out _)
			&& File.Exists(fullPath))
		{
			_ = ServerFolderManager.MoveToRemovedFolder(_sfm.ScenariosFolder, fullPath);
		}

		_ = _db.Scenarios.Remove(scenario);
		_ = await _db.SaveChangesAsync(ct).ConfigureAwait(false);
		return true;
	}

	public async Task<string?> GetScenarioFilePathByIdAsync(UniqueObjectId id, CancellationToken ct)
	{
		var scenario = await _db.Scenarios
			.AsNoTracking()
			.FirstOrDefaultAsync(s => s.Id == id, ct)
			.ConfigureAwait(false);

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

	private IQueryable<TblScenario> Query()
		=> _db.Scenarios
			.Include(f => f.Licence)
			.Include(f => f.Authors)
			.Include(f => f.Tags)
			.Include(f => f.ScenarioPacks)
			.AsSplitQuery();

	private static DtoScenarioDescriptor ToDescriptor(TblScenario file)
		=> new(
			file.Id,
			file.Name,
			file.Description,
			file.ObjectSource,
			file.CreatedDate,
			file.ModifiedDate,
			file.UploadedDate,
			file.Licence?.ToDtoEntry(),
			[.. file.Authors.OrderBy(a => a.Name).Select(a => a.ToDtoEntry())],
			[.. file.Tags.OrderBy(t => t.Name).Select(t => t.ToDtoEntry())],
			[.. file.ScenarioPacks.OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))]);
}
