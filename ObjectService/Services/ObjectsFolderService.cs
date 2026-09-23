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
		// different sets of objects. A duplicate (identical content to an existing file) is indexed
		// too, so the file stays served and the reconcile does not re-read it on every startup.
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

		// The same file content can be represented on disk more than once (duplicate files are collapsed
		// to the oldest, but the index can transiently hold several). If an identical file is still
		// present the object stays available - only the file mapping was dropped.
		if (entry.xxHash3.HasValue && IsContentStillOnDisk(entry.xxHash3.Value))
		{
			return new GameDataImportResult(
				GameDataImportStatus.Removed,
				$"Removed {entry.DisplayName} from the index (identical content is still present)",
				entry);
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

		// xxHash3 is the authoritative identity of a file. If we already hold a file with this exact
		// content, this one is a duplicate: keep the oldest (the existing object) and add nothing.
		if (Db.DoesObjectWithHashExist(xxHash3, out var duplicateObj))
		{
			duplicateObj!.ModifiedDate = entry.ModifiedDate;

			// A file that reappears (e.g. restored from the Removed folder) makes the object available again.
			duplicateObj.Availability = ObjectAvailability.Available;

			_ = await Db.SaveChangesAsync(ct).ConfigureAwait(false);

			Logger.LogInformation(
				"File \"{RelativePath}\" is a duplicate (xxHash3={XxHash3}); keeping the existing object {ObjectId}",
				relativePath, xxHash3, duplicateObj.Id);

			return GameDataImportStatus.Duplicate;
		}

		if (!SawyerStreamReader.TryGetHeadersFromBytes(bytes, out var hdrs, _ssrLogger))
		{
			return GameDataImportStatus.Failed;
		}

		// The (DatName, DatChecksum) pair is not unique: a binary-different file may carry the same S5
		// name and checksum. Name the object from that pair, disambiguating with the whole-file hash
		// when the name is already taken so the unique Objects.Name constraint still holds.
		var objName = await Db.GetUniqueObjectNameAsync(hdrs.S5.Name, hdrs.S5.Checksum, xxHash3, ct).ConfigureAwait(false);

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
		var duplicates = 0;

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

			var result = await TryReconcileFileAsync(fullPath, () => RemoveCoreAsync(fullPath, ct, persistIndex: false), ct).ConfigureAwait(false);
			if (result?.Status is GameDataImportStatus.Unavailable or GameDataImportStatus.Removed)
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

			var result = await TryReconcileFileAsync(fullPath, () => ImportCoreAsync(fullPath, ct, persistIndex: false), ct).ConfigureAwait(false);
			if (result?.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed = true;
				backfilled++;
			}
			else if (result?.Status is GameDataImportStatus.Duplicate)
			{
				duplicates++;
			}
		}

		// 3. DAT files on disk that are not in the index at all.
		foreach (var file in EnumerateFiles(Sfm.ObjectsFolder, IsDatFile))
		{
			if (indexedPaths.Contains(NormalizePath(file)))
			{
				continue;
			}

			var result = await TryReconcileFileAsync(file, () => ImportCoreAsync(file, ct, persistIndex: false), ct).ConfigureAwait(false);
			if (result?.Status is GameDataImportStatus.Added or GameDataImportStatus.Updated)
			{
				changed = true;
				added++;
			}
			else if (result?.Status is GameDataImportStatus.Duplicate)
			{
				duplicates++;
			}
		}

		if (changed)
		{
			await Sfm.ObjectIndex.SaveIndexAsync(Sfm.IndexFile).ConfigureAwait(false);
		}

		Logger.LogInformation(
			"Objects reconciliation complete: added={Added}, database backfilled={Backfilled}, marked unavailable={Missing}, duplicates ignored={Duplicates}",
			added, backfilled, missing, duplicates);
	}

	/// <summary>
	/// Returns <see langword="true"/> when any file still in the index has the same whole-file content
	/// hash and is present on disk.
	/// </summary>
	private bool IsContentStillOnDisk(ulong xxHash3)
	{
		lock (Sfm.ObjectIndex)
		{
			return Sfm.ObjectIndex.Objects.Any(e =>
				e.xxHash3 == xxHash3
				&& !string.IsNullOrEmpty(e.FileName)
				&& File.Exists(ResolveObjectPath(e.FileName!)));
		}
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
