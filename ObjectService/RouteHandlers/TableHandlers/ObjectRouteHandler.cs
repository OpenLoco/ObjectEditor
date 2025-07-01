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
using System.IO.Hashing;

namespace ObjectService.RouteHandlers.TableHandlers
{
	public class ObjectRouteHandler : ITableRouteHandler
	{
		public static string BaseRoute => RoutesV2.Objects;
		public static Delegate ListDelegate => ListAsync;
		public static Delegate CreateDelegate => CreateAsync;
		public static Delegate ReadDelegate => ReadAsync;
		public static Delegate UpdateDelegate => UpdateAsync;
		public static Delegate DeleteDelegate => DeleteAsync;

		public static void MapRoutes(IEndpointRouteBuilder endpoints)
			=> BaseTableRouteHandler.MapRoutes<ObjectRouteHandler>(endpoints);

		public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
		{
			var resourceRoute = parentRoute.MapGroup(RoutesV2.ResourceRoute);
			_ = resourceRoute.MapGet(RoutesV2.File, GetObjectFile);
			_ = resourceRoute.MapGet(RoutesV2.Images, GetObjectImages);
		}

		static async Task<IResult> CreateAsync(DtoUploadDat request, LocoDbContext db, [FromServices] IServiceProvider sp, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[CreateAsync] Upload requested");

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

			var sfm = sp.GetRequiredService<ServerFolderManager>();

			var (_, LocoObject) = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes, ssrLogger);
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(sfm.ObjectsCustomFolder, $"{uuid}.dat");
			File.WriteAllBytes(saveFileName, datFileBytes);

			logger.LogInformation("File accepted DatName={DatName} DatChecksum={DatChecksum} PathOnDisk={SaveFileName}", hdrs.S5.Name, hdrs.S5.Checksum, saveFileName);

			var creationTime = request.CreatedDate;

			VehicleType? vehicleType = null;
			if (LocoObject?.Object is VehicleObject veh)
			{
				vehicleType = veh.Type;
			}

			var locoTbl = new TblObject()
			{
				Name = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}", // same as DB seeder name. this is NOT unique
				Description = string.Empty,
				ObjectSource = ObjectSource.Custom, // not possible to upload vanilla objects
				ObjectType = hdrs.S5.ObjectType,
				VehicleType = vehicleType,
				Availability = request.InitialAvailability,
				CreatedDate = creationTime,
				ModifiedDate = null,
				UploadedDate = DateTimeOffset.UtcNow,
				Authors = [],
				Tags = [],
				ObjectPacks = [],
				DatObjects = [],
				Licence = null,
			};

			var xxHash3 = XxHash3.HashToUInt64(datFileBytes);

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			sfm.ObjectIndex.Objects.Add(
				new ObjectIndexEntry(hdrs.S5.Name, saveFileName, locoTbl.Id, hdrs.S5.Checksum, xxHash3, locoTbl.ObjectType, locoTbl.ObjectSource, locoTbl.CreatedDate, locoTbl.UploadedDate, locoTbl.VehicleType));

			return Results.Created($"Successfully added {locoTbl.Name} with unique id {locoTbl.Id}", locoTbl.Id);
		}

		static async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, LocoDbContext db, [FromServices] IServiceProvider sp, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[ReadAsync] Read requested for object {ObjectId}", id);

			var eObj = await db.Objects
				.Where(x => x.Id == id)
				.Include(x => x.Licence)
				.Include(x => x.DatObjects)
				.Include(x => x.StringTable)
				.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks))
				.SingleOrDefaultAsync();

			var sfm = sp.GetRequiredService<ServerFolderManager>();
			return ReturnObject(eObj, sfm, logger);
		}

		static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, DtoObjectDescriptor request, LocoDbContext db, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[UpdateAsync] Update requested for object {ObjectId}", id);
			return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
		}

		static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, LocoDbContext db, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[DeleteAsync] Delete requested for object {ObjectId}", id);
			// for now we could soft-delete by marking an object as Unavailable?
			return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
		}

		static async Task<IResult> ListAsync(HttpContext context, LocoDbContext db, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[ListAsync] List requested for object");

			if (context.Request.Query.Count == 0)
			{
				return Results.Ok(
					await db.Objects
						.Include(x => x.DatObjects)
						.Select(x => x.ToDtoEntry())
						.ToListAsync());
			}
			else
			{
				logger.LogInformation("[ListAsync] Request had {ParamCount} query params {Params}", context.Request.Query.Count, context.Request.Query.ToString());

				var query = db.Objects.AsQueryable();
				var filters = context.Request.Query;

				if (!filters.TryGetValue("objectType", out var objectTypeStr) || !Enum.TryParse(objectTypeStr, out ObjectType objectType))
				{
					return Results.BadRequest();
				}

				query = query.Where(x => x.ObjectType == objectType);

				var locoPropertiesForObject = ObjectTypeMapping
					.TypeToStruct(objectType)
					.GetProperties();

				var metadataPropertiesForObject = typeof(ObjectMetadata).GetProperties();

				var allProperties = locoPropertiesForObject.Union(metadataPropertiesForObject);

				foreach (var filter in filters.Where(x => x.Key != "objectType" && !string.IsNullOrEmpty(x.Key) && !string.IsNullOrEmpty(x.Value)))
				{
					var key = filter.Key;
					var value = filter.Value.FirstOrDefault();

					if (!allProperties.Any(x => x.Name == key))
					{
						continue; // Skip disallowed keys
					}

					//query = query.Where("{key} == @0", value);

					//switch (key)
					//{
					//	case "name":
					//		query = query.Where("Name.Contains(@0)", value);
					//		break;
					//	case "age":
					//		if (int.TryParse(value, out var age))
					//		{
					//			query = query.Where("Age == @0", age);
					//		}
					//		break;
					//	case "category":
					//		query = query.Where("Category == @0", value);
					//		break;
					//}
				}

				var results = await query.ToListAsync();
				return Results.Ok(results);

				// transform query into linq/db query

				// s5 header query params
				// object-specific query params
				// metadata query params

				//return Results.Problem(statusCode: StatusCodes.Status501NotImplemented);
			}
		}

		// eg: http://localhost:7229/v1/objects/{id}/images
		static async Task<IResult> GetObjectImages([FromRoute] UniqueObjectId id, LocoDbContext db, [FromServices] IServiceProvider sp, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[GetObjectImages] Get requested for object {ObjectId}", id);

			// currently we MUST have a DAT backing object
			var obj = await db.Objects
				.Include(x => x.DatObjects)
				.Where(x => x.Id == id)
				.SingleOrDefaultAsync();

			if (obj == null)
			{
				return Results.NotFound();
			}

			if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
			{
				logger.LogWarning("Indexed object is a vanilla object");
				return Results.Forbid();
			}

			if (obj.Availability == Definitions.ObjectAvailability.Unavailable)
			{
				logger.LogWarning("Object [Id={Id} Name={Name}] is marked as Unavailable and cannot be downloaded", obj.Id, obj.Name);
				return Results.Forbid();
			}

			var sfm = sp.GetRequiredService<ServerFolderManager>();
			var pm = sp.GetRequiredService<PaletteMap>();

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
				logger.LogWarning("Indexed object had {PathOnDisk} but the file wasn't found there; suggest re-indexing the server object folder", pathOnDisk);
				return Results.NotFound();
			}

			var dummyLogger = new Logger(); // todo: make both libraries and server use a single logging interface

			var locoObj = SawyerStreamReader.LoadFullObjectFromFile(pathOnDisk, dummyLogger, true);

			await using var memoryStream = new MemoryStream();
			using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
			{
				var count = 0;
				foreach (var g1 in locoObj!.Value!.LocoObject!.G1Elements)
				{
					if (!pm.TryConvertG1ToRgba32Bitmap(g1, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
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

		// eg: https://localhost:7230/objects/114
		static async Task<IResult> GetObjectFile([FromRoute] UniqueObjectId id, LocoDbContext db, [FromServices] IServiceProvider sp, [FromServices] ILogger<ObjectRouteHandler> logger)
		{
			logger.LogInformation("[GetObjectFile] Get requested for object {ObjectId}", id);

			var obj = await db.Objects
				.Include(x => x.DatObjects)
				.Where(x => x.Id == id)
				.SingleOrDefaultAsync();

			var sfm = sp.GetRequiredService<ServerFolderManager>();
			return ReturnFile(obj, sfm, logger);
		}

		static IResult ReturnObject(ExpandedTbl<TblObject, TblObjectPack>? eObj, ServerFolderManager sfm, ILogger<ObjectRouteHandler> logger)
		{
			logger.LogDebug("[ReturnObject]");

			if (eObj == null || eObj.Object == null)
			{
				return Results.NotFound();
			}

			if (eObj.Object.Availability == Definitions.ObjectAvailability.Unavailable)
			{
				logger.LogError("Object [Id={Id} Name={Name}] is marked as Unavailable and cannot be downloaded", eObj.Object.Id, eObj.Object.Name);
				return Results.Forbid();
			}

			var obj = eObj.ToDtoDescriptor();

			foreach (var dat in obj.DatObjects)
			{
				if (!sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry) || entry == null)
				{
					logger.LogWarning("Object {datFile} didn't exist in the object index", dat);
					continue;
				}

				if (entry.FileName == null)
				{
					logger.LogWarning("Object {datFile} has a null filename - suggest re-indexing the current folder", dat);
					continue;
				}

				var path = Path.Combine(sfm.ObjectsFolder, entry.FileName);

				if (!File.Exists(path))
				{
					logger.LogWarning("Object {datFile} existed in the object index but not on disk. ExpectedPath=\"{path}\"", dat, path);
					continue;
				}

				if (eObj.Object.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
				{
					logger.LogWarning("User attempted to download a vanilla object");
					dat.DatBytes = null;
				}
				else
				{
					dat.DatBytes = Convert.ToBase64String(File.ReadAllBytes(path));
				}
			}

			return Results.Ok(obj);
		}

		static IResult ReturnFile(TblObject? obj, ServerFolderManager sfm, ILogger<ObjectRouteHandler> logger)
		{
			logger.LogDebug("[ReturnFile]");

			if (obj == null)
			{
				return Results.NotFound();
			}

			if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
			{
				logger.LogDebug("User attempted to download a vanilla object");
				return Results.Forbid();
			}

			var dat = obj.DatObjects.First();
			if (!sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry) || entry == null)
			{
				logger.LogDebug("Object {datFile} didn't exist in the object index", dat);
				return Results.NotFound();
			}

			if (string.IsNullOrEmpty(entry.FileName))
			{
				logger.LogWarning("Object {datFile} has a null filename - suggest re-indexing the current folder", dat);
				return Results.NotFound();
			}

			var path = Path.Combine(sfm.ObjectsFolder, entry.FileName);
			const string contentType = "application/octet-stream";

			if (!File.Exists(path))
			{
				logger.LogWarning("Object {datFile} existed in the object index but not on disk. ExpectedPath=\"{path}\"", dat, path);
			}

			return Results.File(path, contentType, Path.GetFileName(path));
		}
	}
}
