using Common;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectMissingRouteHandler : ITableRouteHandler
{
	public static string BaseRoute => RoutesV2.Missing;
	public static Delegate ListDelegate => ListAsync;
	public static Delegate CreateDelegate => CreateAsync;
	public static Delegate ReadDelegate => ReadAsync;
	public static Delegate UpdateDelegate => UpdateAsync;
	public static Delegate DeleteDelegate => DeleteAsync;

	public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
	{
		// No additional routes needed for missing objects beyond the standard CRUD operations
	}

	static async Task<IResult> ListAsync([FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[ListAsync] List requested for missing objects");

		return Results.Ok(
			await db.Objects
				.Where(x => x.Availability == ObjectAvailability.Missing)
				.Select(x => x.ToDtoEntry())
				.ToListAsync());
	}

	static async Task<IResult> CreateAsync([FromBody] DtoMissingObjectEntry entry, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[CreateAsync] Create requested");

		var objName = $"{entry.DatName}_{entry.DatChecksum}";
		var existing = await db.Objects.FirstOrDefaultAsync(x => x.Name == objName);
		if (existing != null)
		{
			return Results.Conflict($"Object already exists in the database. DatName={entry.DatName} DatChecksum={entry.DatChecksum} UploadedDate={existing.UploadedDate}");
		}

		// double check it's missing
		if (db.DoesObjectExist(entry.DatName, entry.DatChecksum, out var existingObject) && existingObject != null)
		{
			return Results.Conflict($"Object already exists in the database. UniqueId={existingObject.Id} DatName={entry.DatName} DatChecksum={entry.DatChecksum} UploadedDate={existingObject.UploadedDate}");
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

	static async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[ReadAsync] Read requested");

		var existing = await db.Objects
			.FirstOrDefaultAsync(x => x.Availability == ObjectAvailability.Missing && x.Id == id);

		if (existing == null)
		{
			return Results.NotFound();
		}

		return Results.Ok(existing);
	}

	static Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoMissingObjectEntry request, [FromServices] LocoDbContext db)
		=> Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[DeleteAsync] Delete requested");

		var existing = await db.Objects
			.FirstOrDefaultAsync(x => x.Availability == ObjectAvailability.Missing && x.Id == id);

		if (existing == null)
		{
			return Results.NotFound();
		}

		_ = db.Objects.Remove(existing);
		_ = await db.SaveChangesAsync();

		return Results.Ok();
	}
}
