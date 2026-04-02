using Definitions;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using SixLabors.ImageSharp;
using System.IO.Compression;

namespace ObjectService.Frontend;

public sealed class ObjectExplorerService
{
	readonly IHttpClientFactory _httpClientFactory;
	readonly IHttpContextAccessor _httpContextAccessor;

	public ObjectExplorerService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task<ObjectBrowsePageViewModel> GetObjectsAsync(ObjectBrowseQuery request, CancellationToken cancellationToken = default)
	{
		using var client = CreateApiClient();
		var pageSize = Math.Clamp(request.PageSize, 12, 100);
		var requestedPage = Math.Max(request.Page, 1);
		var search = string.IsNullOrWhiteSpace(request.Search) ? null : request.Search.Trim();
		var objects = (await Client.GetObjectListAsync(client)).ToList();

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

		if (request.Availability.HasValue)
		{
			query = query.Where(x => x.Availability == request.Availability.Value);
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
			.OrderByDescending(x => x.UploadedDate)
			.ThenBy(x => x.InternalName)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToList();

		var items = rows.Select(MapBrowseItem).ToList();
		return new ObjectBrowsePageViewModel(totalCount, filteredCount, page, pageSize, items);
	}

	public async Task<ObjectDetailViewModel?> GetObjectAsync(UniqueObjectId id, CancellationToken cancellationToken = default)
	{
		using var client = CreateApiClient();
		var obj = await Client.GetObjectAsync(client, id);

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
			obj.Licence?.Name,
			obj.Licence?.Text,
			[.. obj.Authors.Select(x => x.Name).OrderBy(x => x)],
			[.. obj.Tags.Select(x => x.Name).OrderBy(x => x)],
			[.. obj.ObjectPacks.Select(x => x.Name).OrderBy(x => x)],
			files,
			stringTableGroups,
			images,
			imageTableMessage);
	}

	ObjectListItemViewModel MapBrowseItem(DtoObjectEntry row)
	{
		var canDownload = IsDownloadAllowed(row.ObjectSource, row.Availability);

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
			canDownload);
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
		var zipBytes = await Client.GetObjectImagesAsync(client, id);
		if (zipBytes == null || zipBytes.Length == 0)
		{
			return [];
		}

		using var memoryStream = new MemoryStream(zipBytes, writable: false);
		using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read, false);
		var images = new List<ObjectImageViewModel>();

		foreach (var entry in zipArchive.Entries.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase))
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (!entry.FullName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}

			await using var entryStream = entry.Open();
			await using var pngStream = new MemoryStream();
			await entryStream.CopyToAsync(pngStream, cancellationToken);
			var pngBytes = pngStream.ToArray();

			using var image = Image.Load(pngBytes);
			images.Add(new ObjectImageViewModel(
				ParseImageIndex(entry.Name),
				image.Width,
				image.Height,
				$"data:image/png;base64,{Convert.ToBase64String(pngBytes)}"));
		}

		return [.. images.OrderBy(x => x.Index)];
	}

	HttpClient CreateApiClient()
	{
		var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("An active HTTP request is required to create the ObjectService API client.");
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/");
		return client;
	}

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

	static int ParseImageIndex(string fileName)
		=> int.TryParse(Path.GetFileNameWithoutExtension(fileName), out var index)
			? index
			: int.MaxValue;

	static int LanguagePriority(LanguageId language)
		=> language switch
		{
			LanguageId.English_UK => 0,
			LanguageId.English_US => 1,
			_ => 2,
		};
}
