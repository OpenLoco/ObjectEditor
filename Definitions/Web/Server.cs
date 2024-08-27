using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;

namespace OpenLoco.Definitions.Web
{
	// this must be done because eager-loading related many-to-many data in entity framework is recursive and cannot be turned off...
	public record ExpandedTblLocoObject(TblLocoObject Object, ICollection<TblAuthor> Authors, ICollection<TblTag> Tags, ICollection<TblModpack> Modpacks);

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

		// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
		public static async Task<IResult> GetDat(string objectName, uint checksum, bool returnObjBytes, LocoDb db)
		{
			var eObj = await db.Objects
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return eObj == null || eObj.Object == null
				? Results.NotFound()
				: Results.Ok(await PrepareLocoObject(eObj, returnObjBytes));
		}

		// eg: https://localhost:7230/objects/originaldat?uniqueObjectId=246263256&returnObjBytes=false
		public static async Task<IResult> GetObject(int uniqueObjectId, bool returnObjBytes, LocoDb db)
		{
			Console.WriteLine($"Object [{uniqueObjectId}] requested");
			var eObj = await db.Objects
				.Where(x => x.Id == uniqueObjectId)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Authors, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return eObj == null || eObj.Object == null
				? Results.NotFound()
				: Results.Ok(await PrepareLocoObject(eObj, returnObjBytes));
		}

		// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
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

		// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
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

		public static async Task<DtoLocoObject> PrepareLocoObject(ExpandedTblLocoObject eObj, bool returnObjBytes)
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
		public static async Task<IResult> UploadDat(DtoLocoObject locoObject, LocoDb db)
		{
			if (locoObject.OriginalBytes == null)
			{
				return Results.BadRequest("OriginalBytes cannot be null - it must contain the valid bytes of a loco dat object.");
			}

			var obj = SawyerStreamReader.LoadAndDecodeFromStream(Convert.FromBase64String(locoObject.OriginalBytes));
			if (obj == null || obj.Value.decodedData.Length == 0)
			{
				return Results.BadRequest("Provided byte data was unable to be decoded into a real loco dat object.");
			}

			if (db.DoesObjectExist(locoObject.OriginalName, locoObject.OriginalChecksum))
			{
				return Results.Accepted("Provided object already exists in the database.");
			}

			const string UploadFolder = "Q:\\Games\\Locomotion\\Server\\CustomObjects\\Uploaded";
			var uuid = Guid.NewGuid();
			var saveFileName = Path.Combine(UploadFolder, uuid.ToString(), ".dat");
			Console.WriteLine($"File accepted OriginalName={locoObject.OriginalName} OriginalChecksum={locoObject.OriginalChecksum} PathOnDisk={saveFileName}");
			_ = obj.Value.s5Header;
			_ = obj.Value.objHeader;
			//var decodedData = obj.Value.decodedData;

			//ObjectIndex.AddObject(await SawyerStreamReader.GetDatFileInfoFromBytesAsync((saveFileName, locoObject.OriginalBytes)));

			var locoTbl = new TblLocoObject()
			{
				// for now, trust DtoLocoObj, but we could full-parse here
				Name = locoObject.Name,
				PathOnDisk = saveFileName,
				OriginalName = locoObject.OriginalName,
				OriginalChecksum = locoObject.OriginalChecksum,
				IsVanilla = locoObject.IsVanilla,
				ObjectType = locoObject.ObjectType,
				VehicleType = locoObject.VehicleType,
				Description = locoObject.Description,
				Authors = locoObject.Authors,
				CreationDate = locoObject.CreationDate,
				LastEditDate = locoObject.LastEditDate,
				UploadDate = DateTimeOffset.UtcNow,
				Tags = locoObject.Tags,
				Modpacks = locoObject.Modpacks,
				Availability = locoObject.Availability,
				Licence = locoObject.Licence
			};

			_ = db.Objects.Add(locoTbl);
			_ = await db.SaveChangesAsync();

			return Results.Created($"Successfully added {locoObject.Name} with unique id {locoTbl.Id}", locoTbl.Id);
		}
	}
}
