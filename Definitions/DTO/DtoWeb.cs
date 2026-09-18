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
	ICollection<DtoItemRef> SC5Files,
	ICollection<DtoItemRef> SC5FilePacks) : IHasId;

/// <summary>Tag plus the entities that reference it.</summary>
public record DtoTagDescriptor(
	UniqueObjectId Id,
	string Name,
	ICollection<DtoItemRef> Objects,
	ICollection<DtoItemRef> ObjectPacks,
	ICollection<DtoItemRef> SC5Files,
	ICollection<DtoItemRef> SC5FilePacks) : IHasId;

/// <summary>Licence plus the entities that use it.</summary>
public record DtoLicenceDescriptor(
	UniqueObjectId Id,
	string Name,
	string Text,
	ICollection<DtoItemRef> Objects,
	ICollection<DtoItemRef> ObjectPacks,
	ICollection<DtoItemRef> SC5Files,
	ICollection<DtoItemRef> SC5FilePacks) : IHasId;

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
public record DtoSC5FilePackDescriptor(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	ICollection<DtoAuthorEntry> Authors,
	ICollection<DtoTagEntry> Tags,
	ICollection<DtoItemRef> SC5Files) : IHasId, IDbDates;

/// <summary>A single scenario (SC5) file with its related metadata.</summary>
public record DtoSC5FileDescriptor(
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
	ICollection<DtoItemRef> SC5FilePacks) : IHasId, IDbDates;

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
public record DtoSC5FileListEntry(
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
public record DtoSC5FilePackListEntry(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	int AuthorCount,
	int TagCount,
	int FileCount) : IHasId;
