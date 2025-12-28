using Common;
using Common.Logging;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.SourceData;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectMissingRouteHandler : ITableRouteHandler
{
	public static string BaseRoute => RoutesV2.Missing;
	public static Delegate ListDelegate => ListMissingObjects;
	public static Delegate CreateDelegate => AddMissingObject;
	public static Delegate ReadDelegate => ReadNotImplementedAsync;
	public static Delegate UpdateDelegate => UpdateNotImplementedAsync;
	public static Delegate DeleteDelegate => DeleteNotImplementedAsync;

	static async Task<IResult> ListMissingObjects([FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[ListMissingObjects] List requested for missing objects");

		return Results.Ok(
			await db.Objects
				.Include(x => x.DatObjects)
				.Where(x => x.Availability == ObjectAvailability.Missing)
				.Select(x => x.ToDtoEntry())
				.ToListAsync());
	}

	static async Task<IResult> AddMissingObject([FromBody] DtoMissingObjectEntry entry, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		var objName = $"{entry.DatName}_{entry.DatChecksum}";
		var existing = await db.Objects.FirstOrDefaultAsync(x => x.Name == objName);
		if (existing != null)
		{
			return Results.Conflict($"Object already exists in the database. DatName={entry.DatName} DatChecksum={entry.DatChecksum} UploadedDate={existing!.UploadedDate}");
		}

		// double check it's missing
		if (db.DoesObjectExist(entry.DatName, entry.DatChecksum, out var existingObject) && existingObject != null)
		{
			return Results.Conflict($"Object already exists in the database. UniqueId={existingObject.Id} DatName={entry.DatName} DatChecksum={entry.DatChecksum} UploadedDate={existingObject!.UploadedDate}");
		}

		// save to db if true
		var tblObject = new TblObject()
		{
			Name = $"{entry.DatName}_{entry.DatChecksum}",
			Description = string.Empty,
			ObjectSource = ObjectSource.Custom,
			ObjectType = entry.ObjectType,
			VehicleType = null,
			Availability = ObjectAvailability.Missing,
			CreatedDate = null,
			ModifiedDate = null,
			UploadedDate = DateOnly.UtcToday,
			Authors = [],
			Tags = [],
			ObjectPacks = [],
			DatObjects = [],
			StringTable = [],
			SubObjectId = 0,
			Licence = null,
		};

		_ = await db.Objects.AddAsync(tblObject);
		_ = await db.SaveChangesAsync();

		// make dat objects
		//var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		tblObject.DatObjects.Add(new TblDatObject()
		{
			ObjectId = tblObject.Id,
			DatName = entry.DatName,
			DatChecksum = entry.DatChecksum,
			xxHash3 = 0,
			Object = tblObject,
		});

		// save again
		_ = await db.SaveChangesAsync();
		return Results.Created($"Successfully added 'missing' DAT object {tblObject.Name} with checksum {entry.DatChecksum} and unique id {tblObject.Id}", tblObject.Id);
	}

	static Task<IResult> ReadNotImplementedAsync(UniqueObjectId id, [FromServices] LocoDbContext db)
		=> Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static Task<IResult> UpdateNotImplementedAsync(UniqueObjectId id, DtoMissingObjectEntry request, [FromServices] LocoDbContext db)
		=> Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static Task<IResult> DeleteNotImplementedAsync(UniqueObjectId id, [FromServices] LocoDbContext db)
		=> Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
}
