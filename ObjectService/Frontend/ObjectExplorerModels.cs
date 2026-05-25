using Definitions;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Definitions.Web;

namespace ObjectService.Frontend;

public sealed record ObjectBrowseQuery(
	string? Search,
	ObjectType? ObjectType,
	ObjectSource? ObjectSource,
	ObjectAvailability? Availability,
	int Page = 1,
	int PageSize = 48);

public sealed record ObjectBrowsePageViewModel(
	int TotalCount,
	int FilteredCount,
	int Page,
	int PageSize,
	IReadOnlyList<ObjectListItemViewModel> Items)
{
	public int TotalPages => Math.Max(1, (int)Math.Ceiling(FilteredCount / (double)PageSize));

	public bool HasPreviousPage => Page > 1;

	public bool HasNextPage => Page < TotalPages;
}

public sealed record ObjectListItemViewModel(
	UniqueObjectId Id,
	string InternalName,
	string? DisplayName,
	string? DatName,
	uint? DatChecksum,
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	VehicleType? VehicleType,
	ObjectAvailability Availability,
	DateOnly UploadedDate,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	bool CanDownload)
{
	public string ResolvedTitle => string.IsNullOrWhiteSpace(DisplayName) ? DatName ?? InternalName : DisplayName;

	public string ApiUrl => $"{RoutesV2.Prefix}{RoutesV2.Objects}/{Id}";

	public string DownloadUrl => $"{ApiUrl}{RoutesV2.File}";
}

public sealed record ObjectDetailViewModel(
	UniqueObjectId Id,
	string InternalName,
	string DisplayName,
	string? PrimaryDatName,
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	VehicleType? VehicleType,
	ObjectAvailability Availability,
	string? Description,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	DateOnly UploadedDate,
	string? LicenceName,
	string? LicenceText,
	IReadOnlyList<string> Authors,
	IReadOnlyList<string> Tags,
	IReadOnlyList<string> ObjectPacks,
	IReadOnlyList<ObjectFileEntryViewModel> Files,
	IReadOnlyList<StringTableGroupViewModel> StringTableGroups,
	IReadOnlyList<ObjectImageViewModel> Images,
	string? ImageTableMessage)
{
	public string ApiUrl => $"{RoutesV2.Prefix}{RoutesV2.Objects}/{Id}";

	public string DownloadUrl => $"{ApiUrl}{RoutesV2.File}";

	public bool CanDownloadAnyFile => Files.Any(x => x.CanDownload);

	public bool HasImages => Images.Count > 0;
}

public sealed record ObjectImageViewModel(
	int Index,
	int Width,
	int Height,
	string DataUrl);

public sealed record ObjectFileEntryViewModel(
	string DatName,
	uint DatChecksum,
	ulong xxHash3,
	bool CanDownload,
	string StatusText);

public sealed record StringTableGroupViewModel(
	string Key,
	IReadOnlyList<StringTableTranslationViewModel> Values);

public sealed record StringTableTranslationViewModel(
	LanguageId Language,
	string Text);
