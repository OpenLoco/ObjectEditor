using Dat.FileParsing;
using Definitions;
using Definitions.Database;
using Index;
using Microsoft.EntityFrameworkCore;
using System.IO.Hashing;

namespace ObjectService.Services;

/// <summary>
/// The service for <c>GameData/Objects</c>. Game objects are their own entity type: this service
/// maintains the local <see cref="ObjectIndex"/> (persisted to <c>objectIndex.json</c>) and the
/// object tables in the database.
/// </summary>
public sealed class ObjectsFolderService : GameDataFolderServiceBase
{
	private readonly ILogger _ssrLogger;

	public ObjectsFolderService(
		LocoDbContext db,
		ServerFolderManager sfm,
		ILogger<ObjectsFolderService> logger,
		ILoggerFactory loggerFactory)
		: base(db, sfm, logger)
	{
		_ssrLogger = loggerFactory.CreateLogger("SawyerStreamReader");
	}

	public override Task<GameDataImportResult> ImportAsync(string absolutePath, CancellationToken ct)
		=> ImportCoreAsync(absolutePath, ct, persistIndex: true);

	public override Task<GameDataImportResult> RemoveAsync(string absolutePath, CancellationToken ct)
		=> RemoveCoreAsync(absolutePath, ct, persistIndex: true);

	private async Task<GameDataImportResult> ImportCoreAsync(string absolutePath, CancellationToken ct, bool persistIndex)
	{
		if (!File.Exists(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File no longer exists on disk");
		}

		if (!IsDatFile(absolutePath))
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "Not a DAT file");
		}

		var relativePath = Path.GetRelativePath(Sfm.ObjectsFolder, absolutePath);

		byte[] bytes;
		try
		{
			bytes = await File.ReadAllBytesAsync(absolutePath, ct).ConfigureAwait(false);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			return new GameDataImportResult(GameDataImportStatus.Failed, $"Could not read file: {ex.Message}");
		}

		// 'Check' the dropped file: it must have valid S5 + object headers before it is added to
		// the index or the database. GetDatFileInfoFromBytes performs that check and returns null
		// when the file is not a valid DAT object.
		var entry = ObjectIndex.GetDatFileInfoFromBytes(absolutePath, relativePath, bytes, _ssrLogger);
		if (entry == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Failed, "Invalid DAT file (missing or invalid S5/object headers)");
		}

		var dbStatus = await UpsertDatabaseEntryAsync(bytes, entry, relativePath, ct).ConfigureAwait(false);
		if (dbStatus == GameDataImportStatus.Failed)
		{
			return new GameDataImportResult(GameDataImportStatus.Failed, "Could not add the object to the database", entry);
		}

		// Only update the local index once the database succeeded so the two never point at
		// different sets of objects.
		UpdateIndexEntry(entry);

		if (persistIndex)
		{
			await Sfm.ObjectIndex.SaveIndexAsync(Sfm.IndexFile).ConfigureAwait(false);
		}

		Logger.LogInformation("Indexed object {DisplayName} from \"{RelativePath}\" ({Status})", entry.DisplayName, relativePath, dbStatus);

		return new GameDataImportResult(dbStatus, $"Object {entry.DisplayName} processed", entry);
	}

	private async Task<GameDataImportResult> RemoveCoreAsync(string absolutePath, CancellationToken ct, bool persistIndex)
	{
		var entry = FindIndexEntryByFilePath(absolutePath);

		if (entry == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Skipped, "File was not present in the object index");
		}

		// Drop it from the in-memory index and persist the change so the file stops being served.
		lock (Sfm.ObjectIndex)
		{
			Sfm.ObjectIndex.RemoveEntry(entry);
		}

		if (persistIndex)
		{
			await Sfm.ObjectIndex.SaveIndexAsync(Sfm.IndexFile).ConfigureAwait(false);
		}

		if (!entry.DatChecksum.HasValue)
		{
			return new GameDataImportResult(GameDataImportStatus.Removed, $"Removed {entry.DisplayName} from the index", entry);
		}

		// Mark the database object as unavailable rather than deleting it so any metadata (authors,
		// tags, packs) curated against it is preserved.
		var datObject = await Db.DatObjects
			.Include(x => x.Object)
			.FirstOrDefaultAsync(x => x.DatName == entry.DisplayName && x.DatChecksum == entry.DatChecksum.Value, ct)
			.ConfigureAwait(false);

		if (datObject == null)
		{
			return new GameDataImportResult(GameDataImportStatus.Removed, $"Removed {entry.DisplayName} from the index (no database row)", entry);
		}

		datObject.Object.Availability = ObjectAvailability.Unavailable;
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		Logger.LogInformation("Marked object {DisplayName} as unavailable after its file was deleted", entry.DisplayName);

		return new GameDataImportResult(GameDataImportStatus.Unavailable, $"Object {entry.DisplayName} marked unavailable", entry);
	}

	/// <summary>
	/// Adds or refreshes the object's row(s) in the database. Returns
	/// <see cref="GameDataImportStatus.Added"/> for a brand new object,
	/// <see cref="GameDataImportStatus.Updated"/> when it already existed and
	/// <see cref="GameDataImportStatus.Failed"/> when it could not be parsed.
	/// </summary>
	private async Task<GameDataImportStatus> UpsertDatabaseEntryAsync(byte[] bytes, ObjectIndexEntry entry, string relativePath, CancellationToken ct)
	{
		var datName = entry.DisplayName;
		var datChecksum = entry.DatChecksum ?? 0;
		var xxHash3 = entry.xxHash3 ?? XxHash3.HashToUInt64(bytes);

		if (Db.DoesObjectExist(datName, datChecksum, out var existingObj))
		{
			// The object is already known - refresh the mapping, hashes and dates so the live
			// service reflects the (possibly relocated or re-encoded) file without a full reindex.
			var existingDat = existingObj!.DatObjects.FirstOrDefault(d => d.DatName == datName && d.DatChecksum == datChecksum);
			if (existingDat != null && existingDat.xxHash3 != xxHash3)
			{
				existingDat.xxHash3 = xxHash3;
			}

			existingObj.ModifiedDate = entry.ModifiedDate;

			// A file that reappears (e.g. restored from the Removed folder) makes the object available again.
			existingObj.Availability = ObjectAvailability.Available;

			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return GameDataImportStatus.Updated;
		}

		if (!SawyerStreamReader.TryGetHeadersFromBytes(bytes, out var hdrs, _ssrLogger))
		{
			return GameDataImportStatus.Failed;
		}

		// Match the naming convention used by the upload route ({name}_{checksum}) so duplicate
		// display names with different checksums remain unique in the Objects table.
		var objName = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}";

		var missingEntry = await Db.ObjectsMissing.FirstOrDefaultAsync(x => x.DatName == datName && x.DatChecksum == datChecksum, ct).ConfigureAwait(false);
		if (missingEntry != null)
		{
			_ = Db.ObjectsMissing.Remove(missingEntry);
		}

		var tblObject = new TblObject
		{
			Name = objName,
			Description = string.Empty,
			ObjectSource = entry.ObjectSource,
			ObjectType = entry.ObjectType,
			VehicleType = entry.VehicleType,
			Availability = ObjectAvailability.Available,
			CreatedDate = entry.CreatedDate,
			ModifiedDate = entry.ModifiedDate,
			UploadedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
			Authors = [],
			Tags = [],
			ObjectPacks = [],
			DatObjects = [],
			StringTable = [],
			Licence = null,
			OwnerUserId = null,
		};

		_ = await Db.Objects.AddAsync(tblObject, ct).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		var (_, locoObject) = SawyerStreamReader.LoadFullObject(bytes, _ssrLogger, relativePath);
		if (locoObject == null)
		{
			// Roll back the half-created object so a malformed file doesn't leave broken rows behind.
			_ = Db.Objects.Remove(tblObject);
			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);
			return GameDataImportStatus.Failed;
		}

		foreach (var s in locoObject.StringTable.Table)
		{
			foreach (var t in s.Value)
			{
				tblObject.StringTable.Add(new TblStringTableRow { Name = s.Key, Language = t.Key, Text = t.Value, ObjectId = tblObject.Id });
			}
		}

		tblObject.DatObjects.Add(new TblDatObject
		{
			ObjectId = tblObject.Id,
			DatName = datName,
			DatChecksum = datChecksum,
			xxHash3 = xxHash3,
			Object = tblObject,
		});

		_ = await DbSubObjectHelper.AddOrUpdate(Db, tblObject, locoObject.Object).ConfigureAwait(false);
		_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

		return GameDataImportStatus.Added;
	}

	/// <summary>
	/// Reassesses the Objects folder against the object index and database: stale index entries are
	/// dropped (and their database objects marked unavailable), objects missing from the database
	/// are backfilled, and DAT files that aren't in the index are imported. The index is persisted
	/// once at the end rather than per file.
	/// </summary>
	public override async Task ReconcileAsync(CancellationToken ct)
	{
		var dbPairs = await Db.DatObjects
			.AsNoTracking()
			.Select(x => new { x.DatName, x.DatChecksum })
			.ToListAsync(ct)
			.ConfigureAwait(false);
		var dbPairSet = new HashSet<(string Name, uint Checksum)>(dbPairs.Select(p => (p.DatName, p.DatChecksum)));

		List<ObjectIndexEntry> indexEntries;
		lock (Sfm.ObjectIndex)
		{
			indexEntries = [.. Sfm.ObjectIndex.Objects];
		}

		var indexedPaths = new HashSet<string>(
			indexEntries.Where(e => !string.IsNullOrEmpty(e.FileName)).Select(e => NormalizePath(ResolveObjectPath(e.FileName!))),
			StringComparer.OrdinalIgnoreCase);

		var changed = false;
		var missing = 0;
		var backfilled = 0;
		var added = 0;

		// 1. Index entries whose file has gone.
		foreach (var entry in indexEntries)
		{
			if (string.IsNullOrEmpty(entry.FileName))
			{
				continue;
			}

			var fullPath = ResolveObjectPath(entry.FileName!);
			if (File.Exists(fullPath))
			{
				continue;
			}

			var result = await RemoveCoreAsync(fullPath, ct, persistIndex: false).ConfigureAwait(false);
			if (result.Status is GameDataImportStatus.Unavailable or GameDataImportStatus.Removed)
			{
				changed = true;
				missing++;
			}
		}

		// 2. Index entries whose file exists but which are missing from the database.
		foreach (var entry in indexEntries)
		{
			if (string.IsNullOrEmpty(entry.FileName) || !entry.DatChecksum.HasValue)
			{
				continue;
			}

			var fullPath = ResolveObjectPath(entry.FileName!);
			if (!File.Exists(fullPath) || dbPairSet.Contains((entry.DisplayName, entry.DatChecksum.Value)))
			{
				continue;
			}

			var result = await ImportCoreAsync(fullPath, ct, persistIndex: false).ConfigureAwait(false);
			if (result.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed = true;
				backfilled++;
			}
		}

		// 3. DAT files on disk that are not in the index at all.
		foreach (var file in EnumerateFiles(Sfm.ObjectsFolder, IsDatFile))
		{
			if (indexedPaths.Contains(NormalizePath(file)))
			{
				continue;
			}

			var result = await ImportCoreAsync(file, ct, persistIndex: false).ConfigureAwait(false);
			if (result.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed = true;
				added++;
			}
		}

		if (changed)
		{
			await Sfm.ObjectIndex.SaveIndexAsync(Sfm.IndexFile).ConfigureAwait(false);
		}

		Logger.LogInformation(
			"Objects reconciliation complete: added={Added}, database backfilled={Backfilled}, marked unavailable={Missing}",
			added, backfilled, missing);
	}

	/// <summary>
	/// Finds the index entry whose stored filename (absolute for uploaded objects, relative for
	/// scanned ones) resolves to <paramref name="absolutePath"/>.
	/// </summary>
	private ObjectIndexEntry? FindIndexEntryByFilePath(string absolutePath)
	{
		var fullPath = Path.GetFullPath(absolutePath);

		lock (Sfm.ObjectIndex)
		{
			foreach (var entry in Sfm.ObjectIndex.Objects.ToList())
			{
				if (string.IsNullOrEmpty(entry.FileName))
				{
					continue;
				}

				if (string.Equals(Path.GetFullPath(ResolveObjectPath(entry.FileName)), fullPath, StringComparison.OrdinalIgnoreCase))
				{
					return entry;
				}
			}
		}

		return null;
	}

	/// <summary>
	/// Adds the entry to the in-memory index, replacing any existing entry for the same content
	/// (matched by xxHash3 first, then by dat name + checksum) so a moved or renamed file doesn't
	/// leave a stale path behind.
	/// </summary>
	private void UpdateIndexEntry(ObjectIndexEntry entry)
	{
		var index = Sfm.ObjectIndex;
		lock (index)
		{
			ObjectIndexEntry? existing = null;

			if (entry.xxHash3.HasValue)
			{
				_ = index.TryFind(entry.xxHash3.Value, out existing);
			}

			if (existing == null && entry.DatChecksum.HasValue)
			{
				_ = index.TryFind((entry.DisplayName, entry.DatChecksum.Value), out existing);
			}

			if (existing != null)
			{
				index.RemoveEntry(existing);
			}

			index.AddEntry(entry);
		}
	}

	private string ResolveObjectPath(string fileName)
		=> Path.IsPathRooted(fileName) ? fileName : Path.Combine(Sfm.ObjectsFolder, fileName);

	private static string NormalizePath(string path)
		=> Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
}
