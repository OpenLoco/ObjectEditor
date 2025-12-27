using Common;
using Common.Logging;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.IO.Compression;
using static ObjectService.RouteHandlers.TableHandlers.V1DtoExtensions;

namespace ObjectService.RouteHandlers.TableHandlers;

public static class V1DtoExtensions
{
	public record V1DtoObjectDescriptor(
		UniqueObjectId Id,
		string DatName,
		uint DatChecksum,
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		string InternalName,
		string? Description,
		DateOnly? CreationDate,
		DateOnly? LastEditDate,
		DateOnly UploadDate);

	public static V1DtoObjectDescriptor ToDtoEntryLegacy(this TblObject table)
		=> new(
			table.Id,
			table.DatObjects.FirstOrDefault()?.DatName ?? "<--->",
			table.DatObjects.FirstOrDefault()?.DatChecksum ?? 0,
			table.ObjectSource,
			table.ObjectType,
			table.VehicleType,
			table.Name,
			table.Description,
			table.CreatedDate,
			table.ModifiedDate,
			table.UploadedDate);

	public record V1DtoObjectDescriptorWithMetadata(
		UniqueObjectId Id,
		string UniqueName,
		string DatName,
		uint DatChecksum,
		string? DatBytes, // base64-encoded
		ObjectSource ObjectSource,
		ObjectType ObjectType,
		VehicleType? VehicleType,
		string? Description,
		ICollection<TblAuthor> Authors,
		DateOnly? CreationDate,
		DateOnly? LastEditDate,
		DateOnly UploadDate,
		ICollection<TblTag> Tags,
		ICollection<TblObjectPack> ObjectPacks,
		TblLicence? Licence);
}

public class LegacyRouteHandler()
{
	public static void MapRoutes(IEndpointRouteBuilder parentRoute)
	{
		// GET
		_ = parentRoute.MapGet(RoutesV1.ListObjects, ListObjects);
		_ = parentRoute.MapGet(RoutesV1.GetDat, GetDat);
		_ = parentRoute.MapGet(RoutesV1.GetDatFile, GetDatFile);
		_ = parentRoute.MapGet(RoutesV1.GetObject, GetObject);
		_ = parentRoute.MapGet(RoutesV1.GetObjectFile, GetObjectFile);
		_ = parentRoute.MapGet(RoutesV1.GetObjectImages, GetObjectImages);
		_ = parentRoute.MapGet(RoutesV1.ListObjectPacks, ListObjectPacks);
		_ = parentRoute.MapGet(RoutesV1.GetObjectPack, GetObjectPack);
		_ = parentRoute.MapGet(RoutesV1.ListScenarios, ListScenarios);
		_ = parentRoute.MapGet(RoutesV1.GetScenario, GetScenario);
		_ = parentRoute.MapGet(RoutesV1.ListSC5FilePacks, ListSC5FilePacks);
		_ = parentRoute.MapGet(RoutesV1.GetSC5FilePack, GetSC5FilePack);
		_ = parentRoute.MapGet(RoutesV1.ListAuthors, ListAuthors);
		_ = parentRoute.MapGet(RoutesV1.ListLicences, ListLicences);
		_ = parentRoute.MapGet(RoutesV1.ListTags, ListTags);

		// POST
		_ = parentRoute.MapPost(RoutesV1.UploadDat, UploadDat);
		_ = parentRoute.MapPost(RoutesV1.UploadObject, UploadObject);

		// PATCH
		_ = parentRoute.MapPatch(RoutesV1.UpdateDat, UpdateDat);
		_ = parentRoute.MapPatch(RoutesV1.UpdateObject, UpdateObject);
	}

	#region GET

	// eg: https://localhost:7230/v1/objects/list
	public static async Task<IResult> ListObjects(
		[FromQuery] string? datName,
		[FromQuery] uint? datChecksum,
		[FromQuery] string? description,
		[FromQuery] ObjectType? objectType,
		[FromQuery] VehicleType? vehicleType,
		[FromQuery] string? authorName,
		[FromQuery] string? tagName,
		[FromQuery] ObjectSource? objectSource,
		LocoDbContext db,
		[FromServices] ILogger<LegacyRouteHandler> logger)
	{
		logger.LogInformation("[ListObjects]");

		var query = db.Objects
			.Include(x => x.DatObjects)
			.AsQueryable();

		#region Query Construction

		if (!string.IsNullOrEmpty(datName))
		{
			query = query.Where(x => x.DatObjects.Any(y => y.DatName.Contains(datName)));
		}

		if (datChecksum is not null and not 0)
		{
			query = query.Where(x => x.DatObjects.Any(y => y.DatChecksum == datChecksum));
		}

		if (!string.IsNullOrEmpty(description))
		{
			query = query.Where(x => x.Description != null && x.Description.Contains(description));
		}

		if (objectType != null)
		{
			query = query.Where(x => x.ObjectType == objectType);
		}

		if (objectType == ObjectType.Vehicle && vehicleType != null)
		{
			// can only query vehicle type if it's a vehicle. if ObjectType is unspecified, that is fine
			if (objectType is not null and not ObjectType.Vehicle)
			{
				return Results.BadRequest("Cannot query for a Vehicle type on non-Vehicle objects");
			}

			query = query.Where(x => x.VehicleType == vehicleType);
		}

		if (!string.IsNullOrEmpty(authorName))
		{
			query = query.Where(x => x.Authors.Select(a => a.Name).Contains(authorName));
		}

		if (!string.IsNullOrEmpty(tagName))
		{
			query = query.Where(x => x.Tags.Select(t => t.Name).Contains(tagName));
		}

		if (objectSource != null)
		{
			query = query.Where(x => x.ObjectSource == objectSource);
		}

		#endregion

		try
		{
			var result = await query
				.Select(x => x.ToDtoEntryLegacy())
				.ToListAsync();

			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.Problem(ex.Message);
		}
	}

	// eg: https://localhost:7230/v1/objects/getdat?objectName=114&checksum=123$returnObjBytes=false
	public static async Task<IResult> GetDat([FromQuery] string datName, [FromQuery] uint datChecksum, [FromQuery] bool? returnObjBytes, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger, [FromServices] IServiceProvider sp)
	{
		logger.LogInformation("[GetDat] Object [({ObjectName}, {Checksum})] requested", datName, datChecksum);

		var eObj = await db.Objects
			.Include(x => x.DatObjects)
			.Include(x => x.Licence)
			.Where(x => x.DatObjects.First().DatName == datName && x.DatObjects.First().DatChecksum == datChecksum && x.Availability == ObjectAvailability.Available)
			.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks))
			.SingleOrDefaultAsync();

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		return await ReturnObject(returnObjBytes, logger, eObj, sfm);
	}

	// eg: http://localhost:7229/v1/objects/getobjectimages?uniqueObjectId=1
	public static async Task<IResult> GetObjectImages(UniqueObjectId uniqueObjectId, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger, [FromServices] IServiceProvider sp)
	{
		logger.LogInformation("[GetObjectImages] Object [{ObjectId}] requested with images", uniqueObjectId);

		var obj = await db.Objects
			.Include(x => x.DatObjects)
			.Where(x => x.Id == uniqueObjectId && x.Availability == ObjectAvailability.Available)
			.SingleOrDefaultAsync();

		if (obj == null)
		{
			return Results.NotFound();
		}

		var sfm = sp.GetRequiredService<ServerFolderManager>();

		var dat = obj.DatObjects.First();
		if (!sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var index))
		{
			return Results.NotFound();
		}

		var pathOnDisk = Path.Combine(sfm.ObjectsFolder, index!.FileName); // handle windows paths by replacing path separator
		logger.LogInformation("Loading file from {PathOnDisk}", pathOnDisk);

		var fileExists = File.Exists(pathOnDisk);
		if (!fileExists)
		{
			logger.LogWarning("Indexed object had {PathOnDisk} but the file wasn't found there; suggest re-indexing the server object folder.", pathOnDisk);
			return Results.NotFound();
		}

		if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			logger.LogWarning("Indexed object is a vanilla object.");
			return Results.Forbid();
		}

		var dummyLogger = new Logger(); // todo: make both libraries and server use a single logging interface
		var locoObj = SawyerStreamReader.LoadFullObject(pathOnDisk, dummyLogger, true);
		var pm = sp.GetRequiredService<PaletteMap>();

		await using var memoryStream = new MemoryStream();
		using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
		{
			var count = 0;
			foreach (var g1 in locoObj!.LocoObject!.ImageTable.GraphicsElements)
			{
				if (!pm.TryConvertG1ToRgba32Bitmap(g1, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image))
				{
					continue;
				}

				await using (var pngStream = new MemoryStream())
				{
					await image.SaveAsPngAsync(pngStream);
					pngStream.Position = 0;

					var zipEntry = zipArchive.CreateEntry(count++ + ".png", CompressionLevel.Optimal);
					await using (var zipEntryStream = zipEntry.Open())
					{
						await pngStream.CopyToAsync(zipEntryStream);
					}
				}
			}
		}

		memoryStream.Position = 0; // Reset stream position for reading
		var bytes = memoryStream.ToArray();
		return Results.File(bytes, "application/zip", "images.zip");
	}

	// eg: https://localhost:7230/v1/objects/getobject?uniqueObjectId=246263256&returnObjBytes=false
	public static async Task<IResult> GetObject([FromQuery] int uniqueObjectId, [FromQuery] bool? returnObjBytes, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger, [FromServices] IServiceProvider sp)
	{
		logger.LogInformation("[GetObject] Object [{ObjectId}] requested", uniqueObjectId);

		var eObj = await db.Objects
			.Where(x => (int)x.Id == uniqueObjectId && x.Availability == ObjectAvailability.Available)
			.Include(x => x.Licence)
			.Include(x => x.DatObjects)
			.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks))
			.SingleOrDefaultAsync();

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		return await ReturnObject(returnObjBytes, logger, eObj, sfm);
	}

	static async Task<IResult> ReturnObject(bool? returnObjBytes, ILogger<LegacyRouteHandler> logger, ExpandedTbl<TblObject, TblObjectPack>? eObj, ServerFolderManager sfm)
	{
		if (eObj == null || eObj.Object == null)
		{
			return Results.NotFound();
		}

		var dat = eObj.Object.DatObjects.First();
		if (!sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var index))
		{
			return Results.NotFound();
		}

		var obj = eObj!.Object;

		var pathOnDisk = Path.Combine(sfm.ObjectsFolder, index!.FileName); // handle windows paths by replacing path separator
		logger.LogInformation("Loading file from {PathOnDisk}", pathOnDisk);

		var fileExists = File.Exists(pathOnDisk);
		if (!fileExists)
		{
			logger.LogWarning("Indexed object had {PathOnDisk} but the file wasn't found there; suggest re-indexing the server object folder.", pathOnDisk);
		}

		var bytes = (returnObjBytes ?? false) && obj.ObjectSource is ObjectSource.Custom or ObjectSource.OpenLoco && fileExists
			? Convert.ToBase64String(await File.ReadAllBytesAsync(pathOnDisk))
			: null;

		var result = new V1DtoObjectDescriptorWithMetadata(
			obj.Id,
			obj.Name,
			obj.DatObjects.FirstOrDefault()?.DatName ?? "<--->",
			obj.DatObjects.FirstOrDefault()?.DatChecksum ?? 0,
			bytes,
			obj.ObjectSource,
			obj.ObjectType,
			obj.VehicleType,
			obj.Description,
			eObj.Authors,
			obj.CreatedDate,
			obj.ModifiedDate,
			obj.UploadedDate,
			eObj.Tags,
			eObj.Packs,
			obj.Licence);

		return Results.Ok(result);
	}

	// eg: https://localhost:7230/v1/objects/originaldatfile?objectName=114&checksum=123
	public static async Task<IResult> GetDatFile([FromQuery] string datName, [FromQuery] uint datChecksum, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger, [FromServices] IServiceProvider sp)
	{
		logger.LogInformation("[GetDatFile] DatName={DatName} DatChecksum={DatChecksum}", datName, datChecksum);

		var obj = await db.DatObjects
			.Include(x => x.Object)
			.Where(x => x.DatName == datName && x.DatChecksum == datChecksum && x.Object.Availability == ObjectAvailability.Available)
			.SingleOrDefaultAsync();

		if (obj == null)
		{
			return Results.NotFound();
		}

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		return ReturnFile(obj.Object, (obj.DatName, obj.DatChecksum), obj.xxHash3, sfm);
	}

	// eg: https://localhost:7230/v1/objects/getobjectfile?objectName=114&checksum=123
	public static async Task<IResult> GetObjectFile([FromQuery] int uniqueObjectId, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger, [FromServices] IServiceProvider sp)
	{
		logger.LogInformation("[GetObjectFile] Object [{ObjectId}] requested", uniqueObjectId);

		var obj = await db.DatObjects
			.Include(x => x.Object)
			.Where(x => (int)x.Object.Id == uniqueObjectId && x.Object.Availability == ObjectAvailability.Available)
			.FirstOrDefaultAsync(); // may be more than one dat file associated with this object, so just get the first for now

		if (obj == null)
		{
			return Results.NotFound();
		}

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		return ReturnFile(obj.Object, (obj.DatName, obj.DatChecksum), obj.xxHash3, sfm);
	}

	static IResult ReturnFile(TblObject? obj, (string objectName, uint checksum)? datDetails, ulong? xxHash3, ServerFolderManager sfm)
	{
		if (obj == null)
		{
			return Results.NotFound();
		}

		if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			return Results.Forbid();
		}

		ObjectIndexEntry? entry = null;
		if (datDetails != null && !sfm.ObjectIndex.TryFind(datDetails.Value, out entry))
		{
			return Results.NotFound();
		}
		else if (xxHash3 != null && !sfm.ObjectIndex.TryFind(xxHash3.Value, out entry))
		{
			return Results.NotFound();
		}

		const string contentType = "application/octet-stream";

		var path = Path.Combine(sfm.ObjectsFolder, entry!.FileName);
		return obj != null && File.Exists(path)
			? Results.File(path, contentType, Path.GetFileName(path))
			: Results.NotFound();
	}

	// eg: https://localhost:7230/v1/scenarios/list
	public static async Task<IResult> ListScenarios([FromServices] IServiceProvider sp)
		=> await Task.Run(() =>
		{
			var sfm = sp.GetRequiredService<ServerFolderManager>();
			var files = Directory.GetFiles(sfm.ScenariosFolder, "*.SC5", SearchOption.AllDirectories);
			var count = 0UL;
			var filenames = files.Select(x => new DtoScenarioEntry(count++, Path.GetRelativePath(sfm.ScenariosFolder, x)));
			return Results.Ok(filenames.ToList());
		});

	// eg: https://localhost:7230/v1/scenarios/getscenario?uniqueScenarioId=246263256&returnObjBytes=false
	public static async Task<IResult> GetScenario([FromQuery] int uniqueScenarioId, [FromQuery] bool? returnObjBytes, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger)
	{
		logger.LogInformation("[GetScenario] Scenario [{ScenarioId}] requested", uniqueScenarioId);
		return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}

	#endregion

	// eg: https://localhost:7230/v1/authors/list
	public static async Task<IResult> ListAuthors(LocoDbContext db)
		=> Results.Ok(await db.Authors
			.Select(x => new DtoAuthorEntry(x.Id, x.Name))
			.ToListAsync());

	// eg: https://localhost:7230/v1/licences/list
	public static async Task<IResult> ListLicences(LocoDbContext db)
		=> Results.Ok(await db.Licences
			.Select(x => new DtoLicenceEntry(x.Id, x.Name, x.Text))
			.ToListAsync());

	// eg: https://localhost:7230/v1/tags/list
	public static async Task<IResult> ListTags(LocoDbContext db)
		=> Results.Ok(await db.Tags
			.Select(x => new DtoTagEntry(x.Id, x.Name))
			.ToListAsync());

	// eg: https://localhost:7230/v1/objectpacks/list
	public static async Task<IResult> ListObjectPacks(LocoDbContext db)
		=> Results.Ok(
			(await db.ObjectPacks
				.Include(l => l.Licence)
				.ToListAsync())
			.Select(x => x.ToDtoEntry())
			.OrderBy(x => x.Name));

	// eg: https://localhost:7230/v1/objectpacks/getpack?uniqueId=123
	public static async Task<IResult> GetObjectPack([FromQuery] int uniqueId, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.ObjectPacks
				.Where(x => (int)x.Id == uniqueId)
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblObjectPack, TblObject>(x, x.Objects, x.Authors, x.Tags))
				.ToListAsync())
			.Select(x => x.ToDtoDescriptor())
			.OrderBy(x => x.Name));

	// eg: https://localhost:7230/v1/sc5filepacks/list
	public static async Task<IResult> ListSC5FilePacks(LocoDbContext db)
		=> Results.Ok(
			(await db.SC5FilePacks
				.Include(l => l.Licence)
				.ToListAsync())
			.Select(x => x.ToDtoEntry())
			.OrderBy(x => x.Name));

	// eg: https://localhost:7230/v1/sc5filepacks/getpack?uniqueId=123
	public static async Task<IResult> GetSC5FilePack([FromQuery] int uniqueId, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.SC5FilePacks
				.Where(x => (int)x.Id == uniqueId)
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblSC5FilePack, TblSC5File>(x, x.SC5Files, x.Authors, x.Tags))
				.ToListAsync())
			.Select(x => x.ToDtoDescriptor())
			.OrderBy(x => x.Name));

	#region POST

	// eg: https://localhost:7230/v1/uploaddat/...
	public static async Task<IResult> UploadDat(DtoUploadDat request, [FromServices] LocoDbContext db, [FromServices] ILogger<LegacyRouteHandler> logger, [FromServices] IServiceProvider sp)
	{
		logger.LogInformation("[UploadDat] Upload requested");

		if (string.IsNullOrEmpty(request.DatBytesAsBase64))
		{
			return Results.BadRequest($"{nameof(request.DatBytesAsBase64)} cannot be null - it must contain the valid bytes of a loco dat object.");
		}

		byte[]? datFileBytes;
		try
		{
			datFileBytes = Convert.FromBase64String(request.DatBytesAsBase64);
		}
		catch (FormatException ex)
		{
			return Results.BadRequest(ex.Message);
		}

		if (datFileBytes == null || datFileBytes.Length == 0)
		{
			return Results.BadRequest($"Unable to decode {nameof(request.DatBytesAsBase64)} - it must contain the valid bytes of a loco dat object.");
		}

		if (datFileBytes.Length > ServerLimits.MaximumUploadFileSize)
		{
			return Results.BadRequest("Unable to accept file sizes > 5MB");
		}

		var ssrLogger = new Logger();
		if (!SawyerStreamReader.TryGetHeadersFromBytes(datFileBytes, out var hdrs, ssrLogger))
		{
			return Results.BadRequest("Provided data had invalid dat file headers");
		}

		if (hdrs.S5.IsVanilla())
		{
			return Results.BadRequest("Nice try genius. Uploading vanilla objects is not allowed.");
		}

		if (!hdrs.S5.IsValid() || !hdrs.Obj.IsValid())
		{
			return Results.BadRequest("Invalid DAT file.");
		}

		if (db.DoesObjectExist(hdrs.S5.Name, hdrs.S5.Checksum, out var existingObject))
		{
			return Results.Accepted($"Object already exists in the database. DatName={hdrs.S5.Name} DatChecksum={hdrs.S5.Checksum} UploadedDate={existingObject!.UploadedDate}");
		}

		// at this stage, headers must be valid. we can add it to the object index/database, even if the remainder of the object is invalid

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var locoLogger = new Logger();

		var (_, LocoObject) = SawyerStreamReader.LoadFullObject(datFileBytes, locoLogger);
		var uuid = Guid.NewGuid();
		var saveFileName = Path.Combine(sfm.ObjectsCustomFolder, $"{uuid}.dat");
		File.WriteAllBytes(saveFileName, datFileBytes);

		logger.LogInformation("File accepted DatName={DatName} DatChecksum={DatChecksum} PathOnDisk={SaveFileName}", hdrs.S5.Name, hdrs.S5.Checksum, saveFileName);

		var createdDate = request.CreatedDate;
		var modifiedDate = request.ModifiedDate;

		VehicleType? vehicleType = null;
		if (LocoObject?.Object is VehicleObject veh)
		{
			vehicleType = veh.Type;
		}

		var locoTbl = new TblObject()
		{
			Name = uuid.ToString(),
			Description = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}",
			ObjectSource = ObjectSource.Custom, // not possible to upload vanilla objects
			ObjectType = hdrs.S5.ObjectType.Convert(),
			VehicleType = vehicleType,
			Availability = request.InitialAvailability,
			CreatedDate = createdDate,
			ModifiedDate = modifiedDate,
			UploadedDate = DateOnly.UtcToday,
			Authors = [],
			Tags = [],
			ObjectPacks = [],
			Licence = null,
		};
		var addedObj = db.Objects.Add(locoTbl);

		var locoLookupTbl = new TblDatObject()
		{
			DatName = hdrs.S5.Name,
			DatChecksum = hdrs.S5.Checksum,
			xxHash3 = request.xxHash3,
			ObjectId = addedObj.Entity.Id,
			Object = locoTbl,
		};
		locoTbl.DatObjects.Add(locoLookupTbl);

		_ = db.DatObjects.Add(locoLookupTbl);
		_ = await db.SaveChangesAsync();

		sfm.ObjectIndex.Objects.Add(
			new ObjectIndexEntry(hdrs.S5.Name, saveFileName, locoTbl.Id, hdrs.S5.Checksum, request.xxHash3, locoTbl.ObjectType, locoTbl.ObjectSource, createdDate, modifiedDate, locoTbl.VehicleType));

		return Results.Created($"Successfully added {locoTbl.Name} with unique id {locoTbl.Id}", locoTbl.Id);
	}

	public static async Task<IResult> UploadObject([FromServices] ILogger<LegacyRouteHandler> logger)
	{
		logger.LogWarning("[UploadDat] - not implemented");
		return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}

	#endregion

	#region PATCH

	public static async Task<IResult> UpdateDat([FromServices] ILogger<LegacyRouteHandler> logger)
	{
		logger.LogWarning("[UploadDat] - not implemented");
		return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}

	public static async Task<IResult> UpdateObject([FromServices] ILogger<LegacyRouteHandler> logger)
	{
		logger.LogWarning("[UploadDat] - not implemented");
		return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}

	#endregion
}
