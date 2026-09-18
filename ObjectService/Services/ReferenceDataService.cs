using Definitions.Database;
using Definitions.DTO;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

/// <summary>
/// Read-only descriptor queries for reference data (authors, tags, licences) and the
/// entities that reference them. These back the public API's <c>/descriptor</c> routes.
/// </summary>
public interface IReferenceDataService
{
	Task<DtoAuthorDescriptor?> GetAuthorAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoTagDescriptor?> GetTagAsync(UniqueObjectId id, CancellationToken ct);
	Task<DtoLicenceDescriptor?> GetLicenceAsync(UniqueObjectId id, CancellationToken ct);
}

public class ReferenceDataService : IReferenceDataService
{
	private readonly LocoDbContext _db;

	public ReferenceDataService(LocoDbContext db)
	{
		_db = db;
	}

	public async Task<DtoAuthorDescriptor?> GetAuthorAsync(UniqueObjectId id, CancellationToken ct)
	{
		var author = await _db.Authors
			.Include(a => a.Objects)
			.Include(a => a.ObjectPacks)
			.Include(a => a.SC5Files)
			.Include(a => a.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(a => a.Id == id, ct);

		return author is null
			? null
			: new DtoAuthorDescriptor(
				author.Id,
				author.Name,
				[.. author.Objects.OrderBy(o => o.Name).Select(o => new DtoItemRef(o.Id, o.Description ?? o.Name))],
				[.. author.ObjectPacks.OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))],
				[.. author.SC5Files.OrderBy(f => f.Name).Select(f => new DtoItemRef(f.Id, f.Name))],
				[.. author.SC5FilePacks.OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))]);
	}

	public async Task<DtoTagDescriptor?> GetTagAsync(UniqueObjectId id, CancellationToken ct)
	{
		var tag = await _db.Tags
			.Include(t => t.Objects)
			.Include(t => t.ObjectPacks)
			.Include(t => t.SC5Files)
			.Include(t => t.SC5FilePacks)
			.AsSplitQuery()
			.FirstOrDefaultAsync(t => t.Id == id, ct);

		return tag is null
			? null
			: new DtoTagDescriptor(
				tag.Id,
				tag.Name,
				[.. tag.Objects.OrderBy(o => o.Name).Select(o => new DtoItemRef(o.Id, o.Description ?? o.Name))],
				[.. tag.ObjectPacks.OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))],
				[.. tag.SC5Files.OrderBy(f => f.Name).Select(f => new DtoItemRef(f.Id, f.Name))],
				[.. tag.SC5FilePacks.OrderBy(p => p.Name).Select(p => new DtoItemRef(p.Id, p.Name))]);
	}

	public async Task<DtoLicenceDescriptor?> GetLicenceAsync(UniqueObjectId id, CancellationToken ct)
	{
		var licence = await _db.Licences.FirstOrDefaultAsync(l => l.Id == id, ct);
		if (licence is null)
		{
			return null;
		}

		var objects = await _db.Objects
			.Where(o => o.Licence != null && o.Licence.Id == id)
			.OrderBy(o => o.Name)
			.Select(o => new DtoItemRef(o.Id, o.Description ?? o.Name))
			.ToListAsync(ct);

		var objectPacks = await _db.ObjectPacks
			.Where(p => p.Licence != null && p.Licence.Id == id)
			.OrderBy(p => p.Name)
			.Select(p => new DtoItemRef(p.Id, p.Name))
			.ToListAsync(ct);

		var sc5Files = await _db.SC5Files
			.Where(f => f.Licence != null && f.Licence.Id == id)
			.OrderBy(f => f.Name)
			.Select(f => new DtoItemRef(f.Id, f.Name))
			.ToListAsync(ct);

		var sc5FilePacks = await _db.SC5FilePacks
			.Where(p => p.Licence != null && p.Licence.Id == id)
			.OrderBy(p => p.Name)
			.Select(p => new DtoItemRef(p.Id, p.Name))
			.ToListAsync(ct);

		return new DtoLicenceDescriptor(licence.Id, licence.Name, licence.Text, objects, objectPacks, sc5Files, sc5FilePacks);
	}
}
