using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.SourceData;
using OpenLoco.Definitions.Web;
using OpenLoco.ObjectService;
using SixLabors.ImageSharp;
using System.IO.Compression;
using static ObjectService.TableHandlers.LegacyDtoExtensions;

namespace ObjectService.TableHandlers
{
	public static class LegacyDtoExtensions
	{
		public record LegacyDtoObjectDescriptor(
			int Id,
			string DatName,
			uint DatChecksum,
			ObjectSource ObjectSource,
			ObjectType ObjectType,
			VehicleType? VehicleType,
			string InternalName,
			string? Description,
			DateTimeOffset? CreationDate,
			DateTimeOffset? LastEditDate,
			DateTimeOffset UploadDate);

		public static LegacyDtoObjectDescriptor ToDtoEntryLegacy(this TblObject table)
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

		public record LegacyDtoObjectDescriptorWithMetadata(
			int Id,
			string UniqueName,
			string DatName,
			uint DatChecksum,
			string? DatBytes, // base64-encoded
			ObjectSource ObjectSource,
			ObjectType ObjectType,
			VehicleType? VehicleType,
			string? Description,
			ICollection<TblAuthor> Authors,
			DateTimeOffset? CreationDate,
			DateTimeOffset? LastEditDate,
			DateTimeOffset UploadDate,
			ICollection<TblTag> Tags,
			ICollection<TblObjectPack> ObjectPacks,
			TblLicence? Licence);
	}

	public class LegacyRouteHandler(ServerFolderManager serverFolderManager, PaletteMap paletteMap)
	{
		ServerFolderManager ServerFolderManager { get; init; } = serverFolderManager;
		PaletteMap PaletteMap { get; init; } = paletteMap;

		OpenLoco.Common.Logging.ILogger Logger { get; } = new Logger();

		public void MapRoutes(IEndpointRouteBuilder parentRoute)
		{
			// GET
			_ = parentRoute.MapGet(LegacyRoutes.ListObjects, LegacyRouteHandler.ListObjects);
			_ = parentRoute.MapGet(LegacyRoutes.GetDat, GetDat);
			_ = parentRoute.MapGet(LegacyRoutes.GetDatFile, GetDatFile);
			_ = parentRoute.MapGet(LegacyRoutes.GetObject, GetObject);
			_ = parentRoute.MapGet(LegacyRoutes.GetObjectFile, GetObjectFile);
			_ = parentRoute.MapGet(LegacyRoutes.GetObjectImages, GetObjectImages);
			_ = parentRoute.MapGet(LegacyRoutes.ListObjectPacks, ListObjectPacks);
			_ = parentRoute.MapGet(LegacyRoutes.GetObjectPack, GetObjectPack);
			_ = parentRoute.MapGet(LegacyRoutes.ListScenarios, ListScenarios);
			_ = parentRoute.MapGet(LegacyRoutes.GetScenario, GetScenario);
			_ = parentRoute.MapGet(LegacyRoutes.ListSC5FilePacks, ListSC5FilePacks);
			_ = parentRoute.MapGet(LegacyRoutes.GetSC5FilePack, GetSC5FilePack);
			_ = parentRoute.MapGet(LegacyRoutes.ListAuthors, ListAuthors);
			_ = parentRoute.MapGet(LegacyRoutes.ListLicences, ListLicences);
			_ = parentRoute.MapGet(LegacyRoutes.ListTags, ListTags);

			// POST
			_ = parentRoute.MapPost(LegacyRoutes.UploadDat, UploadDat);
			_ = parentRoute.MapPost(LegacyRoutes.UploadObject, UploadObject);

			// PATCH
			_ = parentRoute.MapPatch(LegacyRoutes.UpdateDat, UpdateDat);
			_ = parentRoute.MapPatch(LegacyRoutes.UpdateObject, UpdateObject);
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
			LocoDbContext db)
		{
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
		public async Task<IResult> GetDat([FromQuery] string datName, [FromQuery] uint datChecksum, [FromQuery] bool? returnObjBytes, LocoDbContext db, [FromServices] ILogger<Server> logger)
		{
			logger.LogInformation("Object [({ObjectName}, {Checksum})] requested", datName, datChecksum);

			var eObj = await db.Objects
				.Include(x => x.DatObjects)
				.Include(x => x.Licence)
				.Where(x => x.DatObjects.First().DatName == datName && x.DatObjects.First().DatChecksum == datChecksum)
				.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks))
				.SingleOrDefaultAsync();

			return await ReturnObject(returnObjBytes, logger, eObj);
		}

		// eg: http://localhost:7229/v1/objects/getobjectimages?uniqueObjectId=1
		public async Task<IResult> GetObjectImages(int uniqueObjectId, LocoDbContext db, [FromServices] ILogger<Server> logger)
		{
			Console.WriteLine($"Object [{uniqueObjectId}] requested with images");

			var obj = await db.Objects
				.Include(x => x.DatObjects)
				.Where(x => x.Id == uniqueObjectId)
				.SingleOrDefaultAsync();

			if (obj == null)
			{
				return Results.NotFound();
			}

			var dat = obj.DatObjects.First();
			if (!ServerFolderManager.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var index))
			{
				return Results.NotFound();
			}

			var pathOnDisk = Path.Combine(ServerFolderManager.ObjectsFolder, index!.Filename); // handle windows paths by replacing path separator
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
			var locoObj = SawyerStreamReader.LoadFullObjectFromFile(pathOnDisk, dummyLogger, true);

			await using var memoryStream = new MemoryStream();
			using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
			{
				var count = 0;
				foreach (var g1 in locoObj!.Value!.LocoObject!.G1Elements)
				{
					if (!PaletteMap.TryConvertG1ToRgba32Bitmap(g1, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
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
		public async Task<IResult> GetObject([FromQuery] int uniqueObjectId, [FromQuery] bool? returnObjBytes, LocoDbContext db, [FromServices] ILogger<Server> logger)
		{
			logger.LogInformation("Object [{UniqueObjectId}] requested", uniqueObjectId);

			var eObj = await db.Objects
				.Where(x => x.Id == uniqueObjectId)
				.Include(x => x.Licence)
				.Include(x => x.DatObjects)
				.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks))
				.SingleOrDefaultAsync();

			return await ReturnObject(returnObjBytes, logger, eObj);
		}

		async Task<IResult> ReturnObject(bool? returnObjBytes, ILogger<Server> logger, ExpandedTbl<TblObject, TblObjectPack>? eObj)
		{
			if (eObj == null || eObj.Object == null)
			{
				return Results.NotFound();
			}

			var dat = eObj.Object.DatObjects.First();
			if (!ServerFolderManager.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var index))
			{
				return Results.NotFound();
			}

			var obj = eObj!.Object;

			var pathOnDisk = Path.Combine(ServerFolderManager.ObjectsFolder, index!.Filename); // handle windows paths by replacing path separator
			logger.LogInformation("Loading file from {PathOnDisk}", pathOnDisk);

			var fileExists = File.Exists(pathOnDisk);
			if (!fileExists)
			{
				logger.LogWarning("Indexed object had {PathOnDisk} but the file wasn't found there; suggest re-indexing the server object folder.", pathOnDisk);
			}

			var bytes = (returnObjBytes ?? false) && obj.ObjectSource is ObjectSource.Custom or ObjectSource.OpenLoco && fileExists
				? Convert.ToBase64String(await File.ReadAllBytesAsync(pathOnDisk))
				: null;

			var result = new LegacyDtoObjectDescriptorWithMetadata(
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
		public async Task<IResult> GetDatFile([FromQuery] string datName, [FromQuery] uint datChecksum, LocoDbContext db)
		{
			var obj = await db.DatObjects
				.Include(x => x.Object)
				.Where(x => x.DatName == datName && x.DatChecksum == datChecksum)
				.SingleOrDefaultAsync();

			if (obj == null)
			{
				return Results.NotFound();
			}

			return ReturnFile(obj.Object, (obj.DatName, obj.DatChecksum), obj.xxHash3);
		}

		// eg: https://localhost:7230/v1/objects/getobjectfile?objectName=114&checksum=123
		public async Task<IResult> GetObjectFile([FromQuery] int uniqueObjectId, LocoDbContext db)
		{
			var obj = await db.DatObjects
				.Include(x => x.Object)
				.Where(x => x.Object.Id == uniqueObjectId)
				.FirstOrDefaultAsync(); // may be more than one dat file associated with this object, so just get the first for now

			if (obj == null)
			{
				return Results.NotFound();
			}

			return ReturnFile(obj.Object, (obj.DatName, obj.DatChecksum), obj.xxHash3);
		}

		IResult ReturnFile(TblObject? obj, (string objectName, uint checksum)? datDetails, ulong? xxHash3)
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
			if (datDetails != null && !ServerFolderManager.ObjectIndex.TryFind(datDetails.Value, out entry))
			{
				return Results.NotFound();
			}
			else if (xxHash3 != null && !ServerFolderManager.ObjectIndex.TryFind(xxHash3.Value, out entry))
			{
				return Results.NotFound();
			}

			const string contentType = "application/octet-stream";

			var path = Path.Combine(ServerFolderManager.ObjectsFolder, entry!.Filename);
			return obj != null && File.Exists(path)
				? Results.File(path, contentType, Path.GetFileName(path))
				: Results.NotFound();
		}

		// eg: https://localhost:7230/v1/scenarios/list
		public async Task<IResult> ListScenarios()
			=> await Task.Run(() =>
			{
				var files = Directory.GetFiles(ServerFolderManager.ScenariosFolder, "*.SC5", SearchOption.AllDirectories);
				var count = 0;
				var filenames = files.Select(x => new DtoScenarioEntry(count++, Path.GetRelativePath(ServerFolderManager.ScenariosFolder, x)));
				return Results.Ok(filenames.ToList());
			});

		// eg: https://localhost:7230/v1/scenarios/getscenario?uniqueScenarioId=246263256&returnObjBytes=false
		public async Task<IResult> GetScenario([FromQuery] int uniqueScenarioId, [FromQuery] bool? returnObjBytes, LocoDbContext db, [FromServices] ILogger<Server> logger)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		#endregion

		// eg: https://localhost:7230/v1/authors/list
		public async Task<IResult> ListAuthors(LocoDbContext db)
			=> Results.Ok(await db.Authors
				.Select(x => new DtoAuthorEntry(x.Id, x.Name))
				.ToListAsync());

		// eg: https://localhost:7230/v1/licences/list
		public async Task<IResult> ListLicences(LocoDbContext db)
			=> Results.Ok(await db.Licences
				.Select(x => new DtoLicenceEntry(x.Id, x.Name, x.Text))
				.ToListAsync());

		// eg: https://localhost:7230/v1/tags/list
		public async Task<IResult> ListTags(LocoDbContext db)
			=> Results.Ok(await db.Tags
				.Select(x => new DtoTagEntry(x.Id, x.Name))
				.ToListAsync());

		// eg: https://localhost:7230/v1/objectpacks/list
		public async Task<IResult> ListObjectPacks(LocoDbContext db)
			=> Results.Ok(
				(await db.ObjectPacks
					.Include(l => l.Licence)
					.ToListAsync())
				.Select(x => x.ToDtoEntry())
				.OrderBy(x => x.Name));

		// eg: https://localhost:7230/v1/objectpacks/getpack?uniqueId=123
		public async Task<IResult> GetObjectPack([FromQuery] int uniqueId, LocoDbContext db)
			=> Results.Ok(
				(await db.ObjectPacks
					.Where(x => x.Id == uniqueId)
					.Include(l => l.Licence)
					.Select(x => new ExpandedTblPack<TblObjectPack, TblObject>(x, x.Objects, x.Authors, x.Tags))
					.ToListAsync())
				.Select(x => x.ToDtoDescriptor())
				.OrderBy(x => x.Name));

		// eg: https://localhost:7230/v1/sc5filepacks/list
		public async Task<IResult> ListSC5FilePacks(LocoDbContext db)
			=> Results.Ok(
				(await db.SC5FilePacks
					.Include(l => l.Licence)
					.ToListAsync())
				.Select(x => x.ToDtoEntry())
				.OrderBy(x => x.Name));

		// eg: https://localhost:7230/v1/sc5filepacks/getpack?uniqueId=123
		public async Task<IResult> GetSC5FilePack([FromQuery] int uniqueId, LocoDbContext db)
			=> Results.Ok(
				(await db.SC5FilePacks
					.Where(x => x.Id == uniqueId)
					.Include(l => l.Licence)
					.Select(x => new ExpandedTblPack<TblSC5FilePack, TblSC5File>(x, x.SC5Files, x.Authors, x.Tags))
					.ToListAsync())
				.Select(x => x.ToDtoDescriptor())
				.OrderBy(x => x.Name));

		#region POST

		// eg: https://localhost:7230/v1/uploaddat/...
		public async Task<IResult> UploadDat(DtoUploadDat request, LocoDbContext db, [FromServices] ILogger<Server> logger)
		{
			logger.LogInformation("Upload requested");

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

			if (db.DoesObjectExist(hdrs.S5, out var existingObject))
			{
				return Results.Accepted($"Object already exists in the database. DatName={hdrs.S5.Name} DatChecksum={hdrs.S5.Checksum} UploadedDate={existingObject!.UploadedDate}");
			}

			// at this stage, headers must be valid. we can add it to the object index/database, even if the remainder of the object is invalid

			var (_, LocoObject) = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes, Logger);
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(ServerFolderManager.ObjectsCustomFolder, $"{uuid}.dat");
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
				ObjectType = hdrs.S5.ObjectType,
				VehicleType = vehicleType,
				CreatedDate = createdDate,
				ModifiedDate = modifiedDate,
				UploadedDate = DateTimeOffset.UtcNow,
				Authors = [],
				Tags = [],
				ObjectPacks = [],
				Licence = null,
			};

			ServerFolderManager.ObjectIndex.Objects.Add(new ObjectIndexEntry(saveFileName, hdrs.S5.Name, hdrs.S5.Checksum, request.xxHash3, uuid.ToString(), locoTbl.ObjectType, locoTbl.ObjectSource, createdDate, modifiedDate, locoTbl.VehicleType));
			var addedObj = db.Objects.Add(locoTbl);

			var locoLookupTbl = new TblDatObject()
			{
				DatName = hdrs.S5.Name,
				DatChecksum = hdrs.S5.Checksum,
				xxHash3 = request.xxHash3,
				ObjectId = addedObj.Entity.Id,
				Object = locoTbl,
			};
			_ = db.DatObjects.Add(locoLookupTbl);

			locoTbl.DatObjects.Add(locoLookupTbl);

			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoTbl.Name} with unique id {locoTbl.Id}", locoTbl.Id);
		}

		public async Task<IResult> UploadObject([FromServices] ILogger<Server> logger)
		{
			logger.LogWarning("[UploadDat] - not implemented");
			return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
		}

		#endregion

		#region PATCH

		public async Task<IResult> UpdateDat([FromServices] ILogger<Server> logger)
		{
			logger.LogWarning("[UploadDat] - not implemented");
			return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
		}

		public async Task<IResult> UpdateObject([FromServices] ILogger<Server> logger)
		{
			logger.LogWarning("[UploadDat] - not implemented");
			return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
		}

		#endregion
	}
}
