using Common.Logging;
using Definitions.DTO;
using System.IO.Hashing;

namespace Definitions.Web;

public static class Client
{
	public const string ApiVersion = RoutesV2.Prefix;

	public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoObjectEntry>>(
			client,
			ApiVersion,
			RoutesV2.Objects,
			null,
			logger) ?? [];

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

	public static async Task<DtoObjectPostResponse?> UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateOnly creationDate, DateOnly modifiedDate, ILogger logger)
	{
		var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
		logger.Debug($"Posting {filename} to {client.BaseAddress?.OriginalString}{RoutesV2.Objects}");
		var request = new DtoObjectPost(Convert.ToBase64String(datFileBytes), xxHash3, ObjectAvailability.Available, creationDate, modifiedDate);
		return await ClientHelpers.PostAsync<DtoObjectPost, DtoObjectPostResponse>(
			client,
			ApiVersion,
			RoutesV2.Objects,
			request);
	}

	public static async Task<DtoObjectMissingEntry?> AddMissingObjectAsync(HttpClient client, DtoObjectMissingPost entry, ILogger? logger = null)
	{
		logger?.Debug($"Posting missing object {entry.DatName} with checksum {entry.DatChecksum} to {client.BaseAddress?.OriginalString}{RoutesV2.Objects}{RoutesV2.Missing}");
		return await ClientHelpers.PostAsync<DtoObjectMissingPost, DtoObjectMissingEntry>(
			client,
			ApiVersion,
			RoutesV2.Objects + RoutesV2.Missing,
			entry,
			logger);
	}

	public static async Task<IEnumerable<DtoLicenceEntry>> GetLicencesAsync(HttpClient client, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoLicenceEntry>>(
			client,
			ApiVersion,
			RoutesV2.Licences,
			null,
			logger) ?? [];

	public static async Task<IEnumerable<DtoAuthorEntry>> GetAuthorsAsync(HttpClient client, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoAuthorEntry>>(
			client,
			ApiVersion,
			RoutesV2.Authors,
			null,
			logger) ?? [];

	public static async Task<IEnumerable<DtoTagEntry>> GetTagsAsync(HttpClient client, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoTagEntry>>(
			client,
			ApiVersion,
			RoutesV2.Tags,
			null,
			logger) ?? [];

	public static async Task<IEnumerable<DtoItemPackEntry>> GetObjectPacksAsync(HttpClient client, ILogger? logger = null)
		=> await ClientHelpers.GetAsync<IEnumerable<DtoItemPackEntry>>(
			client,
			ApiVersion,
			RoutesV2.ObjectPacks,
			null,
			logger) ?? [];
}
