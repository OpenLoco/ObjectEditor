using Dat.Converters;
using Dat.FileParsing;
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
using System.IO.Hashing;

namespace ObjectService.Services;

public record UploadResult(bool Success, DtoObjectPostResponse? Descriptor, string? ErrorMessage, int StatusCode);

public interface IObjectUploadService
{
	Task<UploadResult> UploadDatAsync(DtoObjectPost request, CancellationToken ct);
}

public class ObjectUploadService : IObjectUploadService
{
	private readonly LocoDbContext _db;
	private readonly ServerFolderManager _sfm;
	private readonly ILoggerFactory _loggerFactory;
	private readonly ILogger<ObjectUploadService> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly UserManager<TblUser> _userManager;

	public ObjectUploadService(
		LocoDbContext db,
		ServerFolderManager sfm,
		ILoggerFactory loggerFactory,
		ILogger<ObjectUploadService> logger,
		IHttpContextAccessor httpContextAccessor,
		UserManager<TblUser> userManager)
	{
		_db = db;
		_sfm = sfm;
		_loggerFactory = loggerFactory;
		_logger = logger;
		_httpContextAccessor = httpContextAccessor;
		_userManager = userManager;
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
		var saveFileName = Path.Combine(_sfm.ObjectsCustomFolder, $"{uuid}.dat");
		await File.WriteAllBytesAsync(saveFileName, datFileBytes, ct);

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

		_sfm.ObjectIndex.AddEntry(new ObjectIndexEntry(hdrs.S5.Name, saveFileName, tblObject.Id, hdrs.S5.Checksum, xxHash3, tblObject.ObjectType, tblObject.ObjectSource, tblObject.CreatedDate, tblObject.UploadedDate, tblObject.VehicleType));
		_ = _sfm.ObjectIndex.SaveIndexAsync(_sfm.IndexFile);

		var response = new ExpandedTbl<TblObject, TblObjectPack>(tblObject, [], [], []).ToDtoDescriptor();
		return new UploadResult(true, response, null, 201);
	}
}