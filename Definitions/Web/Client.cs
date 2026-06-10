using Definitions.DTO;
using Microsoft.Extensions.Logging;
using System.IO.Hashing;

namespace Definitions.Web;

public static class Client
{
	public const string ApiVersion = RoutesV2.Prefix;

	public static ApiEndpointGroup ObjectsEndpointGroup { get; } = new(RoutesV2.Objects);
	public static ApiEndpointGroup ObjectPacksEndpointGroup { get; } = new(RoutesV2.ObjectPacks);
	public static ApiEndpointGroup ScenariosEndpointGroup { get; } = new(RoutesV2.Scenarios);
	public static ApiEndpointGroup SC5FilePacksEndpointGroup { get; } = new(RoutesV2.SC5FilePacks);
	public static ApiEndpointGroup AuthorsEndpointGroup { get; } = new(RoutesV2.Authors);
	public static ApiEndpointGroup TagsEndpointGroup { get; } = new(RoutesV2.Tags);
	public static ApiEndpointGroup LicencesEndpointGroup { get; } = new(RoutesV2.Licences);
	public static ApiEndpointGroup MissingObjectsEndpointGroup { get; } = new(RoutesV2.Objects + RoutesV2.Missing);

	public static async Task<IEnumerable<T>> GetListAsync<T>(HttpClient client, ApiEndpointGroup endpointGroup, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<IEnumerable<T>>(
			client,
			endpointGroup.Prefix,
			endpointGroup.Route,
			null,
			logger) ?? [];

	public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoObjectEntry>(client, ObjectsEndpointGroup, logger);

	public static async Task<DtoObjectPostResponse?> GetObjectAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<DtoObjectPostResponse>(
			client,
			ApiVersion,
			RoutesV2.Objects,
			id,
			logger);

	public static async Task<DtoObjectPostResponse?> UpdateObjectAsync(HttpClient client, UniqueObjectId id, DtoObjectPostResponse request, ILogger? logger = null)
		=> await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			client,
			ApiVersion,
			RoutesV2.Objects,
			id,
			request,
			logger);

	public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + RoutesV2.Objects + $"/{id}/file",
			() => client.GetAsync(ApiVersion + RoutesV2.Objects + $"/{id}/file"),
			ClientHelpers.ReadBinaryContentAsync,
			logger) ?? default;

	public static async Task<byte[]?> GetObjectImagesAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + RoutesV2.Objects + $"/{id}{RoutesV2.Images}",
			() => client.GetAsync(ApiVersion + RoutesV2.Objects + $"/{id}{RoutesV2.Images}"),
			ClientHelpers.ReadBinaryContentAsync,
			logger) ?? default;

	public static async Task<byte[]?> GetScenarioFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + RoutesV2.Scenarios + $"/{id}/file",
			() => client.GetAsync(ApiVersion + RoutesV2.Scenarios + $"/{id}/file"),
			ClientHelpers.ReadBinaryContentAsync,
			logger) ?? default;

	public static async Task<byte[]?> GetSC5FilePackFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + RoutesV2.SC5FilePacks + $"/{id}/file",
			() => client.GetAsync(ApiVersion + RoutesV2.SC5FilePacks + $"/{id}/file"),
			ClientHelpers.ReadBinaryContentAsync,
			logger) ?? default;

	public static async Task<byte[]?> GetObjectPackFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + RoutesV2.ObjectPacks + $"/{id}/file",
			() => client.GetAsync(ApiVersion + RoutesV2.ObjectPacks + $"/{id}/file"),
			ClientHelpers.ReadBinaryContentAsync,
			logger) ?? default;

	public static async Task<DtoObjectPostResponse?> UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateOnly creationDate, DateOnly modifiedDate, ILogger logger)
	{
		var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		logger.LogDebug("Posting {Filename} to {OriginalString}{Objects}", filename, client.BaseAddress?.OriginalString, RoutesV2.Objects);
		var request = new DtoObjectPost(Convert.ToBase64String(datFileBytes), xxHash3, ObjectAvailability.Available, creationDate, modifiedDate);
		return await ClientHelpers.PostAsync<DtoObjectPost, DtoObjectPostResponse>(
			client,
			ApiVersion,
			RoutesV2.Objects,
			request);
	}

	public static async Task<DtoObjectMissingEntry?> AddMissingObjectAsync(HttpClient client, DtoObjectMissingPost entry, ILogger? logger = null)
	{
		logger?.LogDebug("Posting missing object {DatName} with checksum {DatChecksum} to {OriginalString}{Objects}{Missing}", entry.DatName, entry.DatChecksum, client.BaseAddress?.OriginalString, RoutesV2.Objects, RoutesV2.Missing);
		return await ClientHelpers.PostAsync<DtoObjectMissingPost, DtoObjectMissingEntry>(
			client,
			ApiVersion,
			RoutesV2.Objects + RoutesV2.Missing,
			entry,
			logger);
	}

	public static async Task<IEnumerable<DtoLicenceEntry>> GetLicencesAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoLicenceEntry>(client, LicencesEndpointGroup, logger);

	public static async Task<IEnumerable<DtoAuthorEntry>> GetAuthorsAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoAuthorEntry>(client, AuthorsEndpointGroup, logger);

	public static async Task<IEnumerable<DtoTagEntry>> GetTagsAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoTagEntry>(client, TagsEndpointGroup, logger);

	public static async Task<IEnumerable<DtoItemPackEntry>> GetObjectPacksAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoItemPackEntry>(client, ObjectPacksEndpointGroup, logger);

	public static async Task<DtoItemPackDescriptor<DtoObjectEntry>?> GetObjectPackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> (await ClientHelpers.GetAsync<IEnumerable<DtoItemPackDescriptor<DtoObjectEntry>>>(
			client,
			ApiVersion,
			RoutesV2.ObjectPacks,
			id,
			logger))?.FirstOrDefault();

	public static async Task<IEnumerable<DtoScenarioEntry>> GetScenariosAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoScenarioEntry>(client, ScenariosEndpointGroup, logger);

	public static async Task<IEnumerable<DtoItemPackEntry>> GetSC5FilePacksAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoItemPackEntry>(client, SC5FilePacksEndpointGroup, logger);

	public static async Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetSC5FilePackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null)
		=> (await ClientHelpers.GetAsync<IEnumerable<DtoItemPackDescriptor<DtoScenarioEntry>>>(
			client,
			ApiVersion,
			RoutesV2.SC5FilePacks,
			id,
			logger))?.FirstOrDefault();

	public static async Task<IEnumerable<DtoObjectMissingEntry>> GetMissingObjectsAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoObjectMissingEntry>(client, MissingObjectsEndpointGroup, logger);

	public static async Task<DtoIndexFolderResponse?> IndexFolderAsync(HttpClient client, string folderPath, ILogger? logger = null)
	{
		logger?.LogDebug("Posting index-folder request for \"{Path}\" to {BaseAddress}{Folders}{Index}", folderPath, client.BaseAddress?.OriginalString, RoutesV2.Folders, RoutesV2.Index);
		return await ClientHelpers.PostAsync<DtoIndexFolderRequest, DtoIndexFolderResponse>(
			client,
			ApiVersion,
			RoutesV2.Folders + RoutesV2.Index,
			new DtoIndexFolderRequest(folderPath),
			logger);
	}
}
