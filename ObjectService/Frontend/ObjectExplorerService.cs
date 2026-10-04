using Definitions;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using System.Text.Json;

namespace ObjectService.Frontend;

public sealed class ObjectExplorerService
{
	static readonly JsonSerializerOptions s_subObjectJsonOptions = new() { WriteIndented = true };

	readonly FrontendApiClient _apiClient;

	public ObjectExplorerService(FrontendApiClient apiClient)
	{
		_apiClient = apiClient;
	}

	public async Task<ObjectBrowsePageViewModel> GetObjectsAsync(ObjectBrowseQuery request, CancellationToken cancellationToken = default)
	{
		using var client = _apiClient.CreateClient();
		var pageSize = Math.Clamp(request.PageSize, 12, 100);
		var requestedPage = Math.Max(request.Page, 1);
		var search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();
		var objects = (await Client.GetObjectListAsync(client, cancellationToken: cancellationToken)).ToList();

		var totalCount = objects.Count;
		IEnumerable<DtoObjectEntry> query = objects;

		if (request.ObjectType.HasValue)
		{
			query = query.Where(x => x.ObjectType == request.ObjectType.Value);
		}

		if (request.ObjectSource.HasValue)
		{
			query = query.Where(x => x.ObjectSource == request.ObjectSource.Value);
		}

		if (request.CanDownload.HasValue)
		{
			query = query.Where(x => IsDownloadable(x) == request.CanDownload.Value);
		}

		if (request.VehicleType.HasValue)
		{
			query = query.Where(x => x.VehicleType == request.VehicleType.Value);
		}

		if (search != null)
		{
			query = query.Where(x =>
				ContainsInsensitive(x.InternalName, search)
				|| ContainsInsensitive(x.DisplayName, search)
				|| ContainsInsensitive(x.Description, search));
		}

		var filtered = query.ToList();
		var filteredCount = filtered.Count;
		var totalPages = Math.Max(1, (int)Math.Ceiling(filteredCount / (double)pageSize));
		var page = Math.Min(requestedPage, totalPages);

		var rows = filtered
			.OrderByDescending(x => IsDownloadable(x))
			.ThenByDescending(x => x.UploadedDate)
			.ThenBy(x => x.InternalName)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		var items = rows.Select(MapBrowseItem).ToList();
		return new ObjectBrowsePageViewModel(totalCount, filteredCount, page, pageSize, items);
	}

	public async Task<ObjectDetailViewModel?> GetObjectAsync(UniqueObjectId id, CancellationToken cancellationToken = default)
	{
		using var client = _apiClient.CreateClient();
		// The web frontend only needs the descriptor metadata, never the base64-inlined DAT bytes.
		var obj = await Client.GetObjectAsync(client, id, cancellationToken: cancellationToken, includeDatBytes: false);

		if (obj == null)
		{
			return null;
		}

		var primaryDatName = obj.DatObjects
			.OrderBy(x => x.DatName)
			.ThenBy(x => x.DatChecksum)
			.Select(x => x.DatName)
			.FirstOrDefault();

		var files = obj.DatObjects
			.OrderBy(x => x.DatName)
			.ThenBy(x => x.DatChecksum)
			.Select(dat => BuildFileEntry(obj, dat))
			.ToList();

		var stringTableGroups = obj.StringTable.Table
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.Select(group => new StringTableGroupViewModel(
				group.Key,
				group.Value
					.OrderBy(x => LanguagePriority(x.Key))
					.ThenBy(x => x.Key)
					.Select(x => new StringTableTranslationViewModel(x.Key, x.Value))
					.ToList()))
			.ToList();

		var images = await GetImagesFromApiAsync(client, id, cancellationToken);
		var imageTableMessage = images.Count > 0
			? null
			: IsDownloadAllowed(obj.ObjectSource, obj.Availability)
				? "No renderable images were returned by the public API for this object."
				: "Images are not available for vanilla or unavailable objects.";

		// The full object-specific property data (the sub-object) is returned by the public API as part of
		// the descriptor. The frontend only formats it for display; it never reads DAT files itself.
		var subObjectJson = obj.SubObject is null
			? null
			: JsonSerializer.Serialize(obj.SubObject, s_subObjectJsonOptions);

		return new ObjectDetailViewModel(
			obj.Id,
			obj.Name,
			ResolveDisplayName(obj),
			primaryDatName,
			obj.ObjectType,
			obj.ObjectSource,
			obj.VehicleType,
			obj.Availability,
			obj.Description,
			obj.CreatedDate,
			obj.ModifiedDate,
			obj.UploadedDate,
			obj.Licence is not null ? new LicenceRefViewModel(obj.Licence.Id, obj.Licence.Name, obj.Licence.Text) : null,
			[.. obj.Authors.Select(x => new NamedRefViewModel(x.Id, x.Name)).OrderBy(x => x.Name)],
			[.. obj.Tags.Select(x => new NamedRefViewModel(x.Id, x.Name)).OrderBy(x => x.Name)],
			[.. obj.ObjectPacks.Select(x => x.Name).OrderBy(x => x)],
			files,
			stringTableGroups,
			images,
			imageTableMessage,
			subObjectJson);
	}

	ObjectListItemViewModel MapBrowseItem(DtoObjectEntry row)
	{
		return new ObjectListItemViewModel(
			row.Id,
			row.InternalName,
			null,
			row.DisplayName,
			row.DatChecksum,
			row.ObjectType,
			row.ObjectSource,
			row.VehicleType,
			row.Availability,
			row.UploadedDate,
			row.CreatedDate,
			row.ModifiedDate,
			IsDownloadable(row),
			row.Description);
	}

	ObjectFileEntryViewModel BuildFileEntry(DtoObjectPostResponse obj, DtoDatObjectEntry dat)
	{
		var canDownload = IsDownloadAllowed(obj.ObjectSource, obj.Availability);
		return new ObjectFileEntryViewModel(
			dat.DatName,
			dat.DatChecksum,
			dat.xxHash3,
			canDownload,
			canDownload ? "Available through public API" : "Unavailable through public API");
	}

	async Task<IReadOnlyList<ObjectImageViewModel>> GetImagesFromApiAsync(HttpClient client, UniqueObjectId id, CancellationToken cancellationToken)
	{
		// Only the lightweight metadata is fetched here; each frame is then rendered by the browser from its
		// own (immutable, cacheable) URL instead of being base64-inlined. Reading the metadata - rather than
		// unzipping and re-decoding every PNG - keeps the details page cheap.
		var metadata = await Client.GetObjectImageMetadataAsync(client, id, cancellationToken: cancellationToken);
		if (metadata == null || metadata.Frames.Count == 0)
		{
			return [];
		}

		var baseUrl = $"{Routes.Prefix}{Routes.Objects}/{id}{Routes.Images}";
		return [.. metadata.Frames
			.OrderBy(x => x.Index)
			.Select(frame => new ObjectImageViewModel(frame.Index, frame.Width, frame.Height, $"{baseUrl}/{frame.Index}"))];
	}

	static bool IsDownloadable(DtoObjectEntry row)
		=> row.Availability == ObjectAvailability.Available
			&& row.DatChecksum.HasValue
			&& IsDownloadAllowed(row.ObjectSource, row.Availability);

	static bool IsDownloadAllowed(ObjectSource objectSource, ObjectAvailability availability)
		=> availability != ObjectAvailability.Unavailable
			&& objectSource is not ObjectSource.LocomotionGoG
			&& objectSource is not ObjectSource.LocomotionSteam;

	static string ResolveDisplayName(DtoObjectPostResponse dto)
	{
		if (dto.StringTable.Table.TryGetValue("Name", out var nameRows)
			|| dto.StringTable.Table.TryGetValue("name", out nameRows))
		{
			var localisedName = nameRows
				.OrderBy(x => LanguagePriority(x.Key))
				.ThenBy(x => x.Key)
				.Select(x => x.Value)
				.FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

			if (!string.IsNullOrWhiteSpace(localisedName))
			{
				return localisedName;
			}
		}

		return dto.DisplayName ?? dto.Name;
	}

	static bool ContainsInsensitive(string? value, string search)
		=> !string.IsNullOrWhiteSpace(value)
			&& value.Contains(search, StringComparison.OrdinalIgnoreCase);

	static int LanguagePriority(LanguageId language)
		=> language switch
		{
			LanguageId.English_UK => 0,
			LanguageId.English_US => 1,
			_ => 2,
		};
}
