using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;

namespace OpenLoco.Definitions.Web
{
	// this must be done because eager-loading related many-to-many data in entity framework is recursive and cannot be turned off...
	public record ExpandedTblLocoObject(TblLocoObject Object, ICollection<TblTag> Tags, ICollection<TblModpack> Modpacks);

	public static class Server
	{
		// eg: https://localhost:7230/objects/list
		public static async Task<IResult> ListObjects(LocoDb db)
			=> Results.Ok(
				await db.Objects.Select(x => new DtoObjectIndexEntry(
					x.TblLocoObjectId,
					x.OriginalName,
					x.ObjectType,
					x.IsVanilla,
					x.OriginalChecksum,
					x.VehicleType)).ToListAsync());

		// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
		public static async Task<IResult> GetDat(string objectName, uint checksum, LocoDb db)
		{
			var eObj = await db.Objects
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
				.Include(x => x.Author)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return eObj == null || eObj.Object == null
				? Results.NotFound()
				: Results.Ok(await PrepareLocoObject(eObj));
		}

		// eg: https://localhost:7230/objects/originaldat?objectName=114&checksum=123
		public static async Task<IResult> GetDatFile(string objectName, uint checksum, LocoDb db)
		{
			var obj = await db.Objects
				.Where(x => x.OriginalName == objectName && x.OriginalChecksum == checksum)
				.SingleOrDefaultAsync();

			var fileExists = !obj.IsVanilla && File.Exists(obj.PathOnDisk);

			return fileExists
				? Results.NotFound()
				: Results.File(obj.PathOnDisk);
		}

		// eg: https://localhost:7230/objects/originaldat?uniqueObjectId=246263256
		public static async Task<IResult> GetObject(int uniqueObjectId, LocoDb db)
		{
			Console.WriteLine($"Object [{uniqueObjectId}] requested");
			var eObj = await db.Objects
				.Where(x => x.TblLocoObjectId == uniqueObjectId)
				.Include(x => x.Author)
				.Include(x => x.Licence)
				.Select(x => new ExpandedTblLocoObject(x, x.Tags, x.Modpacks))
				.SingleOrDefaultAsync();

			return eObj == null || eObj.Object == null
				? Results.NotFound()
				: Results.Ok(await PrepareLocoObject(eObj));
		}

		public static async Task<DtoLocoObject> PrepareLocoObject(ExpandedTblLocoObject eObj)
		{
			var obj = eObj!.Object;
			var bytes = !obj.IsVanilla && File.Exists(obj.PathOnDisk) ? await File.ReadAllBytesAsync(obj.PathOnDisk) : null;

			return new DtoLocoObject(
				obj.TblLocoObjectId,
				obj.Name,
				obj.OriginalName,
				obj.OriginalChecksum,
				bytes,
				obj.IsVanilla,
				obj.ObjectType,
				obj.VehicleType,
				obj.Description,
				obj.Author,
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

			var obj = SawyerStreamReader.LoadAndDecodeFromStream(locoObject.OriginalBytes);
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
				Author = locoObject.Author,
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

			return Results.Created($"Successfully added {locoObject.Name} with unique id {locoTbl.TblLocoObjectId}", locoTbl.TblLocoObjectId);
		}
	}
}
