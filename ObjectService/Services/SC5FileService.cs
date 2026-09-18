using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// Database-backed scenario (SC5) file queries that back the public <c>/v2/sc5files</c> routes.
/// </summary>
public interface ISC5FileService
{
	Task<IEnumerable<DtoSC5FileListEntry>> ListEntriesAsync(CancellationToken ct);
	Task<DtoSC5FileDescriptor?> GetDescriptorAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoSC5FileDescriptor?> UpdateAsync(UniqueObjectId id, DtoSC5FileDescriptor request, CancellationToken ct);
	Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct);
}

public class SC5FileService : ISC5FileService
{
	private readonly LocoDbContext _db;

	public SC5FileService(LocoDbContext db)
	{
		_db = db;
	}

	public async Task<IEnumerable<DtoSC5FileListEntry>> ListEntriesAsync(CancellationToken ct)
	{
		var files = await _db.SC5Files
			.Include(f => f.Licence)
			.Include(f => f.Authors)
			.Include(f => f.Tags)
			.Include(f => f.SC5FilePacks)
			.AsSplitQuery()
			.ToListAsync(ct);

		return files
			.Select(f => new DtoSC5FileListEntry(
				f.Id,
				f.Name,
				f.Description,
				f.UploadedDate,
				f.ObjectSource,
				f.Licence?.ToDtoEntry(),
				f.Authors.Count,
				f.Tags.Count,
				f.SC5FilePacks.Count))
			.OrderBy(f => f.Name);
	}

	public async Task<DtoSC5FileDescriptor?> GetDescriptorAsync(UniqueObjectId id, CancellationToken ct)
	{
		var file = await _db.SC5Files
			.Where(f => f.Id == id)
			.Include(f => f.Licence)
			.Include(f => f.Authors)
			.Include(f => f.Tags)
			.Include(f => f.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(ct);

		return file is null ? null : ToDescriptor(file);
	}

	public async Task<DtoSC5FileDescriptor?> UpdateAsync(UniqueObjectId id, DtoSC5FileDescriptor request, CancellationToken ct)
	{
		var file = await _db.SC5Files
			.Where(f => f.Id == id)
			.Include(f => f.Licence)
			.Include(f => f.Authors)
			.Include(f => f.Tags)
			.Include(f => f.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(ct);

		if (file is null)
		{
			return null;
		}

		file.Name = request.Name;
		file.Description = request.Description;
		file.ObjectSource = request.ObjectSource;
		file.CreatedDate = request.CreatedDate;
		file.ModifiedDate = request.ModifiedDate;

		file.Licence = request.Licence is null ? null : await _db.Licences.FindAsync([request.Licence.Id], ct);

		file.Authors.Clear();
		var authorIds = request.Authors.Select(a => a.Id).ToList();
		if (authorIds.Count > 0)
		{
			var authors = await _db.Authors.Where(a => authorIds.Contains(a.Id)).ToListAsync(ct);
			foreach (var author in authors)
			{
				file.Authors.Add(author);
			}
		}

		file.Tags.Clear();
		var tagIds = request.Tags.Select(t => t.Id).ToList();
		if (tagIds.Count > 0)
		{
			var tags = await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct);
			foreach (var tag in tags)
			{
				file.Tags.Add(tag);
			}
		}

		file.SC5FilePacks.Clear();
		var packIds = request.SC5FilePacks.Select(p => p.Id).ToList();
		if (packIds.Count > 0)
		{
			var packs = await _db.SC5FilePacks.Where(p => packIds.Contains(p.Id)).ToListAsync(ct);
			foreach (var pack in packs)
			{
				file.SC5FilePacks.Add(pack);
			}
		}

		_ = await _db.SaveChangesAsync(ct);
		return ToDescriptor(file);
	}

	public async Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct)
	{
		var file = await _db.SC5Files.FindAsync([id], ct);
		if (file is null)
		{
			return false;
		}

		_ = _db.SC5Files.Remove(file);
		_ = await _db.SaveChangesAsync(ct);
		return true;
	}

	static DtoSC5FileDescriptor ToDescriptor(TblSC5File file)
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
			[.. file.SC5FilePacks.OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))]);
}
