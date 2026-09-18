using Definitions;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
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

		var imagesRoute = resourceRoute.MapGroup(Routes.Images);
		_ = imagesRoute.MapGet(Routes.ImageId, GetObjectImageAsync);
	}

	async Task<IResult> CreateDatAsync([FromBody] DtoObjectPost request, [FromServices] IObjectUploadService upload, CancellationToken ct)
	{
		var result = await upload.UploadDatAsync(request, ct);
		return result.Success ? Results.Created($"{BaseRoute}/{result.Descriptor!.Id}", result.Descriptor) : Results.Problem(result.ErrorMessage, statusCode: result.StatusCode);
	}

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		logger.LogInformation("[Read] Object {ObjectId}", id);
		var d = await query.GetByIdAsync(id, ct);
		return d != null ? Results.Ok(d) : Results.NotFound();
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] DtoObjectPostResponse request, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		logger.LogInformation("[Update] Object {ObjectId}", id);
		var r = await query.UpdateAsync(id, request, ct);
		return r != null ? Results.Ok(r) : Results.NotFound();
	}

	async Task<IResult> DeleteAsync([FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		logger.LogInformation("[Delete] Not implemented");
		return Results.Problem(statusCode: StatusCodes.Status501NotImplemented);
	}

	async Task<IResult> ListAsync(HttpContext context, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		logger.LogInformation("[List] Objects");
		return Results.Ok(await query.ListAsync(context, ct));
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

	async Task<IResult> GetObjectImagesAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		var descriptor = await query.GetByIdAsync(id, ct);
		if (descriptor == null)
		{
			return Results.NotFound();
		}

		if (descriptor.Availability == ObjectAvailability.Unavailable || descriptor.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			logger.LogWarning("Object {ObjectId} cannot expose images due to source/availability restrictions", id);
			return Results.Forbid();
		}

		var zip = await query.GetImagesZipAsync(id, ct);
		return zip != null ? Results.File(zip, "application/zip", $"{id}_images.zip") : Results.NotFound();
	}

	async Task<IResult> GetObjectImageAsync([FromRoute] UniqueObjectId id, [FromRoute] int imageId, [FromServices] IObjectQueryService query, [FromServices] ILogger<ObjectRouteHandler> logger, CancellationToken ct)
	{
		var descriptor = await query.GetByIdAsync(id, ct);
		if (descriptor == null)
		{
			return Results.NotFound();
		}

		if (descriptor.Availability == ObjectAvailability.Unavailable || descriptor.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			logger.LogWarning("Object {ObjectId} cannot expose images due to source/availability restrictions", id);
			return Results.Forbid();
		}

		var png = await query.GetImagePngAsync(id, imageId, ct);
		return png != null ? Results.File(png, "image/png") : Results.NotFound();
	}

	async Task<IResult> GetObjectFileAsync([FromRoute] UniqueObjectId id, [FromServices] IObjectQueryService query, CancellationToken ct)
	{
		var descriptor = await query.GetByIdAsync(id, ct);
		if (descriptor == null)
		{
			return Results.NotFound();
		}

		if (descriptor.Availability == ObjectAvailability.Unavailable || descriptor.ObjectSource is ObjectSource.LocomotionGoG or ObjectSource.LocomotionSteam)
		{
			return Results.Forbid();
		}

		var path = await query.GetFilePathAsync(id, ct);
		return path != null && File.Exists(path) ? Results.File(path, "application/octet-stream", Path.GetFileName(path)) : Results.NotFound();
	}
}
