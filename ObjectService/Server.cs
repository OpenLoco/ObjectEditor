using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.SourceData;
using OpenLoco.Definitions.Web;

namespace OpenLoco.ObjectService
{
	public class Server
	{
		public Server(ServerSettings settings)
		{
			Settings = settings;
			ObjectManager = new ObjectFolderManager(Settings.ObjectRootFolder)!;
		}

		public Server(IOptions<ServerSettings> options) : this(options.Value)
		{ }

		ServerSettings Settings { get; init; }

		ObjectFolderManager ObjectManager { get; init; }

		Common.Logging.ILogger Logger { get; } = new Logger();

		// eg: https://localhost:7230/objects/list
		public static async Task<IResult> ListObjects(LocoDb db)
			=> Results.Ok(
				await db.Objects
				.Select(x => new DtoObjectIndexEntry(
					x.Id,
					x.DatName,
					x.ObjectType,
					x.IsVanilla,
					x.DatChecksum,
					x.VehicleType)).ToListAsync());

		// eg: https://localhost:7230/objects/getdat?objectName=114&checksum=123$returnObjBytes=false
		public async Task<IResult> GetDat(string objectName, uint checksum, bool? returnObjBytes, LocoDb db, [FromServices] ILogger<Server> logger)
		{
			Console.WriteLine($"Object [({objectName}, {checksum})] requested");

			var eObj = await db.Objects
				.Where(x => x.DatName == objectName && x.DatChecksum == checksum)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
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
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return await ReturnObject(returnObjBytes, logger, eObj);
		}

		private async Task<IResult> ReturnObject(bool? returnObjBytes, ILogger<Server> logger, ExpandedTblLocoObject? eObj)
		{
			if (eObj == null || eObj.Object == null)
			{
				return Results.NotFound();
			}

			if (!ObjectManager.Index.TryFind((eObj.Object.DatName, eObj.Object.DatChecksum), out var index))
			{
				return Results.NotFound();
			}

			var obj = eObj!.Object;

			var pathOnDisk = Path.Combine(Settings.ObjectRootFolder, index!.Filename); // handle windows paths by replacing path separator
			logger.LogInformation("Loading file from {PathOnDisk}", pathOnDisk);

			var bytes = (returnObjBytes ?? false) && !obj.IsVanilla && File.Exists(pathOnDisk)
				? Convert.ToBase64String(await File.ReadAllBytesAsync(pathOnDisk))
				: null;

			var dtoObject = new DtoDatObjectWithMetadata(
				obj.Id,
				obj.UniqueName,
				obj.DatName,
				obj.DatChecksum,
				bytes,
				obj.IsVanilla,
				obj.ObjectType,
				obj.VehicleType,
				obj.Description,
				eObj.Authors,
				obj.CreationDate,
				obj.LastEditDate,
				obj.UploadDate,
				eObj.Tags,
				eObj.Modpacks,
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

			if (obj.IsVanilla)
			{
				return Results.Forbid();
			}

			if (!ObjectManager.Index.TryFind((obj.DatName, obj.DatChecksum), out var index))
			{
				return Results.NotFound();
			}

			const string contentType = "application/octet-stream";

			var path = Path.Combine(Settings.ObjectRootFolder, index!.Filename);
			return obj?.IsVanilla == false && File.Exists(path)
				? Results.File(path, contentType, Path.GetFileName(path))
				: Results.NotFound();
		}

		// eg: <todo>
		public async Task<IResult> UploadDat(DtoUploadDat request, LocoDb db)
		{
			if (string.IsNullOrEmpty(request.DatBytesAsBase64))
			{
				return Results.BadRequest("DatBytesAsBase64 cannot be null - it must contain the valid bytes of a loco dat object.");
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
				return Results.BadRequest("Unable to decode DatBytesAsBase64 - it must contain the valid bytes of a loco dat object.");
			}

			if (datFileBytes.Length > ServerLimits.MaximumUploadFileSize)
			{
				return Results.BadRequest("Unable to accept file sizes > 5MB");
			}

			if (!SawyerStreamReader.TryGetHeadersFromBytes(datFileBytes, out var hdrs))
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

			(DatFileInfo DatFileInfo, ILocoObject? LocoObject)? obj = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes, Logger);
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(ObjectManager.CustomObjectFolder, $"{uuid}.dat");
			File.WriteAllBytes(saveFileName, datFileBytes);

			Console.WriteLine($"File accepted DatName={hdrs.S5.Name} DatChecksum={hdrs.S5.Checksum} PathOnDisk={saveFileName}");

			var creationTime = request.CreationDate;

			VehicleType? vehicleType = null;
			if (obj?.LocoObject?.Object is VehicleObject veh)
			{
				vehicleType = veh.Type;
			}

			var locoTbl = new TblLocoObject()
			{
				UniqueName = $"{hdrs.S5.Name}_{hdrs.S5.Checksum}", // same as DB seeder name
				DatName = hdrs.S5.Name,
				DatChecksum = hdrs.S5.Checksum,
				IsVanilla = false, // not possible to upload vanilla objects
				ObjectType = hdrs.S5.ObjectType,
				VehicleType = vehicleType,
				Description = string.Empty,
				Authors = [],
				CreationDate = creationTime,
				LastEditDate = null,
				UploadDate = DateTimeOffset.UtcNow,
				Tags = [],
				Modpacks = [],
				Availability = obj?.LocoObject == null ? ObjectAvailability.Unavailable : ObjectAvailability.NewGames,
				Licence = null,
			};

			ObjectManager.Index.Objects.Add(new ObjectIndexEntry(saveFileName, locoTbl.DatName, locoTbl.DatChecksum, locoTbl.ObjectType, locoTbl.IsVanilla, locoTbl.VehicleType));

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoTbl.UniqueName} with unique id {locoTbl.Id}", locoTbl.Id);
		}
	}
}
