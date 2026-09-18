using Definitions.DTO;
using Microsoft.Extensions.Logging;
using System.IO.Hashing;

namespace Definitions.Web;

public static class Client
{
	public const string ApiVersion = Routes.Prefix;

	public static ApiEndpointGroup ObjectsEndpointGroup { get; } = new(Routes.Objects);
	public static ApiEndpointGroup ObjectPacksEndpointGroup { get; } = new(Routes.ObjectPacks);
	public static ApiEndpointGroup ScenariosEndpointGroup { get; } = new(Routes.Scenarios);
	public static ApiEndpointGroup SC5FilePacksEndpointGroup { get; } = new(Routes.SC5FilePacks);
	public static ApiEndpointGroup AuthorsEndpointGroup { get; } = new(Routes.Authors);
	public static ApiEndpointGroup TagsEndpointGroup { get; } = new(Routes.Tags);
	public static ApiEndpointGroup LicencesEndpointGroup { get; } = new(Routes.Licences);
	public static ApiEndpointGroup MissingObjectsEndpointGroup { get; } = new(Routes.Objects + Routes.Missing);

	public static async Task<IEnumerable<T>> GetListAsync<T>(HttpClient client, ApiEndpointGroup endpointGroup, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<IEnumerable<T>>(
			client,
			endpointGroup.Prefix,
			endpointGroup.Route,
			null,
			logger,
			cancellationToken) ?? [];

	public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoObjectEntry>(client, ObjectsEndpointGroup, logger, cancellationToken);

	public static async Task<DtoObjectPostResponse?> GetObjectAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoObjectPostResponse>(
			client,
			ApiVersion,
			Routes.Objects,
			id,
			logger,
			cancellationToken);

	public static async Task<DtoObjectPostResponse?> UpdateObjectAsync(HttpClient client, UniqueObjectId id, DtoObjectPostResponse request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			client,
			ApiVersion,
			Routes.Objects,
			id,
			request,
			logger,
			cancellationToken);

	public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Objects + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.Objects + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetObjectImagesAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Objects + $"/{id}{Routes.Images}",
			ct => client.GetAsync(ApiVersion + Routes.Objects + $"/{id}{Routes.Images}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetObjectImageAsync(HttpClient client, UniqueObjectId id, int imageId, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Objects + $"/{id}{Routes.Images}/{imageId}",
			ct => client.GetAsync(ApiVersion + Routes.Objects + $"/{id}{Routes.Images}/{imageId}", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetScenarioFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.Scenarios + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.Scenarios + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetSC5FilePackFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.SC5FilePacks + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.SC5FilePacks + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<byte[]?> GetObjectPackFileAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.SendRequestAsync(
			client,
			ApiVersion + Routes.ObjectPacks + $"/{id}/file",
			ct => client.GetAsync(ApiVersion + Routes.ObjectPacks + $"/{id}/file", ct),
			ClientHelpers.ReadBinaryContentAsync,
			logger,
			cancellationToken) ?? default;

	public static async Task<DtoObjectPostResponse?> UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateOnly creationDate, DateOnly modifiedDate, ILogger logger, CancellationToken cancellationToken = default)
	{
		var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		logger.LogDebug("Posting {Filename} to {OriginalString}{Objects}", filename, client.BaseAddress?.OriginalString, Routes.Objects);
		var request = new DtoObjectPost(Convert.ToBase64String(datFileBytes), xxHash3, ObjectAvailability.Available, creationDate, modifiedDate);
		return await ClientHelpers.PostAsync<DtoObjectPost, DtoObjectPostResponse>(
			client,
			ApiVersion,
			Routes.Objects,
			request,
			logger,
			cancellationToken);
	}

	public static async Task<DtoObjectMissingEntry?> AddMissingObjectAsync(HttpClient client, DtoObjectMissingPost entry, ILogger? logger = null, CancellationToken cancellationToken = default)
	{
		logger?.LogDebug("Posting missing object {DatName} with checksum {DatChecksum} to {OriginalString}{Objects}{Missing}", entry.DatName, entry.DatChecksum, client.BaseAddress?.OriginalString, Routes.Objects, Routes.Missing);
		return await ClientHelpers.PostAsync<DtoObjectMissingPost, DtoObjectMissingEntry>(
			client,
			ApiVersion,
			Routes.Objects + Routes.Missing,
			entry,
			logger,
			cancellationToken);
	}

	public static async Task<IEnumerable<DtoLicenceEntry>> GetLicencesAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoLicenceEntry>(client, LicencesEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoAuthorEntry>> GetAuthorsAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoAuthorEntry>(client, AuthorsEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoTagEntry>> GetTagsAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoTagEntry>(client, TagsEndpointGroup, logger, cancellationToken);

	public static async Task<IEnumerable<DtoItemPackEntry>> GetObjectPacksAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoItemPackEntry>(client, ObjectPacksEndpointGroup, logger, cancellationToken);

	public static async Task<DtoItemPackDescriptor<DtoObjectEntry>?> GetObjectPackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> (await ClientHelpers.GetAsync<IEnumerable<DtoItemPackDescriptor<DtoObjectEntry>>>(
			client,
			ApiVersion,
			Routes.ObjectPacks,
			id,
			logger,
			cancellationToken))?.FirstOrDefault();

	public static async Task<IEnumerable<DtoScenarioEntry>> GetScenariosAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoScenarioEntry>(client, ScenariosEndpointGroup, logger, cancellationToken);

	public static async Task<DtoScenarioDescriptor?> GetScenarioAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await ClientHelpers.GetAsync<DtoScenarioDescriptor>(
			client,
			ApiVersion,
			Routes.Scenarios,
			id,
			logger,
			cancellationToken);

	public static async Task<IEnumerable<DtoItemPackEntry>> GetSC5FilePacksAsync(HttpClient client, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await GetListAsync<DtoItemPackEntry>(client, SC5FilePacksEndpointGroup, logger, cancellationToken);

	public static async Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetSC5FilePackAsync(HttpClient client, UniqueObjectId id, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> (await ClientHelpers.GetAsync<IEnumerable<DtoItemPackDescriptor<DtoScenarioEntry>>>(
			client,
			ApiVersion,
			Routes.SC5FilePacks,
			id,
			logger,
			cancellationToken))?.FirstOrDefault();

	public static async Task<IEnumerable<DtoObjectMissingEntry>> GetMissingObjectsAsync(HttpClient client, ILogger? logger = null)
		=> await GetListAsync<DtoObjectMissingEntry>(client, MissingObjectsEndpointGroup, logger);
}
