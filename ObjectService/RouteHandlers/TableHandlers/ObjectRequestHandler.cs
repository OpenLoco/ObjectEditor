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
	public class ObjectRequestHandler(ServerFolderManager sfm, PaletteMap paletteMap) : BaseTableRequestHandler<DtoObjectDescriptor>
	{
		ServerFolderManager ServerFolderManager { get; init; } = sfm;
		PaletteMap PaletteMap { get; init; } = paletteMap;

		public override string BaseRoute
			=> Routes.Objects;

		public override void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
		{
			var resourceRoute = parentRoute.MapGroup(Routes.ResourceRoute);
			_ = resourceRoute.MapGet(Routes.File, GetObjectFile);
			_ = resourceRoute.MapGet(Routes.Images, GetObjectImages);
		}

		public override async Task<IResult> CreateAsync(DtoObjectDescriptor request, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> ReadAsync(DbKey id, LocoDbContext db)
		{
			var eObj = await db.Objects
				.Where(x => x.Id == id)
				.Include(x => x.Licence)
				.Include(x => x.DatObjects)
				.Include(x => x.StringTable)
				.Select(x => new ExpandedTbl<TblObject, TblObjectPack>(x, x.Authors, x.Tags, x.ObjectPacks))
				.SingleOrDefaultAsync();

			return ReturnObject(eObj);
		}

		public override async Task<IResult> UpdateAsync(DbKey id, DtoObjectDescriptor request, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> DeleteAsync(DbKey id, LocoDbContext db)
			=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

		public override async Task<IResult> ListAsync(HttpContext context, LocoDbContext db)
		{
			if (context.Request.Query.Count > 0)
			{
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
			else
			{
				return Results.Ok(
					await db.Objects
						.Include(x => x.DatObjects)
						.Select(x => x.ToDtoEntry())
						.ToListAsync());
			}
		}

		// eg: http://localhost:7229/v1/objects/{id}/images
		public async Task<IResult> GetObjectImages(DbKey id, LocoDbContext db, [FromServices] ILogger<Server> logger)
		{
			// currently we MUST have a DAT backing object
			logger.LogInformation("Object [{uniqueObjectId}] requested with images", id);

			var obj = await db.Objects
				.Include(x => x.DatObjects)
				.Where(x => x.Id == id)
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
				//logger.LogWarning("Indexed object had {PathOnDisk} but the file wasn't found there; suggest re-indexing the server object folder.", pathOnDisk);
				return Results.NotFound();
			}

			if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
			{
				//logger.LogWarning("Indexed object is a vanilla object.");
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

		// eg: https://localhost:7230/objects/114
		public async Task<IResult> GetObjectFile([FromRoute] DbKey id, LocoDbContext db, [FromServices] ILogger<Server> logger)
		{
			var obj = await db.Objects
				.Include(x => x.DatObjects)
				.Where(x => x.Id == id)
				.SingleOrDefaultAsync();

			return ReturnFile(obj, logger);
		}

		public IResult ReturnObject(ExpandedTbl<TblObject, TblObjectPack>? eObj)
		{
			Console.WriteLine("[ReturnObject]");

			if (eObj == null || eObj.Object == null)
			{
				return Results.NotFound();
			}

			if (eObj.Object.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
			{
				Console.WriteLine("User attempted to download a vanilla object");
				return Results.Forbid();
			}

			var obj = eObj.ToDtoDescriptor();

			foreach (var dat in obj.DatObjects)
			{
				if (!ServerFolderManager.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry))
				{
					Console.WriteLine("Object {datFile} didn't exist in the object index", dat);
					continue;
				}

				var path = Path.Combine(ServerFolderManager.ObjectsFolder, entry!.Filename);

				if (!File.Exists(path))
				{
					Console.WriteLine("Object {datFile} existed in the object index but not on disk. ExpectedPath=\"{path}\"", dat, path);
				}

				dat.DatBytes = Convert.ToBase64String(File.ReadAllBytes(path));
			}

			return Results.Ok(obj);
		}

		IResult ReturnFile(TblObject? obj, ILogger<Server> logger)
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
			if (!ServerFolderManager.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var index))
			{
				logger.LogDebug("Object {datFile} didn't exist in the object index", dat);
				return Results.NotFound();
			}

			const string contentType = "application/octet-stream";

			var path = Path.Combine(ServerFolderManager.ObjectsFolder, index!.Filename);

			if (!File.Exists(path))
			{
				logger.LogDebug("Object {datFile} existed in the object index but not on disk. ExpectedPath=\"{path}\"", dat, path);
			}

			return Results.File(path, contentType, Path.GetFileName(path));
		}

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

			var (_, LocoObject) = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes, ssrLogger);
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(ServerFolderManager.ObjectsCustomFolder, $"{uuid}.dat");
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
				Name = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}", // same as DB seeder name
				Description = string.Empty,
				ObjectSource = ObjectSource.Custom, // not possible to upload vanilla objects
				ObjectType = hdrs.S5.ObjectType,
				VehicleType = vehicleType,
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

			ServerFolderManager.ObjectIndex.Objects.Add(new ObjectIndexEntry(saveFileName, hdrs.S5.Name, hdrs.S5.Checksum, xxHash3, uuid.ToString(), locoTbl.ObjectType, locoTbl.ObjectSource, locoTbl.CreatedDate, locoTbl.UploadedDate, locoTbl.VehicleType));

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoTbl.Name} with unique id {locoTbl.Id}", locoTbl.Id);
		}
	}
}
