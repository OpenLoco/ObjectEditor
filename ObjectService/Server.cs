using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.SourceData;
using OpenLoco.Definitions.Web;
using System.Security.AccessControl;

namespace OpenLoco.ObjectService
{
	public class Server
	{
		public Server(ServerSettings settings)
		{
			Settings = settings;
			ServerFolderManager = new ServerFolderManager(Settings.RootFolder)!;
		}

		public Server(IOptions<ServerSettings> options) : this(options.Value)
		{ }

		ServerSettings Settings { get; init; }

		ServerFolderManager ServerFolderManager { get; init; }

		Common.Logging.ILogger Logger { get; } = new Logger();

		// eg: https://localhost:7230/objects/list
		public static async Task<IResult> ListObjects(LocoDb db)
			=> Results.Ok(
				await db.Objects
				.Select(x => new DtoObjectIndexEntry(
					x.Id,
					x.DatName,
					x.DatChecksum,
					x.ObjectType,
					x.ObjectSource,
					x.VehicleType)).ToListAsync());

		// eg: https://localhost:7230/objects/search
		public static async Task<IResult> SearchObjects(LocoDb db)
		{
			Console.WriteLine($"Object search requested");
			return Results.Problem(statusCode: StatusCodes.Status501NotImplemented);
		}

		// eg: https://localhost:7230/objects/getdat?objectName=114&checksum=123$returnObjBytes=false
		public async Task<IResult> GetDat(string objectName, uint checksum, bool? returnObjBytes, LocoDb db, [FromServices] ILogger<Server> logger)
		{
			Console.WriteLine($"Object [({objectName}, {checksum})] requested");

			var eObj = await db.Objects
				.Where(x => x.DatName == objectName && x.DatChecksum == checksum)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.ObjectPacks))
				.SingleOrDefaultAsync();

			return await ReturnObject(returnObjBytes, logger, eObj);
		}

		// eg: https://localhost:7230/objects/getobject?uniqueObjectId=246263256&returnObjBytes=false
		public async Task<IResult> GetObject(int uniqueObjectId, bool? returnObjBytes, LocoDb db, [FromServices] ILogger<Server> logger)
		{
			Console.WriteLine($"Object [{uniqueObjectId}] requested");

			var eObj = await db.Objects
				.Where(x => x.Id == uniqueObjectId)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.ObjectPacks))
				.SingleOrDefaultAsync();

			return await ReturnObject(returnObjBytes, logger, eObj);
		}

		private async Task<IResult> ReturnObject(bool? returnObjBytes, ILogger<Server> logger, ExpandedTblLocoObject? eObj)
		{
			if (eObj == null || eObj.Object == null)
			{
				return Results.NotFound();
			}

			if (!ServerFolderManager.ObjectIndex.TryFind((eObj.Object.DatName, eObj.Object.DatChecksum), out var index))
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

			var bytes = (returnObjBytes ?? false) && (obj.ObjectSource is ObjectSource.Custom or ObjectSource.OpenLoco) && fileExists
				? Convert.ToBase64String(await File.ReadAllBytesAsync(pathOnDisk))
				: null;

			var dtoObject = new DtoDatObjectWithMetadata(
				obj.Id,
				obj.UniqueName,
				obj.DatName,
				obj.DatChecksum,
				bytes,
				obj.ObjectSource,
				obj.ObjectType,
				obj.VehicleType,
				obj.Description,
				eObj.Authors,
				obj.CreationDate,
				obj.LastEditDate,
				obj.UploadDate,
				eObj.Tags,
				eObj.ObjectPacks,
				obj.Availability,
				obj.Licence);

			return Results.Ok(dtoObject);
		}

		// eg: https://localhost:7230/objects/originaldatfile?objectName=114&checksum=123
		public async Task<IResult> GetDatFile(string objectName, uint checksum, LocoDb db)
		{
			var obj = await db.Objects
				.Where(x => x.DatName == objectName && x.DatChecksum == checksum)
				.SingleOrDefaultAsync();

			return ReturnFile(obj);
		}

		// eg: https://localhost:7230/objects/getobjectfile?objectName=114&checksum=123
		public async Task<IResult> GetObjectFile(int uniqueObjectId, LocoDb db)
		{
			var obj = await db.Objects
				.Where(x => x.Id == uniqueObjectId)
				.SingleOrDefaultAsync();

			return ReturnFile(obj);
		}

		IResult ReturnFile(TblLocoObject? obj)
		{
			if (obj == null)
			{
				return Results.NotFound();
			}

			if (obj.ObjectSource is ObjectSource.Custom or ObjectSource.OpenLoco)
			{
				return Results.Forbid();
			}

			if (!ServerFolderManager.ObjectIndex.TryFind((obj.DatName, obj.DatChecksum), out var index))
			{
				return Results.NotFound();
			}

			const string contentType = "application/octet-stream";

			var path = Path.Combine(ServerFolderManager.ObjectsFolder, index!.Filename);
			return obj != null && File.Exists(path)
				? Results.File(path, contentType, Path.GetFileName(path))
				: Results.NotFound();
		}

		// eg: https://localhost:7230/scenarios/list
		public async Task<IResult> ListScenarios(LocoDb db)
			=> await Task.Run(() =>
			{
				var files = Directory.GetFiles(ServerFolderManager.ScenariosFolder, "*.SC5", SearchOption.AllDirectories);
				var count = 0;
				var filenames = files.Select(x => new DtoScenarioIndexEntry(count++, Path.GetRelativePath(ServerFolderManager.ScenariosFolder, x)));
				return Results.Ok(filenames.ToList());
			});

		// eg: https://localhost:7230/scenarios/getscenario?uniqueScenarioId=246263256&returnObjBytes=false
		public async Task<IResult> GetScenario(int uniqueScenarioId, bool? returnObjBytes, LocoDb db, [FromServices] ILogger<Server> logger)
		{
			Console.WriteLine($"Scenario [{uniqueScenarioId}] requested");
			return await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
		}

		// eg: <todo>
		public async Task<IResult> UploadDat(DtoUploadDat request, LocoDb db, [FromServices] ILogger<Server> logger)
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
				return Results.Accepted($"Object already exists in the database. DatName={hdrs.S5.Name} DatChecksum={hdrs.S5.Checksum} UploadDate={existingObject!.UploadDate}");
			}

			// at this stage, headers must be valid. we can add it to the object index/database, even if the remainder of the object is invalid

			var (_, LocoObject) = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes, Logger);
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(ServerFolderManager.ObjectsCustomFolder, $"{uuid}.dat");
			File.WriteAllBytes(saveFileName, datFileBytes);

			Console.WriteLine($"File accepted DatName={hdrs.S5.Name} DatChecksum={hdrs.S5.Checksum} PathOnDisk={saveFileName}");

			var creationTime = request.CreationDate;

			VehicleType? vehicleType = null;
			if (LocoObject?.Object is VehicleObject veh)
			{
				vehicleType = veh.Type;
			}

			var locoTbl = new TblLocoObject()
			{
				UniqueName = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}", // same as DB seeder name
				DatName = hdrs.S5.Name,
				DatChecksum = hdrs.S5.Checksum,
				ObjectSource = ObjectSource.Custom, // not possible to upload vanilla objects
				ObjectType = hdrs.S5.ObjectType,
				VehicleType = vehicleType,
				Description = string.Empty,
				Authors = [],
				CreationDate = creationTime,
				LastEditDate = null,
				UploadDate = DateTimeOffset.UtcNow,
				Tags = [],
				ObjectPacks = [],
				Availability = LocoObject == null ? ObjectAvailability.Unavailable : ObjectAvailability.AllGames,
				Licence = null,
			};

			ServerFolderManager.ObjectIndex.Objects.Add(new ObjectIndexEntry(saveFileName, locoTbl.DatName, locoTbl.DatChecksum, locoTbl.ObjectType, locoTbl.ObjectSource, locoTbl.VehicleType));

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoTbl.UniqueName} with unique id {locoTbl.Id}", locoTbl.Id);
		}
	}
}
