using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObjectService.Services;
using System.Security.Claims;

namespace ObjectService.RouteHandlers.TableHandlers;

public class ObjectRouteHandler : ITableRouteHandler
{
	public string BaseRoute => Routes.Objects;
	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateDatAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder parentRoute)
	{
		var config = parentRoute.ServiceProvider.GetRequiredService<IConfiguration>();
		BaseTableRouteHandler.MapRoutes(this, parentRoute, config);
	}

	public void MapAdditionalRoutes(IEndpointRouteBuilder parentRoute)
	{
		_ = parentRoute.MapGet(Routes.Mine, ListMineAsync).RequireAuthorization();

		var resourceRoute = parentRoute.MapGroup(Routes.ResourceRoute);
		_ = resourceRoute.MapGet(Routes.File, GetObjectFileAsync);
		_ = resourceRoute.MapGet(Routes.Images, GetObjectImagesAsync);

		// Output cache the small immutable image responses (see the "ObjectImages" policy in Program.cs).
		var imagesRoute = resourceRoute.MapGroup(Routes.Images);
		_ = imagesRoute.MapGet(Routes.ImageMetadata, GetObjectImageMetadataAsync).CacheOutput("ObjectImages");
		_ = imagesRoute.MapGet(Routes.ImageId, GetObjectImageAsync).CacheOutput("ObjectImages");
	}

	async Task<IResult> CreateDatAsync([FromBody] DtoObjectPost request, [FromServices] IObjectQueryService query, CancellationToken ct)
	{
		try
		{
			var result = await query.UploadDatAsync(request, ct);
			return result.Success ? Results.Created($"{Routes.Prefix}{BaseRoute}/{result.Descriptor!.Id}", result.Descriptor) : Results.Problem(result.ErrorMessage, statusCode: result.StatusCode);
		}
		catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueConstraintViolation(ex))
		{
			// A concurrent upload of the same content can slip past the pre-insert duplicate check and
			// be rejected by the unique DatObjects.xxHash3 index instead. Return the same "already
			// exists" response the non-racing path uses rather than letting it surface as a 500.
			return Results.Problem("Object with identical content already exists.", statusCode: StatusCodes.Status202Accepted);
		}
	}

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct, [FromQuery] bool includeDatBytes = true)
	{
		logger.LogDebug("[Read] Object {ObjectId}", id);
		var d = await query.GetByIdAsync(id, includeDatBytes, ct);
		return d != null ? Results.Ok(d) : Results.NotFound();
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoObjectPostResponse request, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		logger.LogDebug("[Update] Object {ObjectId}", id);
		var result = await query.UpdateAsync(id, request, ct);

		return result.Outcome switch
		{
			ObjectUpdateOutcome.Updated => Results.Ok(result.Descriptor),
			ObjectUpdateOutcome.NotFound => Results.NotFound(),
			ObjectUpdateOutcome.Forbidden => Results.Problem(result.ErrorMessage, statusCode: StatusCodes.Status403Forbidden),
			ObjectUpdateOutcome.InvalidRequest => Results.Problem(result.ErrorMessage, statusCode: StatusCodes.Status400BadRequest),
			ObjectUpdateOutcome.NameConflict => Results.Problem(result.ErrorMessage, statusCode: StatusCodes.Status409Conflict),
			_ => Results.Problem("Unexpected update outcome.", statusCode: StatusCodes.Status500InternalServerError),
		};
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		var result = await query.DeleteObjectAsync(id, ct);

		if (result.Outcome is ObjectDeleteOutcome.Removed)
		{
			logger.LogInformation("[Delete] Object {ObjectId} removed; {FileCount} file(s) parked under Removed", id, result.RemovedFiles?.Count ?? 0);
		}

		return result.Outcome switch
		{
			ObjectDeleteOutcome.Removed => Results.Ok(),
			ObjectDeleteOutcome.NotFound => Results.NotFound(),
			ObjectDeleteOutcome.Forbidden => Results.Problem(result.ErrorMessage, statusCode: StatusCodes.Status403Forbidden),
			_ => Results.Problem("Unexpected delete outcome.", statusCode: StatusCodes.Status500InternalServerError),
		};
	}

	async Task<IResult> ListAsync([FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		logger.LogDebug("[List] Objects");
		return Results.Ok(await query.ListAsync(ct));
	}

	async Task<IResult> ListMineAsync(HttpContext context, [FromServices] IObjectQueryService query, CancellationToken ct)
	{
		var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (string.IsNullOrEmpty(userIdClaim) || !ulong.TryParse(userIdClaim, out var userId))
		{
			return Results.Unauthorized();
		}

		return Results.Ok(await query.ListMineAsync(userId, ct));
	}

	async Task<IResult> GetObjectImagesAsync(HttpContext context, [FromRoute] UniqueObjectId id, [FromServices] IObjectImageService images, CancellationToken ct)
	{
		var result = await images.GetImagesZipAsync(id, ct);
		return result.Outcome switch
		{
			ObjectResourceOutcome.Ok => ServeBytesWithCache(context, result.Bytes!, result.Version, "application/zip", $"{id}_images.zip", "zip"),
			ObjectResourceOutcome.Forbidden => Results.Forbid(),
			_ => Results.NotFound(),
		};
	}

	async Task<IResult> GetObjectImageAsync(HttpContext context, [FromRoute] UniqueObjectId id, [FromRoute] int imageId, [FromServices] IObjectImageService images, CancellationToken ct)
	{
		var result = await images.GetImagePngAsync(id, imageId, ct);
		return result.Outcome switch
		{
			ObjectResourceOutcome.Ok => ServeBytesWithCache(context, result.Bytes!, result.Version, "image/png", null, imageId.ToString()),
			ObjectResourceOutcome.Forbidden => Results.Forbid(),
			_ => Results.NotFound(),
		};
	}

	async Task<IResult> GetObjectImageMetadataAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectImageService images, CancellationToken ct)
	{
		var result = await images.GetImageMetadataAsync(id, ct);
		return result.Outcome switch
		{
			ObjectResourceOutcome.Ok => Results.Ok(result.Metadata),
			ObjectResourceOutcome.Forbidden => Results.Forbid(),
			_ => Results.NotFound(),
		};
	}

	async Task<IResult> GetObjectFileAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectQueryService query, CancellationToken ct)
	{
		var result = await query.GetFilePathAsync(id, ct);
		return result.Outcome switch
		{
			ObjectResourceOutcome.Ok when result.FilePath is not null
				=> Results.File(result.FilePath, "application/octet-stream", Path.GetFileName(result.FilePath)),
			ObjectResourceOutcome.Forbidden => Results.Forbid(),
			_ => Results.NotFound(),
		};
	}

	/// <summary>
	/// Writes immutable binary content with a long-lived cache policy and an ETag derived from the content
	/// version (the source DAT file's xxHash3), so a browser never re-requests an unchanged object image.
	/// The same version always produces the same bytes, so the entry is safe to cache forever - a different
	/// file (or an edited object) has a different hash and therefore a different ETag.
	/// </summary>
	static IResult ServeBytesWithCache(HttpContext context, byte[] bytes, ulong version, string contentType, string? fileName, string variant)
	{
		var etag = $"\"{version:x16}-{variant}\"";
		if (context.Request.Headers.IfNoneMatch == etag)
		{
			return Results.StatusCode(StatusCodes.Status304NotModified);
		}

		context.Response.Headers.ETag = etag;
		context.Response.Headers.CacheControl = "public, max-age=31536000, immutable";
		return fileName is null ? Results.File(bytes, contentType) : Results.File(bytes, contentType, fileName);
	}
}
