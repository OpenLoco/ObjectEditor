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

namespace OpenLoco.ObjectService
{
	public class Server
	{
		public Server(ServerSettings settings)
		{
			Settings = settings;
			Index = ObjectIndex.LoadOrCreateIndex(Settings.ObjectRootFolder)!;
		}

		public Server(IOptions<ServerSettings> options) : this(options.Value)
		{ }

		ServerSettings Settings { get; init; }
		ObjectIndex Index { get; init; }
		Common.Logging.ILogger Logger { get; } = new Logger();

		// eg: https://localhost:7230/objects/list
		public static async Task<IResult> ListObjects(LocoDb db)
			=> Results.Ok(
				await db.Objects
				.Select(x => new DtoObjectIndexEntry(
					x.Id,
					x.OriginalName,
					x.ObjectType,
					x.IsVanilla,
					x.OriginalChecksum,
					x.VehicleType)).ToListAsync());

		// eg: https://localhost:7230/objects/getdat?objectName=114&checksum=123$returnObjBytes=false
		public async Task<IResult> GetDat(string objectName, uint checksum, bool? returnObjBytes, LocoDb db, [FromServices] ILogger<Server> logger)
		{
			Console.WriteLine($"Object [({objectName}, {checksum})] requested");

			var eObj = await db.Objects
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
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

			if (!Index.TryFind((eObj.Object.OriginalName, eObj.Object.OriginalChecksum), out var index))
			{
				return Results.NotFound();
			}

			var obj = eObj!.Object;

			var pathOnDisk = Path.Combine(Settings.ObjectRootFolder, index!.Filename); // handle windows paths by replacing path separator
			logger.LogInformation("Loading file from {PathOnDisk}", pathOnDisk);

			var bytes = (returnObjBytes ?? false) && !obj.IsVanilla && File.Exists(pathOnDisk)
				? Convert.ToBase64String(await File.ReadAllBytesAsync(pathOnDisk))
				: null;

			var dtoObject = new DtoLocoObject(
				obj.Id,
				obj.Name,
				obj.OriginalName,
				obj.OriginalChecksum,
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
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
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

			if (!Index.TryFind((obj.OriginalName, obj.OriginalChecksum), out var index))
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

			var creationTime = request.CreationDate;

			var obj = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes, Logger);
			if (obj == null || obj.Value.LocoObject == null)
			{
				return Results.BadRequest("Provided data was unable to be decoded into a real loco dat object.");
			}

			var datFileInfo = obj.Value.DatFileInfo;
			var locoObject = obj.Value.LocoObject;
			var s5Header = datFileInfo.S5Header;

			if (!s5Header.IsOriginal())
			{
				return Results.BadRequest("Nice try genius. Uploading vanilla objects is not allowed.");
			}

			if (!s5Header.IsValid())
			{
				return Results.BadRequest("Invalid DAT file.");
			}

			if (db.DoesObjectExist(s5Header, out var existingObject))
			{
				return Results.Accepted($"Object already exists in the database. OriginalName={s5Header.Name} OriginalChecksum={s5Header.Checksum} UploadDate={existingObject!.UploadDate}");
			}

			const string UploadFolder = "UploadedObjects";
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(Settings.ObjectRootFolder, UploadFolder, $"{uuid}.dat");
			File.WriteAllBytes(saveFileName, datFileBytes);

			Console.WriteLine($"File accepted OriginalName={s5Header.Name} OriginalChecksum={s5Header.Checksum} PathOnDisk={saveFileName}");

			var locoTbl = new TblLocoObject()
			{
				Name = $"{s5Header.Name}_{s5Header.Checksum}", // same as DB seeder name
				OriginalName = s5Header.Name,
				OriginalChecksum = s5Header.Checksum,
				IsVanilla = false, // not possible to upload vanilla objects
				ObjectType = s5Header.ObjectType,
				VehicleType = s5Header.ObjectType == ObjectType.Vehicle ? (locoObject.Object as VehicleObject)!.Type : null,
				Description = "",
				Authors = [],
				CreationDate = creationTime,
				LastEditDate = null,
				UploadDate = DateTimeOffset.UtcNow,
				Tags = [],
				Modpacks = [],
				Availability = ObjectAvailability.NewGames,
				Licence = null,
			};

			Index.AddObject(new ObjectIndexEntry(saveFileName, locoTbl.OriginalName, locoTbl.ObjectType, locoTbl.IsVanilla, locoTbl.OriginalChecksum, locoTbl.VehicleType));

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoTbl.Name} with unique id {locoTbl.Id}", locoTbl.Id);
		}
	}
}
