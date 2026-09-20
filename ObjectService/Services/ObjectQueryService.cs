using Dat.Converters;
using Dat.FileParsing;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Definitions.SourceData;
using Definitions.Web;
using Index;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.IO.Compression;
using System.IO.Hashing;

namespace ObjectService.Services;

/// <summary>
/// The outcome of an object upload, allowing the route handler to translate validation
/// failures into the appropriate HTTP response.
/// </summary>
public record UploadResult(bool Success, DtoObjectPostResponse? Descriptor, string? ErrorMessage, int StatusCode);

public interface IObjectQueryService
{
	Task<IEnumerable<DtoObjectEntry>> ListAsync(CancellationToken ct);
	Task<IEnumerable<DtoObjectEntry>> ListMineAsync(UniqueObjectId ownerUserId, CancellationToken ct);
	Task<DtoObjectPostResponse?> GetByIdAsync(UniqueObjectId id, CancellationToken ct);
	Task<UploadResult> UploadDatAsync(DtoObjectPost request, CancellationToken ct);
	Task<DtoObjectPostResponse?> UpdateAsync(UniqueObjectId id, DtoObjectPostResponse request, CancellationToken ct);
	Task<byte[]?> GetImagesZipAsync(UniqueObjectId id, CancellationToken ct);
	Task<byte[]?> GetImagePngAsync(UniqueObjectId id, int imageId, CancellationToken ct);
	Task<string?> GetFilePathAsync(UniqueObjectId id, CancellationToken ct);
}

public class ObjectQueryService : IObjectQueryService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;
	private readonly ILogger<ObjectQueryService> _logger;
	private readonly ILoggerFactory _loggerFactory;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<TblUser> _userManager;

	public ObjectQueryService(
		LocoDbContext db,
		ServerFolderManager sfm,
		ILogger<ObjectQueryService> logger,
		ILoggerFactory loggerFactory,
		IHttpContextAccessor httpContextAccessor,
		UserManager<TblUser> userManager)
	{
		_db = db;
		_sfm = sfm;
		_logger = logger;
		_loggerFactory = loggerFactory;
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
	}

	public async Task<IEnumerable<DtoObjectEntry>> ListAsync(CancellationToken ct) => await _db.Objects.Include(x => x.DatObjects).Select(x => x.ToDtoEntry()).ToListAsync(ct);

	public async Task<IEnumerable<DtoObjectEntry>> ListMineAsync(UniqueObjectId ownerUserId, CancellationToken ct)
		=> await _db.Objects
			.Where(x => x.OwnerUserId == ownerUserId)
			.Include(x => x.DatObjects)
			.Select(x => x.ToDtoEntry())
			.ToListAsync(ct);

	public async Task<DtoObjectPostResponse?> GetByIdAsync(UniqueObjectId id, CancellationToken ct)
	{
		var eObj = await _db.Objects.Where(x => x.Id == id).Include(x => x.Licence).Include(x => x.DatObjects).Include(x => x.StringTable).Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks)).SingleOrDefaultAsync(ct);
		if (eObj == null)
		{
			return null;
		}

		var subObject = DbSubObjectHelper.GetDbSubForType(_db, eObj.Object.ObjectType, eObj.Object.Id);
		var descriptor = eObj.ToDtoDescriptor(subObject);
		await PopulateDatFileBytesAsync(descriptor, ct);
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
		if (descriptor.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG || descriptor.Availability == ObjectAvailability.Unavailable)
		{
			return;
		}

		foreach (var datObject in descriptor.DatObjects)
		{
			if (!_sfm.ObjectIndex.TryFind((datObject.DatName, datObject.DatChecksum), out var indexEntry) || indexEntry == null || string.IsNullOrEmpty(indexEntry.FileName))
			{
				continue;
			}

			var objectFilePath = Path.Combine(_sfm.ObjectsFolder, indexEntry.FileName);
			if (!File.Exists(objectFilePath))
			{
				continue;
			}

			var datBytes = await File.ReadAllBytesAsync(objectFilePath, ct);
			datObject.DatBytesAsBase64 = Convert.ToBase64String(datBytes);
		}
	}

	public async Task<DtoObjectPostResponse?> UpdateAsync(UniqueObjectId id, DtoObjectPostResponse request, CancellationToken ct)
	{
		var obj = await _db.Objects.Include(x => x.Licence).Include(x => x.Authors).Include(x => x.Tags).Include(x => x.ObjectPacks).Include(x => x.DatObjects).Include(x => x.StringTable).Where(x => x.Id == id).SingleOrDefaultAsync(ct);
		if (obj == null)
		{
			return null;
		}

		// Vanilla game objects (original Locomotion assets) can NEVER be edited by anyone,
		// regardless of role or ownership. This is a defense-in-depth check alongside
		// the ObjectOwnershipHandler authorization.
		if (obj.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
		{
			_logger.LogWarning("Attempt to update vanilla object {ObjectId} was blocked", id);
			return null;
		}

		obj.Description = request.Description;
		obj.CreatedDate = request.CreatedDate;
		obj.ModifiedDate = request.ModifiedDate;
		obj.Availability = request.Availability;

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

		_ = await _db.SaveChangesAsync(ct);
		var expandedObj = new ExpandedTbl<TblObject, TblObjectPack>(obj, obj.Authors, obj.Tags, obj.ObjectPacks);
		var subObject = DbSubObjectHelper.GetDbSubForType(_db, obj.ObjectType, obj.Id);
		return expandedObj.ToDtoDescriptor(subObject);
	}

	public async Task<byte[]?> GetImagesZipAsync(UniqueObjectId id, CancellationToken ct)
	{
		var obj = await _db.Objects.AsNoTracking().Include(x => x.DatObjects).SingleOrDefaultAsync(x => x.Id == id, ct);
		if (obj == null)
		{
			return null;
		}

		var datEntry = obj.DatObjects.FirstOrDefault();
		if (datEntry == null || !_sfm.ObjectIndex.TryFind((datEntry.DatName, datEntry.DatChecksum), out var indexEntry) || indexEntry == null || string.IsNullOrEmpty(indexEntry.FileName))
		{
			return null;
		}

		var objectFilePath = Path.Combine(_sfm.ObjectsFolder, indexEntry.FileName);
		if (!File.Exists(objectFilePath))
		{
			return null;
		}

		var datBytes = await File.ReadAllBytesAsync(objectFilePath, ct);
		var result = SawyerStreamReader.LoadFullObject(datBytes, _logger);
		if (result.LocoObject?.ImageTable == null)
		{
			return null;
		}

		var palette = PaletteMapLoader.LoadDefault();

		var elements = result.LocoObject.ImageTable.GraphicsElements;
		var tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".zip");
		using var zipStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
		using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			for (var i = 0; i < elements.Count; i++)
			{
				var element = elements[i];

				var entry = archive.CreateEntry(i + ".png", CompressionLevel.Optimal);
				await using var entryStream = entry.Open();
				await element.ToRgba(palette).SaveAsPngAsync(entryStream, ct);
			}
		}
		zipStream.Position = 0;
		using var ms = new MemoryStream();
		await zipStream.CopyToAsync(ms, ct);
		return ms.ToArray();
	}

	public async Task<byte[]?> GetImagePngAsync(UniqueObjectId id, int imageId, CancellationToken ct)
	{
		var obj = await _db.Objects.AsNoTracking().Include(x => x.DatObjects).SingleOrDefaultAsync(x => x.Id == id, ct);
		if (obj == null)
		{
			return null;
		}

		var datEntry = obj.DatObjects.FirstOrDefault();
		if (datEntry == null || !_sfm.ObjectIndex.TryFind((datEntry.DatName, datEntry.DatChecksum), out var indexEntry) || indexEntry == null || string.IsNullOrEmpty(indexEntry.FileName))
		{
			return null;
		}

		var objectFilePath = Path.Combine(_sfm.ObjectsFolder, indexEntry.FileName);
		if (!File.Exists(objectFilePath))
		{
			return null;
		}

		var datBytes = await File.ReadAllBytesAsync(objectFilePath, ct);
		var result = SawyerStreamReader.LoadFullObject(datBytes, _logger);
		if (result.LocoObject?.ImageTable == null)
		{
			return null;
		}

		var palette = PaletteMapLoader.LoadDefault();

		var elements = result.LocoObject.ImageTable.GraphicsElements;
		if (imageId < 0 || imageId >= elements.Count)
		{
			return null;
		}

		var element = elements[imageId];
		if (element == null)
		{
			return null;
		}

		// Trim the image to its non-transparent bounding box in its native representation,
		// decoding to RGBA only if it is palette-indexed (so no full-image round-trip is needed).
		using var image = element.TrimmedToRgba(palette);
		using var ms = new MemoryStream();
		await image.SaveAsPngAsync(ms, ct);
		return ms.ToArray();
	}

	public async Task<string?> GetFilePathAsync(UniqueObjectId id, CancellationToken ct)
	{
		var obj = await _db.Objects.Include(x => x.DatObjects).Where(x => x.Id == id).SingleOrDefaultAsync(ct);
		if (obj == null)
		{
			return null;
		}

		var dat = obj.DatObjects.First();
		if (!_sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry) || entry == null)
		{
			return null;
		}

		if (string.IsNullOrEmpty(entry.FileName))
		{
			return null;
		}

		return Path.Combine(_sfm.ObjectsFolder, entry.FileName);
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

		if (hdrs.S5.IsVanilla())
		{
			return new UploadResult(false, null, "Uploading vanilla objects is not allowed", 400);
		}

		if (!hdrs.S5.IsValid() || !hdrs.Obj.IsValid())
		{
			return new UploadResult(false, null, "Invalid DAT file", 400);
		}

		var objName = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}";
		var existing = await _db.Objects.FirstOrDefaultAsync(x => x.Name == objName, ct);
		if (existing != null)
		{
			return new UploadResult(false, null, $"Object already exists. UploadedDate={existing.UploadedDate}", 202);
		}

		var missingEntry = await _db.ObjectsMissing.FirstOrDefaultAsync(x => x.DatName == hdrs.S5.Name && x.DatChecksum == hdrs.S5.Checksum, ct);
		if (missingEntry != null)
		{
			_ = _db.ObjectsMissing.Remove(missingEntry);
			_ = await _db.SaveChangesAsync(ct);
		}

		if (_db.DoesObjectExist(hdrs.S5.Name, hdrs.S5.Checksum, out var existingObj))
		{
			return new UploadResult(false, null, $"DatObject already exists. UploadedDate={existingObj!.UploadedDate}", 202);
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
			SubObjectId = 0,
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

		var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		tblObject.DatObjects.Add(new TblDatObject() { ObjectId = tblObject.Id, DatName = DatFileInfo.S5Header.Name, DatChecksum = DatFileInfo.S5Header.Checksum, xxHash3 = xxHash3, Object = tblObject, });

		_ = await DbSubObjectHelper.AddOrUpdate(_db, tblObject, LocoObject.Object);
		_ = await _db.SaveChangesAsync(ct);

		_sfm.ObjectIndex.AddEntry(new ObjectIndexEntry(hdrs.S5.Name, relativeFileName, tblObject.Id, hdrs.S5.Checksum, xxHash3, tblObject.ObjectType, tblObject.ObjectSource, tblObject.CreatedDate, tblObject.UploadedDate, tblObject.VehicleType));
		_ = _sfm.ObjectIndex.SaveIndexAsync(_sfm.IndexFile);

		var subObject = DbSubObjectHelper.GetDbSubForType(_db, tblObject.ObjectType, tblObject.Id);
		var response = new ExpandedTbl<TblObject, TblObjectPack>(tblObject, [], [], []).ToDtoDescriptor(subObject);
		return new UploadResult(true, response, null, 201);
	}
}
