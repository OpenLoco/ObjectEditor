using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;

namespace OpenLoco.Definitions.Web
{
	// this must be done because eager-loading related many-to-many data in entity framework is recursive and cannot be turned off...
	internal record ExpandedTblLocoObject(TblLocoObject Object, ICollection<TblAuthor> Authors, ICollection<TblTag> Tags, ICollection<TblModpack> Modpacks);

	public static class Server
	{
		// eg: https://localhost:7230/objects/list
		public static async Task<IResult> ListObjects(LocoDb db)
			=> Results.Ok(
				await db.Objects
				.Where(x => (int)x.ObjectType < Limits.kMaxObjectTypes) // for now - only return value objects
				.Select(x => new DtoObjectIndexEntry(
					x.Id,
					x.OriginalName,
					x.ObjectType,
					x.IsVanilla,
					x.OriginalChecksum,
					x.VehicleType)).ToListAsync());

		// eg: https://localhost:7230/objects/getdat?objectName=114&checksum=123$returnObjBytes=false
		public static async Task<IResult> GetDat(string objectName, uint checksum, bool? returnObjBytes, LocoDb db)
		{
			var eObj = await db.Objects
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return eObj == null || eObj.Object == null
				? Results.NotFound()
				: Results.Ok(await PrepareLocoObject(eObj, returnObjBytes ?? false));
		}

		// eg: https://localhost:7230/objects/getobject?uniqueObjectId=246263256&returnObjBytes=false
		public static async Task<IResult> GetObject(int uniqueObjectId, bool? returnObjBytes, LocoDb db)
		{
			Console.WriteLine($"Object [{uniqueObjectId}] requested");
			var eObj = await db.Objects
				.Where(x => x.Id == uniqueObjectId)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return eObj == null || eObj.Object == null
				? Results.NotFound()
				: Results.Ok(await PrepareLocoObject(eObj, returnObjBytes ?? false));
		}

		// eg: https://localhost:7230/objects/originaldatfile?objectName=114&checksum=123
		public static async Task<IResult> GetDatFile(string objectName, uint checksum, LocoDb db)
		{
			var obj = await db.Objects
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
				.SingleOrDefaultAsync();

			const string contentType = "application/octet-stream";

			return obj?.IsVanilla == false && File.Exists(obj.PathOnDisk)
				? Results.File(obj.PathOnDisk, contentType, Path.GetFileName(obj.PathOnDisk))
				: Results.NotFound();
		}

		// eg: https://localhost:7230/objects/getobjectfile?objectName=114&checksum=123
		public static async Task<IResult> GetObjectFile(int uniqueObjectId, LocoDb db)
		{
			var obj = await db.Objects
				.Where(x => x.Id == uniqueObjectId)
				.SingleOrDefaultAsync();

			const string contentType = "application/octet-stream";

			return obj?.IsVanilla == false && File.Exists(obj.PathOnDisk)
				? Results.File(obj.PathOnDisk, contentType, Path.GetFileName(obj.PathOnDisk))
				: Results.NotFound();
		}

		internal static async Task<DtoLocoObject> PrepareLocoObject(ExpandedTblLocoObject eObj, bool returnObjBytes)
		{
			var obj = eObj!.Object;
			var bytes = returnObjBytes && !obj.IsVanilla && File.Exists(obj.PathOnDisk) ? Convert.ToBase64String(await File.ReadAllBytesAsync(obj.PathOnDisk)) : null;

			return new DtoLocoObject(
				obj.Id,
				obj.Name,
				obj.OriginalName,
				obj.OriginalChecksum,
				bytes,
				obj.IsVanilla,
				obj.ObjectType,
				obj.VehicleType,
				obj.Description,
				obj.Authors,
				obj.CreationDate,
				obj.LastEditDate,
				obj.UploadDate,
				eObj.Tags,
				eObj.Modpacks,
				obj.Availability,
				obj.Licence);
		}

		// eg: <todo>
		public static async Task<IResult> UploadDat(DtoUploadDat request, LocoDb db)
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

			var obj = SawyerStreamReader.LoadFullObjectFromStream(datFileBytes);
			if (obj == null || obj.Value.LocoObject == null)
			{
				return Results.BadRequest("Provided data was unable to be decoded into a real loco dat object.");
			}

			var datFileInfo = obj.Value.DatFileInfo;
			var locoObject = obj.Value.LocoObject;
			var s5Header = datFileInfo.S5Header;

			if (OriginalObjectFiles.Names.TryGetValue(s5Header.Name, out var chksum) && chksum == s5Header.Checksum)
			{
				return Results.BadRequest("Nice try genius. Uploading vanilla objects is not allowed.");
			}

			if (db.DoesObjectExist(s5Header, out var existingObject))
			{
				return Results.Accepted($"Object already exists in the database. OriginalName={s5Header.Name} OriginalChecksum={s5Header.Checksum} UploadDate={existingObject!.UploadDate}");
			}

			const string UploadFolder = "Q:\\Games\\Locomotion\\Server\\CustomObjects\\Uploaded";
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(UploadFolder, $"{uuid}.dat");
			Console.WriteLine($"File accepted OriginalName={s5Header.Name} OriginalChecksum={s5Header.Checksum} PathOnDisk={saveFileName}");

			var locoTbl = new TblLocoObject()
			{
				Name = $"{s5Header.Name}_{s5Header.Checksum}", // same as DB seeder name
				PathOnDisk = saveFileName,
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

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoTbl.Name} with unique id {locoTbl.Id}", locoTbl.Id);
		}
	}
}
