using Dat.Services;
using Definitions.Database;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;

namespace ObjectService.RouteHandlers.TableHandlers;

public static class FolderRouteHandler
{
	// Marker type used only as a logger category, because static classes cannot
	// be used as the type argument for ILogger<T>.
	public sealed class LoggerCategory { }

	public static void MapRoutes(IEndpointRouteBuilder parentRoute)
	{
		var group = parentRoute.MapGroup(RoutesV2.Folders);
		_ = group.MapPost(RoutesV2.Index, IndexFolderAsync);
	}

	public static async Task<IResult> IndexFolderAsync(
		DtoIndexFolderRequest request,
		[FromServices] ServerFolderManager sfm,
		[FromServices] ILogger<LoggerCategory> logger,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request?.Path))
		{
			return Results.Problem("Path is required.", statusCode: StatusCodes.Status400BadRequest);
		}

		var path = request.Path;
		if (!Directory.Exists(path))
		{
			return Results.Problem($"Directory does not exist on the server: {path}", statusCode: StatusCodes.Status404NotFound);
		}

		logger.LogInformation("[IndexFolder] Indexing folder \"{Path}\"", path);

		LocoDbContext ContextFactory() => LocoDbContext.GetDbFromFile(sfm.DatabasePath)
			?? throw new FileNotFoundException($"Database file not found: {sfm.DatabasePath}");

		// Scan the directory once so we can report scanned/failed counts.
		var scan = await DatFolderScanner.ScanDirectoryAsync(path, logger, progress: null, cancellationToken);

		HashSet<ulong> existingHashes;
		await using (var db = ContextFactory())
		{
			existingHashes = [.. db.DatObjects.Select(d => d.xxHash3)];
		}

		var succeededHashes = scan.Succeeded.Select(r => r.xxHash3).Distinct().ToList();
		var addedCount = succeededHashes.Count(h => !existingHashes.Contains(h));
		var skippedCount = succeededHashes.Count - addedCount;

		// Hand off to the index service for the additive insert. Existing hashes
		// are skipped inside the service.
		var indexService = new LocalObjectIndexService(ContextFactory, logger);
		var relativeFiles = scan.Succeeded.Select(r => r.RelativePath).ToList();
		await indexService.AddFilesAsync(path, relativeFiles, progress: null, cancellationToken);

		var scannedCount = scan.Succeeded.Count + scan.Failed.Count;
		logger.LogInformation(
			"[IndexFolder] Done. Scanned={Scanned} Added={Added} Skipped={Skipped} Failed={Failed}",
			scannedCount, addedCount, skippedCount, scan.Failed.Count);

		return Results.Ok(new DtoIndexFolderResponse(path, scannedCount, addedCount, skippedCount, scan.Failed.Count));
	}
}
