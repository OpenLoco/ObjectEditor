using Definitions.DTO;
using Definitions.ObjectModels.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gui.Models;

public interface IOnlineBrowseResultItem
{
	UniqueObjectId Id { get; }
}

public record OnlineItemPackBrowseResult(
	UniqueObjectId Id,
	string Name,
	string? Description,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	DtoLicenceEntry? Licence,
	IReadOnlyList<DtoAuthorEntry> Authors,
	IReadOnlyList<DtoTagEntry> Tags,
	IReadOnlyList<FileSystemItem> Items,
	OnlineApiEndpointGroup Group) : IOnlineBrowseResultItem
{
	public string ItemLabelSingular
		=> Group == OnlineApiEndpointGroup.ObjectPacks ? "Object" : "Scenario";

	public string ItemLabelPlural
		=> Group == OnlineApiEndpointGroup.ObjectPacks ? "Objects" : "Scenarios";

	public string ItemCountText
		=> Items.Count == 1
			? $"1 {ItemLabelSingular}"
			: $"{Items.Count} {ItemLabelPlural}";

	public string LicenceName
		=> Licence?.Name ?? "Unspecified";

	public bool HasDescription
		=> !string.IsNullOrWhiteSpace(Description);

	public bool HasCreatedDate
		=> CreatedDate != null;

	public bool HasModifiedDate
		=> ModifiedDate != null;

	public bool HasAuthors
		=> Authors.Count > 0;

	public bool HasTags
		=> Tags.Count > 0;

	public string AuthorsText
		=> string.Join(", ", Authors.Select(x => x.Name));

	public string TagsText
		=> string.Join(", ", Tags.Select(x => x.Name));
}

public record OnlineAuthorBrowseResult(
	UniqueObjectId Id,
	string Name) : IOnlineBrowseResultItem;

public record OnlineTagBrowseResult(
	UniqueObjectId Id,
	string Name) : IOnlineBrowseResultItem;

public record OnlineLicenceBrowseResult(
	UniqueObjectId Id,
	string Name,
	string Text) : IOnlineBrowseResultItem
{
	public bool HasText
		=> !string.IsNullOrWhiteSpace(Text);
}

public record OnlineMissingObjectBrowseResult(
	UniqueObjectId Id,
	string DatName,
	uint DatChecksum,
	ObjectType ObjectType) : IOnlineBrowseResultItem
{
	public string DatChecksumHex
		=> $"0x{DatChecksum:X8}";
}