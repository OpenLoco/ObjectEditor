using Definitions.Database;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

/// <summary>
/// A lightweight <c>id + name</c> reference to another entity, used by the descriptor
/// DTOs to describe relationships without duplicating the full entity payload.
/// </summary>
public record DtoItemRef(
	UniqueObjectId Id,
	string Name) : IHasId;

/// <summary>Author plus the entities that reference them.</summary>
public record DtoAuthorDescriptor(
	UniqueObjectId Id,
	string Name,
	ICollection<DtoItemRef> Objects,
	ICollection<DtoItemRef> ObjectPacks,
	ICollection<DtoItemRef> Scenarios,
	ICollection<DtoItemRef> ScenarioPacks) : IHasId;

/// <summary>Tag plus the entities that reference it.</summary>
public record DtoTagDescriptor(
	UniqueObjectId Id,
	string Name,
	ICollection<DtoItemRef> Objects,
	ICollection<DtoItemRef> ObjectPacks,
	ICollection<DtoItemRef> Scenarios,
	ICollection<DtoItemRef> ScenarioPacks) : IHasId;

/// <summary>Licence plus the entities that use it.</summary>
public record DtoLicenceDescriptor(
	UniqueObjectId Id,
	string Name,
	string Text,
	ICollection<DtoItemRef> Objects,
	ICollection<DtoItemRef> ObjectPacks,
	ICollection<DtoItemRef> Scenarios,
	ICollection<DtoItemRef> ScenarioPacks) : IHasId;

/// <summary>A single object pack with its related metadata and objects.</summary>
public record DtoObjectPackDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags,
	ICollection<DtoItemRef> Objects) : IHasId, IDbDates;

/// <summary>A single scenario (SC5) pack with its related metadata and scenario files.</summary>
public record DtoScenarioPackDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags,
	ICollection<DtoItemRef> Scenarios) : IHasId, IDbDates;

/// <summary>A single scenario (SC5) file with its related metadata.</summary>
public record DtoScenarioDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags,
	ICollection<DtoItemRef> ScenarioPacks) : IHasId, IDbDates;

/// <summary>Row shape for the object pack list view, including relationship counts.</summary>
public record DtoObjectPackListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount,
	int ObjectCount) : IHasId;

/// <summary>Row shape for the scenario (SC5) file list view, including relationship counts.</summary>
public record DtoScenarioListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	ObjectSource ObjectSource,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount,
	int PackCount) : IHasId;

/// <summary>Row shape for the scenario (SC5) file pack list view, including relationship counts.</summary>
public record DtoScenarioPackListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount,
	int FileCount) : IHasId;

/// <summary>
/// Common shape of a game-data file descriptor (a music, sound effect, tutorial or graphics file).
/// Implemented by the per-entity descriptor records so their shared query logic can populate them.
/// </summary>
public interface IGameFileDescriptor : IHasId
{
	string Name { get; }
	string? Description { get; }
	ObjectSource ObjectSource { get; }
	DateOnly? CreatedDate { get; }
	DateOnly? ModifiedDate { get; }
	DtoLicenceEntry? Licence { get; }
	ICollection<DtoAuthorEntry> Authors { get; }
	ICollection<DtoTagEntry> Tags { get; }
}

/// <summary>A single music file with its related metadata.</summary>
public record DtoMusicDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags) : IHasId, IDbDates, IGameFileDescriptor;

/// <summary>Row shape for the music list view, including relationship counts.</summary>
public record DtoMusicListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	ObjectSource ObjectSource,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount) : IHasId;

/// <summary>A single sound effect file with its related metadata.</summary>
public record DtoSoundEffectDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags) : IHasId, IDbDates, IGameFileDescriptor;

/// <summary>Row shape for the sound effect list view, including relationship counts.</summary>
public record DtoSoundEffectListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	ObjectSource ObjectSource,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount) : IHasId;


/// <summary>A single tutorial file with its related metadata.</summary>
public record DtoTutorialDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags) : IHasId, IDbDates, IGameFileDescriptor;

/// <summary>Row shape for the tutorial list view, including relationship counts.</summary>
public record DtoTutorialListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	ObjectSource ObjectSource,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount) : IHasId;

/// <summary>A single graphics file with its related metadata.</summary>
public record DtoGraphicsDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags) : IHasId, IDbDates, IGameFileDescriptor;

/// <summary>Row shape for the graphics file list view, including relationship counts.</summary>
public record DtoGraphicsListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	ObjectSource ObjectSource,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount) : IHasId;

