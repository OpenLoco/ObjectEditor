using Common;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
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
			await db.ObjectsMissing
				.Select(x => x.ToDtoEntry())
				.ToListAsync());
	}

	static async Task<IResult> CreateAsync([FromBody] DtoObjectMissingUpload entry, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[CreateAsync] Create requested");

		var existing = await db.ObjectsMissing
			.FirstOrDefaultAsync(x => x.DatName == entry.DatName && x.DatChecksum == entry.DatChecksum);
		if (existing != null)
		{
			return Results.Conflict($"Missing object already exists in the database. DatName={entry.DatName} DatChecksum={entry.DatChecksum}");
		}

		// save to db
		var tblObjectMissing = new TblObjectMissing()
		{
			DatName = entry.DatName,
			DatChecksum = entry.DatChecksum,
			ObjectType = entry.ObjectType,
		};

		_ = await db.ObjectsMissing.AddAsync(tblObjectMissing);
		_ = await db.SaveChangesAsync();

		return Results.Created($"Successfully added missing object {entry.DatName} with checksum {entry.DatChecksum} and unique id {tblObjectMissing.Id}", tblObjectMissing.ToDtoEntry());
	}

	static async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[ReadAsync] Read requested");

		var existing = await db.ObjectsMissing
			.FirstOrDefaultAsync(x => x.Id == id);

		if (existing == null)
		{
			return Results.NotFound();
		}

		return Results.Ok(existing.ToDtoEntry());
	}

	static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoObjectMissingUpload request, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[UpdateAsync] UpdateAsync requested");

		return await Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));
	}

	static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] ILogger<ObjectMissingRouteHandler> logger)
	{
		logger.LogInformation("[DeleteAsync] Delete requested");

		var existing = await db.ObjectsMissing
			.FirstOrDefaultAsync(x => x.Id == id);

		if (existing == null)
		{
			return Results.NotFound();
		}

		_ = db.ObjectsMissing.Remove(existing);
		_ = await db.SaveChangesAsync();

		return Results.Ok();
	}
}
