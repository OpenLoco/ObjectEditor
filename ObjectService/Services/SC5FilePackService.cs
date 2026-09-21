using System.IO.Compression;
using Common;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.SourceData;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

public interface IScenarioPackService
{
	Task<IEnumerable<DtoItemPackDescriptor<DtoScenarioEntry>>> ListPacksAsync(CancellationToken ct);
	Task<IEnumerable<DtoScenarioPackListEntry>> ListEntriesAsync(CancellationToken ct);
	Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetPackAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoScenarioPackDescriptor?> GetDescriptorAsync(UniqueObjectId id, CancellationToken ct);
	Task<(Stream? Stream, string FileName)> GetPackFileAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoItemPackDescriptor<DtoScenarioEntry>> CreatePackAsync(DtoItemPackDescriptor<DtoScenarioEntry> request, UniqueObjectId ownerUserId, CancellationToken ct);
	Task<DtoScenarioPackDescriptor?> UpdateAsync(UniqueObjectId id, DtoScenarioPackDescriptor request, CancellationToken ct);
	Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct);
}

public class ScenarioPackService : IScenarioPackService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;
	public ScenarioPackService(LocoDbContext db, ServerFolderManager sfm)
	{
		_db = db;
		_sfm = sfm;
	}

	public async Task<IEnumerable<DtoItemPackDescriptor<DtoScenarioEntry>>> ListPacksAsync(CancellationToken ct)
	{
		var packs = await _db.ScenarioPacks.Include(l => l.Licence).ToListAsync(ct);
		return packs.Select(x => x.ToDtoEntry()).OrderBy(x => x.Name);
	}

	public async Task<IEnumerable<DtoScenarioPackListEntry>> ListEntriesAsync(CancellationToken ct)
	{
		var packs = await _db.ScenarioPacks
			.Include(p => p.Licence)
			.Include(p => p.Authors)
			.Include(p => p.Tags)
			.Include(p => p.Scenarios)
			.AsSplitQuery()
			.ToListAsync(ct);

		return packs
			.Select(p => new DtoScenarioPackListEntry(
				p.Id,
				p.Name,
				p.Description,
				p.UploadedDate,
				p.Licence?.ToDtoEntry(),
				p.Authors.Count,
				p.Tags.Count,
				p.Scenarios.Count))
			.OrderBy(p => p.Name);
	}

	public async Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetPackAsync(UniqueObjectId id, CancellationToken ct)
	{
		var pack = await _db.ScenarioPacks
			.Where(x => x.Id == id)
			.Include(l => l.Licence)
			.Select(x => new ExpandedTblPack<TblScenarioPack, TblScenario>(x, x.Scenarios, x.Authors, x.Tags))
			.SingleOrDefaultAsync(ct);

		return pack?.ToDtoDescriptor();
	}

	public async Task<DtoScenarioPackDescriptor?> GetDescriptorAsync(UniqueObjectId id, CancellationToken ct)
	{
		var pack = await _db.ScenarioPacks
			.Where(x => x.Id == id)
			.Include(x => x.Licence)
			.Include(x => x.Authors)
			.Include(x => x.Tags)
			.Include(x => x.Scenarios)
			.AsSplitQuery()
			.FirstOrDefaultAsync(ct);

		return pack is null ? null : ToDescriptor(pack);
	}

	public async Task<DtoScenarioPackDescriptor?> UpdateAsync(UniqueObjectId id, DtoScenarioPackDescriptor request, CancellationToken ct)
	{
		var pack = await _db.ScenarioPacks
			.Where(x => x.Id == id)
			.Include(x => x.Licence)
			.Include(x => x.Authors)
			.Include(x => x.Tags)
			.Include(x => x.Scenarios)
			.AsSplitQuery()
			.FirstOrDefaultAsync(ct);

		if (pack is null)
		{
			return null;
		}

		pack.Name = request.Name;
		pack.Description = request.Description;
		pack.CreatedDate = request.CreatedDate;
		pack.ModifiedDate = request.ModifiedDate;

		pack.Licence = request.Licence is null ? null : await _db.Licences.FindAsync([request.Licence.Id], ct);

		pack.Authors.Clear();
		var authorIds = request.Authors.Select(a => a.Id).ToList();
		if (authorIds.Count > 0)
		{
			var authors = await _db.Authors.Where(a => authorIds.Contains(a.Id)).ToListAsync(ct);
			foreach (var author in authors)
			{
				pack.Authors.Add(author);
			}
		}

		pack.Tags.Clear();
		var tagIds = request.Tags.Select(t => t.Id).ToList();
		if (tagIds.Count > 0)
		{
			var tags = await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);
			foreach (var tag in tags)
			{
				pack.Tags.Add(tag);
			}
		}

		pack.Scenarios.Clear();
		var fileIds = request.SC5Files.Select(f => f.Id).ToList();
		if (fileIds.Count > 0)
		{
			var files = await _db.Scenarios.Where(f => fileIds.Contains(f.Id)).ToListAsync(ct);
			foreach (var file in files)
			{
				pack.Scenarios.Add(file);
			}
		}

		_ = await _db.SaveChangesAsync(ct);
		return ToDescriptor(pack);
	}

	public async Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct)
	{
		var pack = await _db.ScenarioPacks.FindAsync([id], ct);
		if (pack is null)
		{
			return false;
		}

		_ = _db.ScenarioPacks.Remove(pack);
		_ = await _db.SaveChangesAsync(ct);
		return true;
	}

	static DtoScenarioPackDescriptor ToDescriptor(TblScenarioPack pack)
		=> new(
			pack.Id,
			pack.Name,
			pack.Description,
			pack.CreatedDate,
			pack.ModifiedDate,
			pack.UploadedDate,
			pack.Licence?.ToDtoEntry(),
			[.. pack.Authors.OrderBy(a => a.Name).Select(a => a.ToDtoEntry())],
			[.. pack.Tags.OrderBy(t => t.Name).Select(t => t.ToDtoEntry())],
			[.. pack.Scenarios.OrderBy(f => f.Name).Select(f => new DtoItemRef(f.Id, f.Name))]);

	public async Task<(Stream? Stream, string FileName)> GetPackFileAsync(UniqueObjectId id, CancellationToken ct)
	{
		var pack = await _db.ScenarioPacks.Where(x => x.Id == id).Include(x => x.Scenarios).SingleOrDefaultAsync(ct);
		if (pack == null)
		{
			return (null, string.Empty);
		}

		var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
		var zipStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
		using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			foreach (var scenario in pack.Scenarios)
			{
				if (!RouteHelpers.TryGetSafeRelativePathUnderRoot(_sfm.ScenariosFolder, scenario.Name, out var fullPath, out var entryName))
				{
					continue;
				}

				if (!File.Exists(fullPath))
				{
					continue;
				}

				await using var fs = File.OpenRead(fullPath);
				var ze = archive.CreateEntry(entryName, CompressionLevel.Optimal);
				await using var es = ze.Open();
				await fs.CopyToAsync(es, ct);
			}
		}
		zipStream.Position = 0;
		var dn = DownloadNameHelper.MakeSafeDownloadFileName(pack.Name, ".zip", "scenario-pack");
		return (zipStream, dn);
	}

	public async Task<DtoItemPackDescriptor<DtoScenarioEntry>> CreatePackAsync(DtoItemPackDescriptor<DtoScenarioEntry> request, UniqueObjectId ownerUserId, CancellationToken ct)
	{
		var pack = new TblScenarioPack
		{
			Name = request.Name,
			Description = request.Description,
			OwnerUserId = ownerUserId,
		};

		// Link the owner's associated author if available
		var owner = await _db.Users.Include(u => u.AssociatedAuthor).FirstOrDefaultAsync(u => u.Id == ownerUserId, ct);
		if (owner?.AssociatedAuthor != null)
		{
			pack.Authors.Add(owner.AssociatedAuthor);
		}

		// Add requested scenario files if specified
		if (request.Items?.Count > 0)
		{
			var fileIds = request.Items.Select(i => i.Id).ToList();
			var files = await _db.Scenarios.Where(f => fileIds.Contains(f.Id)).ToListAsync(ct);
			foreach (var file in files)
			{
				pack.Scenarios.Add(file);
			}
		}

		_ = await _db.ScenarioPacks.AddAsync(pack, ct);
		_ = await _db.SaveChangesAsync(ct);

		// Reload to get computed columns like UploadedDate
		var created = await _db.ScenarioPacks.Include(p => p.Licence).FirstAsync(p => p.Id == pack.Id, ct);
		return created.ToDtoEntry();
	}
}