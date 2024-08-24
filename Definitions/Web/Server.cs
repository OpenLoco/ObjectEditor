using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;

namespace OpenLoco.Definitions.Web
{
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
			var obj = await db.Objects.SingleOrDefaultAsync(x => x.OriginalName == objectName && x.OriginalChecksum == checksum);
			return obj == null
			? Results.NotFound()
			: Results.Ok(obj);
		}

		// eg: https://localhost:7230/objects/originaldat?uniqueObjectId=246263256
		public static async Task<IResult> GetObject(int uniqueObjectId, LocoDb db)
		{
			Console.WriteLine($"Object [{uniqueObjectId}] requested");
			var obj = await db.Objects.FindAsync(uniqueObjectId);
			if (obj == null)
			{
				return Results.NotFound();
			}

			var bytes = !obj.IsVanilla && File.Exists(obj.PathOnDisk) ? await File.ReadAllBytesAsync(obj.PathOnDisk) : null;

			return Results.Ok(new DtoLocoObject(
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
					obj.Tags,
					obj.Modpacks,
					obj.Availability,
					obj.Licence));
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
