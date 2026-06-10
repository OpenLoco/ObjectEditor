using Definitions;
using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Services;

// Database-backed local object index. Replaces the JSON-based ObjectIndex used by
// the editor client. Each scanned .dat file becomes a TblObject (one per unique
// content hash) with one or more TblDatObject rows pointing at it.
//
// The service uses LocoDbContext (the shared object-domain schema). Callers are
// expected to provide a context factory so each operation can scope its own
// short-lived context.
public class LocalObjectIndexService
{
	readonly Func<LocoDbContext> contextFactory;
	readonly ILogger logger;

	public LocalObjectIndexService(Func<LocoDbContext> contextFactory, ILogger logger)
	{
		this.contextFactory = contextFactory;
		this.logger = logger;
	}

	// Rebuilds the index from scratch by scanning the given directory and
	// replacing all existing rows. Existing TblObject/TblDatObject entries are
	// deleted before the scan results are inserted.
	public async Task RebuildFromFolderAsync(string directory, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
	{
		var results = await DatFolderScanner.ScanDirectoryAsync(directory, logger, progress, cancellationToken);
		await using var db = contextFactory();

		db.DatObjects.RemoveRange(db.DatObjects);
		db.Objects.RemoveRange(db.Objects);
		_ = await db.SaveChangesAsync(cancellationToken);

		await InsertScanResultsAsync(db, results.Succeeded, cancellationToken);

		foreach (var f in results.Failed)
		{
			logger.LogWarning("Failed to scan: {File}", f);
		}
	}

	// Adds the given relative .dat files (under `directory`) to the index, leaving
	// any pre-existing rows untouched. Existing hashes are skipped.
	public async Task AddFilesAsync(string directory, IEnumerable<string> relativeFiles, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
	{
		var results = DatFolderScanner.ScanFiles(directory, [.. relativeFiles], logger, progress, cancellationToken);
		await using var db = contextFactory();
		await InsertScanResultsAsync(db, results.Succeeded, cancellationToken);
	}

	public async Task<TblObject?> TryFindByDatAsync(string datName, uint datChecksum, CancellationToken cancellationToken = default)
	{
		await using var db = contextFactory();
		var dat = await db.DatObjects.AsNoTracking()
			.Include(d => d.Object)
			.FirstOrDefaultAsync(d => d.DatName == datName && d.DatChecksum == datChecksum, cancellationToken);
		return dat?.Object;
	}

	public async Task<TblObject?> TryFindByHashAsync(ulong xxHash3, CancellationToken cancellationToken = default)
	{
		await using var db = contextFactory();
		var dat = await db.DatObjects.AsNoTracking()
			.Include(d => d.Object)
			.FirstOrDefaultAsync(d => d.xxHash3 == xxHash3, cancellationToken);
		return dat?.Object;
	}

	// Builds a flat ObjectIndex projection from the DB for binding by the UI and
	// for use by anything that previously consumed the JSON ObjectIndex.
	public async Task<ObjectIndex> BuildObjectIndexAsync(CancellationToken cancellationToken = default)
	{
		await using var db = contextFactory();
		var rows = await db.DatObjects.AsNoTracking()
			.Include(d => d.Object)
			.Select(d => new ObjectIndexEntry(
				d.DatName,
				d.FileName,
				d.Object.Id,
				d.DatChecksum,
				d.xxHash3,
				d.Object.ObjectType,
				d.Object.ObjectSource,
				d.Object.CreatedDate,
				d.Object.ModifiedDate,
				d.Object.VehicleType,
				ObjectAvailability.Available))
			.ToListAsync(cancellationToken);
		return new ObjectIndex(rows);
	}

	public async Task<int> DeleteByHashAsync(IEnumerable<ulong> hashes, CancellationToken cancellationToken = default)
	{
		var hashSet = hashes.ToHashSet();
		await using var db = contextFactory();
		var dats = await db.DatObjects.Where(d => hashSet.Contains(d.xxHash3)).ToListAsync(cancellationToken);
		db.DatObjects.RemoveRange(dats);

		// also delete orphaned TblObjects (no remaining DatObjects)
		var orphanIds = dats.Select(d => d.ObjectId).Distinct().ToList();
		var orphans = await db.Objects
			.Where(o => orphanIds.Contains(o.Id) && !db.DatObjects.Any(d => d.ObjectId == o.Id))
			.ToListAsync(cancellationToken);
		db.Objects.RemoveRange(orphans);

		return await db.SaveChangesAsync(cancellationToken);
	}

	static async Task InsertScanResultsAsync(LocoDbContext db, IReadOnlyList<DatFileScanResult> results, CancellationToken cancellationToken)
	{
		if (results.Count == 0)
		{
			return;
		}

		// Group by xxHash3: identical content → single TblObject with multiple TblDatObject rows.
		var existingHashes = (await db.DatObjects.Select(d => d.xxHash3).ToListAsync(cancellationToken)).ToHashSet();

		foreach (var group in results.GroupBy(r => r.xxHash3))
		{
			if (existingHashes.Contains(group.Key))
			{
				continue;
			}

			var first = group.First();
			var obj = new TblObject
			{
				Name = first.DatName,
				Description = null,
				ObjectType = first.ObjectType,
				ObjectSource = first.ObjectSource,
				VehicleType = first.VehicleType,
				Availability = ObjectAvailability.Available,
				CreatedDate = first.CreatedDate,
				ModifiedDate = first.ModifiedDate,
				SubObjectId = 0,
				Authors = [],
				Tags = [],
				ObjectPacks = [],
				DatObjects = [],
				StringTable = [],
				Licence = null,
			};
			_ = db.Objects.Add(obj);
			_ = await db.SaveChangesAsync(cancellationToken); // need Id

			foreach (var r in group)
			{
				obj.DatObjects.Add(new TblDatObject
				{
					DatName = r.DatName,
					DatChecksum = r.DatChecksum,
					xxHash3 = r.xxHash3,
					FileName = r.RelativePath,
					ObjectId = obj.Id,
					Object = obj,
				});
			}
		}

		_ = await db.SaveChangesAsync(cancellationToken);
	}
}
