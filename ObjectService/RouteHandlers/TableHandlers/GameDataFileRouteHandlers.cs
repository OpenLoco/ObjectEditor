using Definitions.DTO;
using Definitions.Web;
using Microsoft.AspNetCore.Mvc;
using ObjectService.Services;

namespace ObjectService.RouteHandlers.TableHandlers;

/// <summary>
/// Shared route handler for the game-data file entities (music, sound effects, tutorials, graphics).
/// Mirrors <see cref="ScenarioRouteHandler"/> but without the pack relationship.
/// </summary>
public abstract class GameDataFileRouteHandler<TService, TListEntry, TDescriptor> : ITableRouteHandler
	where TService : IGameDataFileQueryService<TListEntry, TDescriptor>
	where TDescriptor : class, IGameFileDescriptor
{
	public abstract string BaseRoute { get; }

	public Delegate ListDelegate => ListAsync;
	public Delegate CreateDelegate => CreateAsync;
	public Delegate ReadDelegate => ReadAsync;
	public Delegate UpdateDelegate => UpdateAsync;
	public Delegate DeleteDelegate => DeleteAsync;

	public void MapRoutes(IEndpointRouteBuilder e)
		=> BaseTableRouteHandler.MapRoutes(this, e, e.ServiceProvider.GetRequiredService<IConfiguration>());

	public void MapAdditionalRoutes(IEndpointRouteBuilder p)
		=> p.MapGroup(Routes.ResourceRoute).MapGet(Routes.File, GetFileAsync);

	async Task<IResult> ListAsync([FromServices] TService svc, CancellationToken ct)
		=> Results.Ok(await svc.ListEntriesAsync(ct));

	Task<IResult> CreateAsync()
		=> Task.FromResult(Results.Problem(statusCode: StatusCodes.Status501NotImplemented));

	async Task<IResult> ReadAsync([FromRoute] UniqueObjectId id, [FromServices] TService svc, CancellationToken ct)
	{
		var descriptor = await svc.GetDescriptorAsync(id, ct);
		return descriptor != null ? Results.Ok(descriptor) : Results.NotFound();
	}

	async Task<IResult> GetFileAsync([FromRoute] UniqueObjectId id, [FromServices] TService svc, CancellationToken ct)
	{
		var path = await svc.GetFilePathAsync(id, ct);
		return path != null ? Results.File(path, "application/octet-stream", Path.GetFileName(path)) : Results.NotFound();
	}

	async Task<IResult> UpdateAsync([FromRoute] UniqueObjectId id, [FromBody] TDescriptor request, [FromServices] TService svc, CancellationToken ct)
	{
		var updated = await svc.UpdateAsync(id, request, ct);
		return updated != null ? Results.Ok(updated) : Results.NotFound();
	}

	async Task<IResult> DeleteAsync([FromRoute] UniqueObjectId id, [FromServices] TService svc, CancellationToken ct)
	{
		var deleted = await svc.DeleteAsync(id, ct);
		return deleted ? Results.Ok() : Results.NotFound();
	}
}

/// <summary>Routes for music files (<c>/v2/music</c>).</summary>
public sealed class MusicRouteHandler : GameDataFileRouteHandler<IMusicService, DtoMusicListEntry, DtoMusicDescriptor>
{
	public override string BaseRoute => Routes.Music;
}

/// <summary>Routes for sound effects (<c>/v2/soundeffects</c>).</summary>
public sealed class SoundEffectsRouteHandler : GameDataFileRouteHandler<ISoundEffectsService, DtoSoundEffectListEntry, DtoSoundEffectDescriptor>
{
	public override string BaseRoute => Routes.SoundEffects;
}

/// <summary>Routes for tutorials (<c>/v2/tutorials</c>).</summary>
public sealed class TutorialsRouteHandler : GameDataFileRouteHandler<ITutorialsService, DtoTutorialListEntry, DtoTutorialDescriptor>
{
	public override string BaseRoute => Routes.Tutorials;
}

/// <summary>Routes for graphics files (<c>/v2/graphics</c>).</summary>
public sealed class GraphicsRouteHandler : GameDataFileRouteHandler<IGraphicsService, DtoGraphicsListEntry, DtoGraphicsDescriptor>
{
	public override string BaseRoute => Routes.Graphics;
}
