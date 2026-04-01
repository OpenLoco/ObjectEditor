using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.SourceData;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace ObjectService.RouteHandlers.TableHandlers;

public class SC5FilePackRouteHandler : ITableRouteHandler
{
	public static string BaseRoute => RoutesV2.SC5FilePacks;
	public static Delegate ListDelegate => ListAsync;
	public static Delegate CreateDelegate => CreateAsync;
	public static Delegate ReadDelegate => ReadAsync;
	public static Delegate UpdateDelegate => UpdateAsync;
	public static Delegate DeleteDelegate => DeleteAsync;

	public static void MapRoutes(IEndpointRouteBuilder endpoints)
		=> BaseTableRouteHandler.MapRoutes<SC5FilePackRouteHandler>(endpoints);

	public static void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
	{
		var resourceRoute = parentRoute.MapGroup(RoutesV2.ResourceRoute);
		_ = resourceRoute.MapGet(RoutesV2.File, GetSC5FilePackFileAsync);
	}

	public static async Task<IResult> ListAsync(HttpContext context, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.SC5FilePacks
				.Include(l => l.Licence)
				.ToListAsync())
			.Select(x => x.ToDtoEntry())
			.OrderBy(x => x.Name));

	public static async Task<IResult> CreateAsync([FromBody] DtoItemPackDescriptor<DtoScenarioEntry> request, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db)
		=> Results.Ok(
			(await db.SC5FilePacks
				.Where(x => x.Id == id)
				.Include(l => l.Licence)
				.Select(x => new ExpandedTblPack<TblSC5FilePack, TblSC5File>(x, x.SC5Files, x.Authors, x.Tags))
				.ToListAsync())
			.Select(x => x.ToDtoDescriptor())
			.OrderBy(x => x.Name));

	public static async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoItemPackDescriptor<DtoScenarioEntry> request, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db)
		=> await Task.Run(() => Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	public static async Task<IResult> GetSC5FilePackFileAsync([FromRoute] UniqueObjectId id, [FromServices] LocoDbContext db, [FromServices] IServiceProvider sp)
	{
		var pack = await db.SC5FilePacks
			.Where(x => x.Id == id)
			.Include(x => x.SC5Files)
			.SingleOrDefaultAsync();

		if (pack == null)
		{
			return Results.NotFound();
		}

		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var tempZipPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.zip");
		var zipStream = new FileStream(
			tempZipPath,
			FileMode.Create,
			FileAccess.ReadWrite,
			FileShare.None,
			bufferSize: 4096,
			options: FileOptions.Asynchronous | FileOptions.DeleteOnClose);

		using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			foreach (var sc5File in pack.SC5Files)
			{
				if (!RouteHelpers.TryGetSafeRelativePathUnderRoot(sfm.ScenariosFolder, sc5File.Name, out var fullFilePath, out var entryName))
				{
					continue;
				}

				if (!File.Exists(fullFilePath))
				{
					continue;
				}

				await using var fileStream = File.OpenRead(fullFilePath);
				var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
				await using var entryStream = entry.Open();
				await fileStream.CopyToAsync(entryStream);
			}
		}

		zipStream.Position = 0;
		return Results.File(zipStream, "application/zip", $"{pack.Name}.zip");
	}
}
