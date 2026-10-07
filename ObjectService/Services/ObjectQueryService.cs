using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Definitions.SourceData;
using Definitions.Web;
using Index;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;
using System.IO.Hashing;

namespace ObjectService.Services;

/// <summary>
/// The outcome of an object upload, allowing the route handler to translate validation
/// failures into the appropriate HTTP response.
/// </summary>
public record UploadResult(bool Success, DtoObjectPostResponse? Descriptor, string? ErrorMessage, int StatusCode);

/// <summary>The outcome of removing an object.</summary>
public enum ObjectDeleteOutcome
{
	/// <summary>The object is no longer available and its files were parked under <c>Removed</c>.</summary>
	Removed,

	/// <summary>No object with the given id exists.</summary>
	NotFound,

	/// <summary>The object cannot be removed (vanilla Locomotion assets).</summary>
	Forbidden,
}

/// <summary>
/// Result of removing an object. The database row is intentionally kept (marked
/// <see cref="ObjectAvailability.Unavailable"/>) so curated metadata and pack/scenario references survive;
/// <see cref="RemovedFiles"/> lists the files moved into the category's <c>Removed</c> folder.
/// </summary>
public record ObjectDeleteResult(ObjectDeleteOutcome Outcome, string? ErrorMessage = null, IReadOnlyList<string>? RemovedFiles = null);

/// <summary>The outcome of replacing an object with <c>PUT</c>.</summary>
public enum ObjectUpdateOutcome
{
	/// <summary>The stored object now matches the request.</summary>
	Updated,

	/// <summary>No object with the given id exists.</summary>
	NotFound,

	/// <summary>The object cannot be updated (vanilla Locomotion assets).</summary>
	Forbidden,

	/// <summary>The request cannot be applied - no name, or a sub-object that is not the declared type's.</summary>
	InvalidRequest,

	/// <summary>Another object already uses the requested name.</summary>
	NameConflict,
}

/// <summary>
/// Result of replacing an object, allowing the route handler to translate validation failures into the
/// appropriate HTTP response.
/// </summary>
public record ObjectUpdateResult(ObjectUpdateOutcome Outcome, DtoObjectPostResponse? Descriptor = null, string? ErrorMessage = null);

/// <summary>
/// The outcome of resolving one of an object's resources (its images or its DAT file). <see cref="Forbidden"/>
/// means the object exists but its source/availability forbids exposing the resource (vanilla Locomotion
/// assets and unavailable objects).
/// </summary>
public enum ObjectResourceOutcome
{
	Ok,
	NotFound,
	Forbidden,
}

/// <summary>Rendered image bytes plus the content version (the source file's xxHash3) used for caching.</summary>
public record ObjectImageResult(ObjectResourceOutcome Outcome, byte[]? Bytes = null, ulong Version = 0);

/// <summary>Rendered image-table metadata plus the content version (xxHash3).</summary>
public record ObjectImageMetadataResult(ObjectResourceOutcome Outcome, ObjectImageMetadata? Metadata = null, ulong Version = 0);

/// <summary>The primary DAT file path, or the reason it cannot be exposed.</summary>
public record ObjectFileResult(ObjectResourceOutcome Outcome, string? FilePath = null);

public interface IObjectQueryService
{
	Task<IEnumerable<DtoObjectEntry>> ListAsync(CancellationToken ct);
	Task<IEnumerable<DtoObjectEntry>> ListMineAsync(UniqueObjectId ownerUserId, CancellationToken ct);

	/// <summary>
	/// The object descriptor. When <paramref name="includeDatBytes"/> is <see langword="true"/> the raw DAT
	/// bytes are base64-attached to each DatObject (a full file read + base64 encode); callers that only
	/// need metadata (e.g. the web frontend) should pass <see langword="false"/>.
	/// </summary>
	Task<DtoObjectPostResponse?> GetByIdAsync(UniqueObjectId id, bool includeDatBytes, CancellationToken ct);

	Task<UploadResult> UploadDatAsync(DtoObjectPost request, CancellationToken ct);
	Task<ObjectUpdateResult> UpdateAsync(UniqueObjectId id, DtoObjectPostResponse request, CancellationToken ct);
	Task<ObjectDeleteResult> DeleteObjectAsync(UniqueObjectId id, CancellationToken ct);

	/// <summary>The primary DAT file path, or the reason it cannot be exposed.</summary>
	Task<ObjectFileResult> GetFilePathAsync(UniqueObjectId id, CancellationToken ct);
}

public class ObjectQueryService : IObjectQueryService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<ObjectQueryService> _logger;
	private readonly ILoggerFactory _loggerFactory;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<TblUser> _userManager;
	private readonly IObjectImageCache _imageCache;

	public ObjectQueryService(
		LocoDbContext db,
		ServerFolderManager sfm,
		ILogger<ObjectQueryService> logger,
		ILoggerFactory loggerFactory,
		IHttpContextAccessor httpContextAccessor,
		UserManager<TblUser> userManager,
		IObjectImageCache imageCache)
	{
		_db = db;
		_sfm = sfm;
		_logger = logger;
		_loggerFactory = loggerFactory;
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
		_imageCache = imageCache;
	}

	public async Task<IEnumerable<DtoObjectEntry>> ListAsync(CancellationToken ct) => await _db.Objects.AsNoTracking().Include(x => x.DatObjects).Select(x => x.ToDtoEntry()).ToListAsync(ct);

	public async Task<IEnumerable<DtoObjectEntry>> ListMineAsync(UniqueObjectId ownerUserId, CancellationToken ct)
		=> await _db.Objects
			.AsNoTracking()
			.Where(x => x.OwnerUserId == ownerUserId)
			.Include(x => x.DatObjects)
			.Select(x => x.ToDtoEntry())
			.ToListAsync(ct);

	public async Task<DtoObjectPostResponse?> GetByIdAsync(UniqueObjectId id, bool includeDatBytes, CancellationToken ct)
	{
		var eObj = await _db.Objects.Where(x => x.Id == id).Include(x => x.Licence).Include(x => x.DatObjects).Include(x => x.StringTable).Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks)).SingleOrDefaultAsync(ct);
		if (eObj == null)
		{
			return null;
		}

		var subObject = DbSubObjectHelper.GetDbSubForType(_db, eObj.Object.ObjectType, eObj.Object.Id);
		var descriptor = eObj.ToDtoDescriptor(subObject);
		if (includeDatBytes)
		{
			await PopulateDatFileBytesAsync(descriptor, ct);
		}

		return descriptor;
	}

	/// <summary>
	/// Attaches the raw DAT file bytes (base64-encoded) to each of the descriptor's DatObjects so
	/// clients can download the object in one round-trip. Vanilla (Locomotion Steam/GoG) and
	/// unavailable objects never get their bytes attached - distributing the copyrighted originals
	/// is not permitted - so their DatBytesAsBase64 stays null and clients fall back to metadata-only.
	/// </summary>
	async Task PopulateDatFileBytesAsync(DtoObjectPostResponse descriptor, CancellationToken ct)
	{
		if (!ObjectAvailabilityRules.IsDownloadable(descriptor.ObjectSource, descriptor.Availability))
		{
			return;
		}

		foreach (var datObject in descriptor.DatObjects)
		{
			if (!_sfm.ObjectIndex.TryFind((datObject.DatName, datObject.DatChecksum), out var indexEntry) || indexEntry == null || string.IsNullOrEmpty(indexEntry.FileName))
			{
				continue;
			}

			if (!RouteHelpers.TryGetSafePathUnderRoot(_sfm.ObjectsFolder, indexEntry.FileName, out var objectFilePath, out _)
				|| !File.Exists(objectFilePath))
			{
				continue;
			}

			var datBytes = await File.ReadAllBytesAsync(objectFilePath, ct);
			datObject.DatBytesAsBase64 = Convert.ToBase64String(datBytes);
		}
	}

	public async Task<ObjectUpdateResult> UpdateAsync(UniqueObjectId id, DtoObjectPostResponse request, CancellationToken ct)
	{
		// PUT semantics: the stored object is made to match the request. Every column of the header row
		// (Objects) and of the object's sub-object table is taken from the request. The one thing a request
		// cannot change is which DAT file(s) the object is built from: the DatObjects rows describe the file
		// on disk, which a web request has no business reassigning.
		var obj = await _db.Objects.Include(x => x.Licence).Include(x => x.Authors).Include(x => x.Tags).Include(x => x.ObjectPacks).Include(x => x.DatObjects).Include(x => x.StringTable).Where(x => x.Id == id).SingleOrDefaultAsync(ct);
		if (obj == null)
		{
			return new(ObjectUpdateOutcome.NotFound);
		}

		// Vanilla game objects (original Locomotion assets) can NEVER be edited by anyone,
		// regardless of role or ownership. This is a defense-in-depth check alongside
		// the ObjectOwnershipHandler authorization.
		if (obj.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			_logger.LogWarning("Attempt to update vanilla object {ObjectId} was blocked", id);
			return new(ObjectUpdateOutcome.Forbidden, ErrorMessage: "Vanilla Locomotion objects cannot be edited.");
		}

		// Everything is validated before anything is mutated so that a rejected request cannot half-apply.
		if (string.IsNullOrWhiteSpace(request.Name))
		{
			return new(ObjectUpdateOutcome.InvalidRequest, ErrorMessage: "A name is required.");
		}

		if (request.SubObject is not null && !DbSubObjectHelper.IsSubObjectOfType(request.SubObject, request.ObjectType))
		{
			_logger.LogWarning("Object {ObjectId} update rejected: {SubObjectDto} does not belong to {ObjectType}", id, request.SubObject.GetType().Name, request.ObjectType);
			return new(ObjectUpdateOutcome.InvalidRequest, ErrorMessage: $"{request.SubObject.GetType().Name} is not the sub-object of ObjectType.{request.ObjectType}.");
		}

		if (!string.Equals(obj.Name, request.Name, StringComparison.Ordinal) && await _db.Objects.AnyAsync(x => x.Id != id && x.Name == request.Name, ct))
		{
			return new(ObjectUpdateOutcome.NameConflict, ErrorMessage: $"An object named '{request.Name}' already exists.");
		}

		var previousObjectType = obj.ObjectType;

		obj.Name = request.Name;
		obj.ObjectType = request.ObjectType;

		// The object source is server-owned: it tells us where the object came from, and only the server
		// knows that. Uploads always store Custom, while Steam/GoG/OpenLoco objects are placed in the
		// server folders by hand, so no client can move an object between sources.
		if (request.ObjectSource != obj.ObjectSource)
		{
			_logger.LogWarning("Object {ObjectId} source is server-owned; ignoring the requested {RequestedSource} (stored: {ObjectSource})", id, request.ObjectSource, obj.ObjectSource);
		}

		obj.VehicleType = request.VehicleType;
		obj.Description = request.Description;
		obj.CreatedDate = request.CreatedDate;
		obj.ModifiedDate = request.ModifiedDate;

		// Availability is only meaningful for Custom content: vanilla Locomotion and OpenLoco objects are
		// never available, so a request cannot mark them so.
		obj.Availability = obj.ObjectSource == ObjectSource.Custom
			? request.Availability
			: ObjectAvailability.Unavailable;

		if (request.Licence == null)
		{
			obj.Licence = null;
		}
		else
		{
			obj.Licence = await _db.Licences.SingleOrDefaultAsync(l => l.Id == request.Licence.Id, ct);
		}

		if (request.Authors == null || request.Authors.Count == 0)
		{
			obj.Authors.Clear();
		}
		else
		{
			var ids = request.Authors.Select(a => a.Id).ToList();
			var items = await _db.Authors.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
			obj.Authors.Clear();
			foreach (var i in items)
			{
				obj.Authors.Add(i);
			}
		}

		if (request.Tags == null || request.Tags.Count == 0)
		{
			obj.Tags.Clear();
		}
		else
		{
			var ids = request.Tags.Select(t => t.Id).ToList();
			var items = await _db.Tags.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
			obj.Tags.Clear();
			foreach (var i in items)
			{
				obj.Tags.Add(i);
			}
		}

		if (request.ObjectPacks == null || request.ObjectPacks.Count == 0)
		{
			obj.ObjectPacks.Clear();
		}
		else
		{
			var ids = request.ObjectPacks.Select(p => p.Id).ToList();
			var items = await _db.ObjectPacks.Where(x => ids.Contains(x.Id)).ToListAsync(ct);
			obj.ObjectPacks.Clear();
			foreach (var i in items)
			{
				obj.ObjectPacks.Add(i);
			}
		}

		// String-table rows are stored one per name+language. A PUT replaces the whole resource, so the
		// descriptor's rows become the object's rows (an empty table clears them).
		SyncStringTable(obj, request.StringTable);

		// A PUT replaces the resource, so the sub-object becomes exactly what the request says: supplied data
		// is applied, and an omitted sub-object removes the existing row. The row belongs to the object's
		// current type, so changing the type must not leave the old type's row behind.
		if (previousObjectType != obj.ObjectType && await DbSubObjectHelper.GetSubObjectRowAsync(_db, previousObjectType, obj.Id) is { } previousSubObject)
		{
			_logger.LogInformation("Object {ObjectId} changed type from {PreviousType} to {ObjectType}; removing its {PreviousType} sub-object row", id, previousObjectType, obj.ObjectType, previousObjectType);
			_ = _db.Remove(previousSubObject);
		}

		if (request.SubObject is not null)
		{
			var subObjectEntity = SubObjectDtoMapper.ToTableEntity(request.SubObject, obj);
			_ = await DbSubObjectHelper.AddOrUpdate(_db, obj, subObjectEntity);
		}
		else if (previousObjectType == obj.ObjectType && await DbSubObjectHelper.GetSubObjectRowAsync(_db, obj.ObjectType, obj.Id) is { } existingSubObject)
		{
			_ = _db.Remove(existingSubObject);
		}

		try
		{
			_ = await _db.SaveChangesAsync(ct);
		}
		catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueConstraintViolation(ex))
		{
			// The pre-check above can lose a race with a concurrent rename.
			_logger.LogWarning(ex, "Object {ObjectId} update rejected: name '{Name}' is already taken", id, request.Name);
			return new(ObjectUpdateOutcome.NameConflict, ErrorMessage: $"An object named '{request.Name}' already exists.");
		}

		var expandedObj = new ExpandedTbl<TblObject, TblObjectPack>(obj, obj.Authors, obj.Tags, obj.ObjectPacks);
		var subObject = DbSubObjectHelper.GetDbSubForType(_db, obj.ObjectType, obj.Id);
		return new(ObjectUpdateOutcome.Updated, expandedObj.ToDtoDescriptor(subObject));
	}

	/// <summary>
	/// Replaces the object's string-table rows with the supplied descriptor (PUT semantics). Rows are
	/// matched by name and language: existing rows are updated, new rows are added, and every row the
	/// request omits is removed — so a descriptor with no rows clears the string table, and clients are
	/// expected to send the rows they want to keep.
	/// </summary>
	static void SyncStringTable(TblObject obj, DtoStringTableDescriptor? descriptor)
	{
		var requested = descriptor?.Table ?? new Dictionary<string, Dictionary<LanguageId, string>>();

		// A duplicate name+language row would be a legacy artefact; keep the first.
		var existing = obj.StringTable
			.GroupBy(row => (row.Name, row.Language))
			.ToDictionary(group => group.Key, group => group.First());

		foreach (var (name, languages) in requested)
		{
			foreach (var (language, text) in languages)
			{
				if (existing.TryGetValue((name, language), out var row))
				{
					row.Text = text;
				}
				else
				{
					obj.StringTable.Add(new TblStringTableRow
					{
						Name = name,
						Language = language,
						Text = text,
						ObjectId = obj.Id,
					});
				}
			}
		}

		var requestedKeys = requested
			.SelectMany(entry => entry.Value.Keys.Select(language => (entry.Key, language)))
			.ToHashSet();

		foreach (var row in obj.StringTable.Where(row => !requestedKeys.Contains((row.Name, row.Language))).ToList())
		{
			_ = obj.StringTable.Remove(row);
		}
	}

	public async Task<ObjectFileResult> GetFilePathAsync(UniqueObjectId id, CancellationToken ct)
	{
		var (_, dat, outcome) = await ObjectResourceResolver.ResolvePrimaryDatAsync(_db, id, ct).ConfigureAwait(false);
		if (outcome != ObjectResourceOutcome.Ok)
		{
			return new(outcome);
		}

		if (dat == null || !ObjectResourceResolver.TryResolveObjectFilePath(_sfm, dat, out var objectFilePath))
		{
			return new(ObjectResourceOutcome.NotFound);
		}

		return new(ObjectResourceOutcome.Ok, objectFilePath);
	}

	public async Task<UploadResult> UploadDatAsync(DtoObjectPost request, CancellationToken ct)
	{
		if (string.IsNullOrEmpty(request.DatBytesAsBase64))
		{
			return new UploadResult(false, null, "DatBytesAsBase64 cannot be null", 400);
		}

		byte[]? datFileBytes;
		try
		{
			datFileBytes = Convert.FromBase64String(request.DatBytesAsBase64);
		}
		catch (FormatException ex)
		{
			return new UploadResult(false, null, ex.Message, 400);
		}

		if (datFileBytes == null || datFileBytes.Length == 0)
		{
			return new UploadResult(false, null, "Decoded bytes are empty", 400);
		}

		if (datFileBytes.Length > ServerLimits.MaximumUploadFileSize)
		{
			return new UploadResult(false, null, $"Uploads limited to {ServerLimits.MaximumUploadFileSize / (1024 * 1024)}MB", 413);
		}

		var ssrLogger = _loggerFactory.CreateLogger("SawyerStreamReader");
		if (!SawyerStreamReader.TryGetHeadersFromBytes(datFileBytes, out var hdrs, ssrLogger))
		{
			return new UploadResult(false, null, "Invalid dat file headers", 400);
		}

		if (!hdrs.S5.IsValid() || !hdrs.Obj.IsValid())
		{
			return new UploadResult(false, null, "Invalid DAT file", 400);
		}

		// Work out what the file actually is. Modders routinely label their own work as "vanilla" to
		// dodge the in-game restrictions, and the header cannot be rewritten (that would change the
		// checksum), so vanilla (and custom) uploads are accepted and stored in the Custom folder as
		// custom objects. Genuine OpenLoco objects are refused outright.
		var detectedSource = OriginalObjectFiles.GetFileSource(hdrs.S5.Name, hdrs.S5.Checksum, hdrs.S5.ObjectSource);
		if (detectedSource is ObjectSource.OpenLoco)
		{
			_logger.LogWarning("Rejected upload \"{Name}\" ({Checksum}): its content identifies it as an OpenLoco object", hdrs.S5.Name, hdrs.S5.Checksum);
			return new UploadResult(false, null, "Uploading OpenLoco objects is not allowed", 400);
		}

		_logger.LogInformation("Accepted upload \"{Name}\" ({Checksum}); detected source {Source}, storing it in the Custom folder", hdrs.S5.Name, hdrs.S5.Checksum, detectedSource);

		// xxHash3 over the whole file is the authoritative identity: identical content means the object
		// already exists regardless of its S5 name/checksum. A binary-different file that happens to
		// share the S5 name/checksum is a genuinely different object and is imported as one.
		var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		if (_db.DoesObjectWithHashExist(xxHash3, out var existingObj))
		{
			return new UploadResult(false, null, $"Object with identical content already exists. UploadedDate={existingObj!.UploadedDate}", 202);
		}

		var missingEntry = await _db.ObjectsMissing.FirstOrDefaultAsync(x => x.DatName == hdrs.S5.Name && x.DatChecksum == hdrs.S5.Checksum, ct);
		if (missingEntry != null)
		{
			_ = _db.ObjectsMissing.Remove(missingEntry);
			_ = await _db.SaveChangesAsync(ct);
		}

		var (DatFileInfo, LocoObject) = SawyerStreamReader.LoadFullObject(datFileBytes, ssrLogger);
		if (LocoObject == null)
		{
			return new UploadResult(false, null, "Could not parse DAT object", 400);
		}

		var uuid = Guid.NewGuid();
		// Index entries always store a path relative to the Objects folder (the same convention used
		// for scanned files), so the absolute path is only needed when writing the file.
		var relativeFileName = ServerFolderManager.GetCustomObjectRelativeFileName(uuid);
		await File.WriteAllBytesAsync(Path.Combine(_sfm.ObjectsFolder, relativeFileName), datFileBytes, ct);

		VehicleType? vehicleType = null;
		if (LocoObject.Object is VehicleObject veh)
		{
			vehicleType = veh.Type;
		}

		// Determine the owner user if the request is authenticated
		UniqueObjectId? ownerUserId = null;
		var currentUser = _httpContextAccessor.HttpContext?.User;
		if (currentUser?.Identity?.IsAuthenticated == true)
		{
			var user = await _userManager.GetUserAsync(currentUser);
			if (user != null)
			{
				ownerUserId = user.Id;
			}
		}

		var objName = await _db.GetUniqueObjectNameAsync(hdrs.S5.Name, hdrs.S5.Checksum, xxHash3, ct);

		var tblObject = new TblObject()
		{
			Name = objName,
			Description = string.Empty,
			ObjectSource = ObjectSource.Custom,
			ObjectType = hdrs.S5.ObjectType.Convert(),
			VehicleType = vehicleType,
			Availability = request.InitialAvailability,
			CreatedDate = request.CreatedDate,
			ModifiedDate = request.ModifiedDate,
			UploadedDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
			Authors = [],
			Tags = [],
			ObjectPacks = [],
			DatObjects = [],
			StringTable = [],
			Licence = null,
			OwnerUserId = ownerUserId,
		};

		_ = await _db.Objects.AddAsync(tblObject, ct);
		_ = await _db.SaveChangesAsync(ct);

		foreach (var s in LocoObject.StringTable.Table)
		{
			foreach (var t in s.Value)
			{
				tblObject.StringTable.Add(new TblStringTableRow() { Name = s.Key, Language = t.Key, Text = t.Value, ObjectId = tblObject.Id, });
			}
		}

		tblObject.DatObjects.Add(new TblDatObject() { ObjectId = tblObject.Id, DatName = DatFileInfo.S5Header.Name, DatChecksum = DatFileInfo.S5Header.Checksum, xxHash3 = xxHash3, Object = tblObject, });

		_ = await DbSubObjectHelper.AddOrUpdate(_db, tblObject, LocoObject.Object);
		_ = await _db.SaveChangesAsync(ct);

		// The object is fully decoded at this point, so warm the image cache now (thumbnail + metadata) to
		// spare the first browse/detail request from decoding it again.
		if (LocoObject.ImageTable is { } imageTable)
		{
			await ObjectImageRender.WarmAsync(_imageCache, xxHash3, imageTable.GraphicsElements, ct);
		}

		_sfm.ObjectIndex.AddEntry(new ObjectIndexEntry(hdrs.S5.Name, relativeFileName, tblObject.Id, hdrs.S5.Checksum, xxHash3, tblObject.ObjectType, tblObject.ObjectSource, tblObject.CreatedDate, tblObject.UploadedDate, tblObject.VehicleType));
		_ = _sfm.ObjectIndex.SaveIndexAsync(_sfm.IndexFile);

		var subObject = DbSubObjectHelper.GetDbSubForType(_db, tblObject.ObjectType, tblObject.Id);
		var response = new ExpandedTbl<TblObject, TblObjectPack>(tblObject, [], [], []).ToDtoDescriptor(subObject);
		return new UploadResult(true, response, null, 201);
	}

	/// <summary>
	/// "Deletes" an object by moving its DAT file(s) into <c>GameData/Objects/Removed</c>, dropping their
	/// object-index entries and marking the row <see cref="ObjectAvailability.Unavailable"/>. The row itself
	/// is kept so metadata, tags, packs and scenario references survive, and the file is kept so the removal
	/// is recoverable. <c>Removed</c> is ignored by the file watchers, so the object is never re-imported.
	/// </summary>
	public async Task<ObjectDeleteResult> DeleteObjectAsync(UniqueObjectId id, CancellationToken ct)
	{
		var obj = await _db.Objects
			.Where(x => x.Id == id)
			.Include(x => x.DatObjects)
			.SingleOrDefaultAsync(ct);

		if (obj is null)
		{
			return new ObjectDeleteResult(ObjectDeleteOutcome.NotFound);
		}

		if (obj.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			return new ObjectDeleteResult(ObjectDeleteOutcome.Forbidden, "Vanilla Locomotion objects cannot be removed");
		}

		// Resolve the files behind this object's DAT entries before anything is moved.
		var entries = new List<(ObjectIndexEntry Entry, string FullPath)>();
		foreach (var dat in obj.DatObjects)
		{
			if (!_sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry)
				|| entry?.FileName is null
				|| !RouteHelpers.TryGetSafePathUnderRoot(_sfm.ObjectsFolder, entry.FileName, out var fullPath, out _))
			{
				continue;
			}

			entries.Add((entry, fullPath));
		}

		var removedFiles = new List<string>();
		foreach (var (_, fullPath) in entries)
		{
			if (ServerFolderManager.MoveToRemovedFolder(_sfm.ObjectsFolder, fullPath) is { } moved)
			{
				removedFiles.Add(moved);
			}
		}

		lock (_sfm.ObjectIndex)
		{
			foreach (var (entry, _) in entries)
			{
				_sfm.ObjectIndex.RemoveEntry(entry);
			}
		}

		if (entries.Count > 0)
		{
			await _sfm.ObjectIndex.SaveIndexAsync(_sfm.IndexFile);
		}

		obj.Availability = ObjectAvailability.Unavailable;
		_ = await _db.SaveChangesAsync(ct);

		_logger.LogInformation(
			"Removed object {ObjectId} ({Name}): marked unavailable and parked {Count} file(s) under the Objects Removed folder",
			obj.Id, obj.Name, removedFiles.Count);

		return new ObjectDeleteResult(ObjectDeleteOutcome.Removed, null, removedFiles);
	}
}