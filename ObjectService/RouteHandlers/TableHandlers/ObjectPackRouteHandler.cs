using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.SourceData;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectPackRouteHandler : ITableRouteHandler
{
	public static string BaseRoute => RoutesV2.ObjectPacks;
	public static Delegate ListDelegate => ListAsync;
	public static Delegate CreateDelegate => CreateAsync;
	public static Delegate ReadDelegate => ReadAsync;
	public static Delegate UpdateDelegate => UpdateAsync;
	public static Delegate DeleteDelegate => DeleteAsync;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<ObjectPackRouteHandler>(endpoints);

	public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
	{
		var resourceRoute = parentRoute.MapGroup(RoutesV2.ResourceRoute);
		_ = resourceRoute.MapGet(RoutesV2.File, GetObjectPackFileAsync);
	}

	public static async Task<IResult> ListAsync(HttpContext context, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.ObjectPacks
				.Include(l => l.Licence)
				.ToListAsync())
			.Select(x => x.ToDtoEntry())
			.OrderBy(x => x.Name));

	public static async Task<IResult> CreateAsync([FromBody] DtoItemPackDescriptor<DtoObjectEntry> request, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> ReadAsync(UniqueObjectId id, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.ObjectPacks
				.Where(x => x.Id == id)
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblObjectPack, TblObject>(x, x.Objects, x.Authors, x.Tags))
				.ToListAsync())
			.Select(x => x.ToDtoDescriptor())
			.OrderBy(x => x.Name));

	public static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoItemPackDescriptor<DtoObjectEntry> request, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> GetObjectPackFileAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] IServiceProvider sp)
	{
		var pack = await db.ObjectPacks
			.Where(x => x.Id == id)
			.Include(x => x.Objects)
				.ThenInclude(o => o.DatObjects)
			.SingleOrDefaultAsync();

		if (pack == null)
		{
			return Results.NotFound();
		}

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var zipStream = new MemoryStream();

		using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			foreach (var obj in pack.Objects)
			{
				if (obj.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
				{
					continue;
				}

				foreach (var dat in obj.DatObjects)
				{
					if (!sfm.ObjectIndex.TryFind((dat.DatName, dat.DatChecksum), out var entry) || entry == null || string.IsNullOrEmpty(entry.FileName))
					{
						continue;
					}

					var filePath = Path.Combine(sfm.ObjectsFolder, entry.FileName);
					if (!File.Exists(filePath))
					{
						continue;
					}

					// Use the relative path from the objects folder as the entry name to avoid duplicate filename collisions
					archive.CreateEntryFromFile(filePath, entry.FileName.Replace('\\', '/'));
				}
			}
		}

		zipStream.Position = 0;
		return Results.File(zipStream, "application/zip", $"{pack.Name}.zip");
	}
}
