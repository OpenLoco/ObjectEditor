using Common;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gui;

//public class LocalUser(string Email, string Password)
//{
//	public string Email { get; } = Email;
//	public string Password { get; } = Password;
//	public string UserName { get; set; } // set when user logs in
//	public TblAuthor? AssociatedAuthor { get; set; }
//}

public class ObjectServiceClient
{
	//public LocalUser LocoUser { get; set; }

	public HttpClient WebClient { get; }

	public ILogger Logger { get; } = null!;

	public CookieContainer CookieContainer { get; set; }

	public ObjectServiceClient(EditorSettings settings, ILogger logger)
	{
		Logger = logger;
		CookieContainer = new CookieContainer();
		var handler = new HttpClientHandler() { CookieContainer = CookieContainer };
		WebClient = new HttpClient(handler);

		var serverAddress = settings.UseHttps
			? settings.ServerAddressHttps
			: settings.ServerAddressHttp;

		if (Uri.TryCreate(serverAddress, new(), out var serverUri))
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

		//LocoUser = new LocalUser(settings.ServerEmail, settings.ServerPassword);
	}

	//public async Task<DtoLoginRequest>

	public async Task<IEnumerable<T>> GetListAsync<T>(ApiEndpointGroup endpointGroup)
		=> await Client.GetListAsync<T>(WebClient, endpointGroup, Logger);

	public async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync()
		=> await Client.GetObjectListAsync(WebClient, Logger);

	public async Task<DtoServerStatus?> GetServerStatusAsync()
		=> await Client.GetServerStatusAsync(WebClient, Logger);

	public async Task<DtoObjectPostResponse?> GetObjectAsync(UniqueObjectId id)
		=> await Client.GetObjectAsync(WebClient, id, Logger);

	public async Task<DtoObjectPostResponse?> UpdateObjectAsync(UniqueObjectId id, DtoObjectPostResponse request)
		=> await Client.UpdateObjectAsync(WebClient, id, request, Logger);

	public async Task<byte[]?> GetObjectFileAsync(UniqueObjectId id)
		=> await Client.GetObjectFileAsync(WebClient, id, Logger);

	public async Task<byte[]?> GetScenarioFileAsync(UniqueObjectId id)
		=> await Client.GetScenarioFileAsync(WebClient, id, Logger);

	public async Task<byte[]?> GetScenarioPackFileAsync(UniqueObjectId id)
		=> await Client.GetScenarioPackFileAsync(WebClient, id, Logger);

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

	public async Task<IEnumerable<DtoScenarioListEntry>> GetScenariosAsync()
		=> await Client.GetScenariosAsync(WebClient, Logger);

	public async Task<DtoScenarioDescriptor?> GetScenarioAsync(UniqueObjectId id)
		=> await Client.GetScenarioAsync(WebClient, id, Logger);

	public async Task<IEnumerable<DtoItemPackEntry>> GetScenarioPacksAsync()
		=> await Client.GetScenarioPacksAsync(WebClient, Logger);

	public async Task<DtoItemPackDescriptor<DtoScenarioEntry>?> GetScenarioPackAsync(UniqueObjectId id)
		=> await Client.GetScenarioPackAsync(WebClient, id, Logger);

	public async Task<IEnumerable<DtoObjectMissingEntry>> GetMissingObjectsAsync()
		=> await Client.GetMissingObjectsAsync(WebClient, Logger);
}
