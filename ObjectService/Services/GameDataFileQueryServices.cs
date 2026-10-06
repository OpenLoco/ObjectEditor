using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Microsoft.EntityFrameworkCore;
using ObjectService.RouteHandlers;

namespace ObjectService.Services;

/// <summary>
/// Read/update/delete queries for one game-data file entity type, backing that entity's public API
/// routes. Each entity (music, sound effects, tutorials, graphics) has its own service.
/// </summary>
public interface IGameDataFileQueryService<TListEntry, TDescriptor>
	where TDescriptor : class, IGameFileDescriptor
{
	Task<IEnumerable<TListEntry>> ListEntriesAsync(CancellationToken ct);
	Task<TDescriptor?> GetDescriptorAsync(UniqueObjectId id, CancellationToken ct);
	Task<TDescriptor?> UpdateAsync(UniqueObjectId id, TDescriptor request, CancellationToken ct);
	Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct);
	Task<string?> GetFilePathAsync(UniqueObjectId id, CancellationToken ct);
}

/// <summary>
/// Shared implementation for the game-data file query services. The entity, list DTO and descriptor
/// DTO are all supplied by the concrete service, so the behaviour is identical while each entity
/// keeps its own service, DTOs and route.
/// </summary>
public abstract class GameDataFileQueryService<TEntity, TListEntry, TDescriptor> : IGameDataFileQueryService<TListEntry, TDescriptor>
	where TEntity : DbFileObject
	where TDescriptor : class, IGameFileDescriptor
{
	private readonly LocoDbContext _db;
	private readonly string _folder;

	protected GameDataFileQueryService(LocoDbContext db, string folder)
	{
		_db = db;
		_folder = folder;
	}

	protected LocoDbContext Db => _db;

	/// <summary>The table for this entity type.</summary>
	protected abstract DbSet<TEntity> Set { get; }

	protected abstract TListEntry ToListEntry(TEntity entity);

	protected abstract TDescriptor ToDescriptor(TEntity entity);

	public async Task<IEnumerable<TListEntry>> ListEntriesAsync(CancellationToken ct)
	{
		var files = await Query()
			.OrderBy(f => f.Name)
			.AsSplitQuery()
			.ToListAsync(ct)
			.ConfigureAwait(false);

		return files.Select(ToListEntry).ToArray();
	}

	public async Task<TDescriptor?> GetDescriptorAsync(UniqueObjectId id, CancellationToken ct)
	{
		var file = await Query()
			.Where(f => f.Id == id)
			.AsSplitQuery()
			.FirstOrDefaultAsync(ct)
			.ConfigureAwait(false);

		return file is null ? default : ToDescriptor(file);
	}

	private IQueryable<TEntity> Query()
		=> Set
			.Include(f => f.Licence)
			.Include(f => f.Authors)
			.Include(f => f.Tags);

	public async Task<TDescriptor?> UpdateAsync(UniqueObjectId id, TDescriptor request, CancellationToken ct)
	{
		var file = await Query()
			.Where(f => f.Id == id)
			.AsSplitQuery()
			.FirstOrDefaultAsync(ct)
			.ConfigureAwait(false);

		if (file is null)
		{
			return default;
		}

		file.Name = request.Name;
		file.Description = request.Description;
		file.ObjectSource = request.ObjectSource;
		file.CreatedDate = request.CreatedDate;
		file.ModifiedDate = request.ModifiedDate;

		file.Licence = request.Licence is null
			? null
			: await _db.Licences.FindAsync([request.Licence.Id], ct).ConfigureAwait(false);

		file.Authors.Clear();
		var authorIds = request.Authors.Select(a => a.Id).ToList();
		if (authorIds.Count > 0)
		{
			foreach (var author in await _db.Authors.Where(a => authorIds.Contains(a.Id)).ToListAsync(ct).ConfigureAwait(false))
			{
				file.Authors.Add(author);
			}
		}

		file.Tags.Clear();
		var tagIds = request.Tags.Select(t => t.Id).ToList();
		if (tagIds.Count > 0)
		{
			foreach (var tag in await _db.Tags.Where(t => tagIds.Contains(t.Id)).ToListAsync(ct).ConfigureAwait(false))
			{
				file.Tags.Add(tag);
			}
		}

		_ = await _db.SaveChangesAsync(ct).ConfigureAwait(false);
		return ToDescriptor(file);
	}

	public async Task<bool> DeleteAsync(UniqueObjectId id, CancellationToken ct)
	{
		var file = await Set.FindAsync([id], ct).ConfigureAwait(false);
		if (file is null)
		{
			return false;
		}

		// Park the file under the category's Removed folder so a file that is still on disk is not
		// silently re-imported by the next reconciliation. The database is the source of truth, so the
		// row is kept and marked unavailable rather than deleted - curated metadata and any references
		// survive, and the row is restored if the file is re-added. The file is located regardless of
		// availability so a non-downloadable file can still be parked.
		var path = await ResolveFilePathAsync(id, ct).ConfigureAwait(false);
		if (path != null)
		{
			_ = ServerFolderManager.MoveToRemovedFolder(_folder, path);
		}

		file.Availability = ObjectAvailability.Unavailable;
		_ = await _db.SaveChangesAsync(ct).ConfigureAwait(false);
		return true;
	}

	/// <summary>
	/// The on-disk path to serve for a file download, or <see langword="null"/> when the entity is not
	/// downloadable (only available <see cref="ObjectSource.Custom"/> content is ever served).
	/// </summary>
	public async Task<string?> GetFilePathAsync(UniqueObjectId id, CancellationToken ct)
	{
		var (path, downloadable) = await ResolveFileAsync(id, ct).ConfigureAwait(false);
		return downloadable ? path : null;
	}

	/// <summary>Resolves the on-disk path regardless of downloadability (used when parking a removed file).</summary>
	private async Task<string?> ResolveFilePathAsync(UniqueObjectId id, CancellationToken ct)
		=> (await ResolveFileAsync(id, ct).ConfigureAwait(false)).Path;

	private async Task<(string? Path, bool Downloadable)> ResolveFileAsync(UniqueObjectId id, CancellationToken ct)
	{
		var file = await Set
			.AsNoTracking()
			.Where(f => f.Id == id)
			.Select(f => new { f.Name, f.ObjectSource, f.Availability })
			.FirstOrDefaultAsync(ct)
			.ConfigureAwait(false);

		if (file is null
			|| string.IsNullOrWhiteSpace(file.Name)
			|| !RouteHelpers.TryGetSafeRelativePathUnderRoot(_folder, file.Name, out var fullPath, out _)
			|| !File.Exists(fullPath))
		{
			return (null, false);
		}

		return (fullPath, ObjectAvailabilityRules.IsDownloadable(file.ObjectSource, file.Availability));
	}
}

/// <summary>Read/update/delete queries for music files.</summary>
public interface IMusicService : IGameDataFileQueryService<DtoMusicListEntry, DtoMusicDescriptor>
{
}

public sealed class MusicService(LocoDbContext db, ServerFolderManager sfm)
	: GameDataFileQueryService<TblMusic, DtoMusicListEntry, DtoMusicDescriptor>(db, sfm.MusicFolder), IMusicService
{
	protected override DbSet<TblMusic> Set => Db.Music;

	protected override DtoMusicListEntry ToListEntry(TblMusic entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.UploadedDate, entity.ObjectSource, entity.Availability, entity.Licence?.ToDtoEntry(), entity.Authors.Count, entity.Tags.Count);

	protected override DtoMusicDescriptor ToDescriptor(TblMusic entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.ObjectSource, entity.Availability, entity.CreatedDate, entity.ModifiedDate, entity.UploadedDate, entity.Licence?.ToDtoEntry(), [.. entity.Authors.OrderBy(a => a.Name).Select(a => a.ToDtoEntry())], [.. entity.Tags.OrderBy(t => t.Name).Select(t => t.ToDtoEntry())]);
}

/// <summary>Read/update/delete queries for sound effects.</summary>
public interface ISoundEffectsService : IGameDataFileQueryService<DtoSoundEffectListEntry, DtoSoundEffectDescriptor>
{
}

public sealed class SoundEffectsService(LocoDbContext db, ServerFolderManager sfm)
	: GameDataFileQueryService<TblSoundEffect, DtoSoundEffectListEntry, DtoSoundEffectDescriptor>(db, sfm.SoundEffectsFolder), ISoundEffectsService
{
	protected override DbSet<TblSoundEffect> Set => Db.SoundEffects;

	protected override DtoSoundEffectListEntry ToListEntry(TblSoundEffect entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.UploadedDate, entity.ObjectSource, entity.Availability, entity.Licence?.ToDtoEntry(), entity.Authors.Count, entity.Tags.Count);

	protected override DtoSoundEffectDescriptor ToDescriptor(TblSoundEffect entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.ObjectSource, entity.Availability, entity.CreatedDate, entity.ModifiedDate, entity.UploadedDate, entity.Licence?.ToDtoEntry(), [.. entity.Authors.OrderBy(a => a.Name).Select(a => a.ToDtoEntry())], [.. entity.Tags.OrderBy(t => t.Name).Select(t => t.ToDtoEntry())]);
}

/// <summary>Read/update/delete queries for tutorials.</summary>
public interface ITutorialsService : IGameDataFileQueryService<DtoTutorialListEntry, DtoTutorialDescriptor>
{
}

public sealed class TutorialsService(LocoDbContext db, ServerFolderManager sfm)
	: GameDataFileQueryService<TblTutorial, DtoTutorialListEntry, DtoTutorialDescriptor>(db, sfm.TutorialsFolder), ITutorialsService
{
	protected override DbSet<TblTutorial> Set => Db.Tutorials;

	protected override DtoTutorialListEntry ToListEntry(TblTutorial entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.UploadedDate, entity.ObjectSource, entity.Availability, entity.Licence?.ToDtoEntry(), entity.Authors.Count, entity.Tags.Count);

	protected override DtoTutorialDescriptor ToDescriptor(TblTutorial entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.ObjectSource, entity.Availability, entity.CreatedDate, entity.ModifiedDate, entity.UploadedDate, entity.Licence?.ToDtoEntry(), [.. entity.Authors.OrderBy(a => a.Name).Select(a => a.ToDtoEntry())], [.. entity.Tags.OrderBy(t => t.Name).Select(t => t.ToDtoEntry())]);
}

/// <summary>Read/update/delete queries for graphics files.</summary>
public interface IGraphicsService : IGameDataFileQueryService<DtoGraphicsListEntry, DtoGraphicsDescriptor>
{
}

public sealed class GraphicsService(LocoDbContext db, ServerFolderManager sfm)
	: GameDataFileQueryService<TblGraphics, DtoGraphicsListEntry, DtoGraphicsDescriptor>(db, sfm.GraphicsFolder), IGraphicsService
{
	protected override DbSet<TblGraphics> Set => Db.Graphics;

	protected override DtoGraphicsListEntry ToListEntry(TblGraphics entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.UploadedDate, entity.ObjectSource, entity.Availability, entity.Licence?.ToDtoEntry(), entity.Authors.Count, entity.Tags.Count);

	protected override DtoGraphicsDescriptor ToDescriptor(TblGraphics entity)
		=> new(entity.Id, entity.Name, entity.Description, entity.ObjectSource, entity.Availability, entity.CreatedDate, entity.ModifiedDate, entity.UploadedDate, entity.Licence?.ToDtoEntry(), [.. entity.Authors.OrderBy(a => a.Name).Select(a => a.ToDtoEntry())], [.. entity.Tags.OrderBy(t => t.Name).Select(t => t.ToDtoEntry())]);
}

