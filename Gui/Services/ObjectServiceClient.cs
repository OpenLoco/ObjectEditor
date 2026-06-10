using Common;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gui.Services;

public class ObjectServiceClient
{
	public HttpClient WebClient { get; }

	public ILogger Logger { get; } = null!;

	public CookieContainer CookieContainer { get; set; }

	readonly EditorSettings settings;

	public ObjectServiceClient(EditorSettings settings, ILogger logger, Uri? baseAddressOverride = null)
	{
		this.settings = settings;
		Logger = logger;
		CookieContainer = new CookieContainer();
		var handler = new HttpClientHandler() { CookieContainer = CookieContainer };
		WebClient = new HttpClient(handler);

		// baseAddressOverride wins when supplied (eg. an in-process EmbeddedObjectServiceHost).
		// Otherwise fall back to the configured remote server address.
		var serverUri = baseAddressOverride;
		var serverAddress = baseAddressOverride?.ToString();

		if (serverUri is null)
		{
			serverAddress = settings.UseHttps
				? settings.ServerAddressHttps
				: settings.ServerAddressHttp;
			_ = Uri.TryCreate(serverAddress, UriKind.Absolute, out serverUri);
		}

		if (serverUri is not null)
		{
			WebClient.BaseAddress = serverUri;

			var currentAppVersion = VersionHelpers.GetCurrentAppVersion();
			WebClient.DefaultRequestHeaders.UserAgent.ParseAdd($"ObjectEditor/{currentAppVersion}");

			Logger.LogInformation("Successfully registered object service with address \"{ServerUri}\"", serverUri);
		}
		else
		{
			Logger.LogError("Unable to parse object service address \"{ServerAddress}\". Online functionality will not work until the address is corrected and the editor is restarted.", serverAddress);
		}
	}

	// Used when the embedded ObjectService is started or restarted after construction (eg.
	// hot-reapply after settings change). Pass null to fall back to the configured remote
	// server address from EditorSettings.
	public void RetargetBaseAddress(Uri? newBaseAddress)
	{
		if (newBaseAddress is not null)
		{
			WebClient.BaseAddress = newBaseAddress;
			Logger.LogInformation("Object service client retargeted to \"{ServerUri}\"", newBaseAddress);
			return;
		}

		var serverAddress = settings.UseHttps ? settings.ServerAddressHttps : settings.ServerAddressHttp;
		if (Uri.TryCreate(serverAddress, UriKind.Absolute, out var remoteUri))
		{
			WebClient.BaseAddress = remoteUri;
			Logger.LogInformation("Object service client reverted to remote address \"{ServerUri}\"", remoteUri);
		}
		else
		{
			Logger.LogWarning("Object service client could not revert to remote address; \"{ServerAddress}\" is not a valid URI.", serverAddress);
		}
	}

	public async Task<IEnumerable<T>> GetListAsync<T>(ApiEndpointGroup endpointGroup)
		=> await Client.GetListAsync<T>(WebClient, endpointGroup, Logger);

	public async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync()
		=> await Client.GetObjectListAsync(WebClient, Logger);

	public async Task<DtoObjectPostResponse?> GetObjectAsync(UniqueObjectId id)
		=> await Client.GetObjectAsync(WebClient, id, Logger);

	public async Task<DtoObjectPostResponse?> UpdateObjectAsync(UniqueObjectId id, DtoObjectPostResponse request)
		=> await Client.UpdateObjectAsync(WebClient, id, request, Logger);

	public async Task<byte[]?> GetObjectFileAsync(UniqueObjectId id)
		=> await Client.GetObjectFileAsync(WebClient, id, Logger);

	public async Task<byte[]?> GetScenarioFileAsync(UniqueObjectId id)
		=> await Client.GetScenarioFileAsync(WebClient, id, Logger);

	public async Task<byte[]?> GetSC5FilePackFileAsync(UniqueObjectId id)
		=> await Client.GetSC5FilePackFileAsync(WebClient, id, Logger);

	public async Task<byte[]?> GetObjectPackFileAsync(UniqueObjectId id)
		=> await Client.GetObjectPackFileAsync(WebClient, id, Logger);

	public async Task<DtoObjectPostResponse?> UploadDatFileAsync(string filename, byte[] datFileBytes, DateOnly creationDate, DateOnly modifiedDate)
		=> await Client.UploadDatFileAsync(WebClient, filename, datFileBytes, creationDate, modifiedDate, Logger);

	public async Task<DtoObjectMissingEntry?> AddMissingObjectAsync(DtoObjectMissingPost entry)
		=> await Client.AddMissingObjectAsync(WebClient, entry, Logger);

	public async Task<IEnumerable<DtoLicenceEntry>> GetLicencesAsync()
		=> await Client.GetLicencesAsync(WebClient, Logger);

	public async Task<IEnumerable<DtoAuthorEntry>> GetAuthorsAsync()
		=> await Client.GetAuthorsAsync(WebClient, Logger);

	public async Task<IEnumerable<DtoTagEntry>> GetTagsAsync()
		=> await Client.GetTagsAsync(WebClient, Logger);

	public async Task<IEnumerable<DtoItemPackEntry>> GetObjectPacksAsync()
		=> await Client.GetObjectPacksAsync(WebClient, Logger);

	public async Task<DtoItemPackDescriptor<DtoObjectEntry>?> GetObjectPackAsync(UniqueObjectId id)
		=> await Client.GetObjectPackAsync(WebClient, id, Logger);

	public async Task<IEnumerable<DtoScenarioEntry>> GetScenariosAsync()
		=> await Client.GetScenariosAsync(WebClient, Logger);

	public async Task<IEnumerable<DtoItemPackEntry>> GetSC5FilePacksAsync()
		=> await Client.GetSC5FilePacksAsync(WebClient, Logger);

	public async Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetSC5FilePackAsync(UniqueObjectId id)
		=> await Client.GetSC5FilePackAsync(WebClient, id, Logger);

	public async Task<IEnumerable<DtoObjectMissingEntry>> GetMissingObjectsAsync()
		=> await Client.GetMissingObjectsAsync(WebClient, Logger);
}
