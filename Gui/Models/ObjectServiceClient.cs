using OpenLoco.Common.Logging;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenLoco.Gui.Models
{
	public class LocalUser(string Email, string Password)
	{
		public string Email { get; } = Email;
		public string Password { get; } = Password;
		public string UserName { get; set; } // set when user logs in
		public TblAuthor? AssociatedAuthor { get; set; }
	}

	public class ObjectServiceClient
	{
		//public LocalUser LocoUser { get; set; }

		public HttpClient WebClient { get; }

		public ILogger Logger { get; init; }

		public CookieContainer CookieContainer { get; set; }

		public ObjectServiceClient(EditorSettings settings, ILogger logger)
		{
			var serverAddress = settings.UseHttps
				? settings.ServerAddressHttps
				: settings.ServerAddressHttp;

			if (Uri.TryCreate(serverAddress, new(), out var serverUri))
			{
				CookieContainer = new CookieContainer();
				var handler = new HttpClientHandler() { CookieContainer = CookieContainer };
				WebClient = new HttpClient(handler) { BaseAddress = serverUri, };
				Logger?.Info($"Successfully registered object service with address \"{serverUri}\"");
			}
			else
			{
				Logger?.Error($"Unable to parse object service address \"{serverAddress}\". Online functionality will not work until the address is corrected and the editor is restarted.");
			}

			Logger = logger;

			//LocoUser = new LocalUser(settings.ServerEmail, settings.ServerPassword);
		}

		//public async Task<DtoLoginRequest>

		public async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync()
			=> await Client.GetObjectListAsync(WebClient, Logger);

		public async Task<DtoObjectDescriptor?> GetObjectAsync(UniqueObjectId id)
			=> await Client.GetObjectAsync(WebClient, id, Logger);

		public async Task<byte[]?> GetObjectFileAsync(UniqueObjectId id)
			=> await Client.GetObjectFileAsync(WebClient, id, Logger);

		public async Task UploadDatFileAsync(string filename, byte[] datFileBytes, DateOnly creationDate, DateOnly modifiedDate)
			=> await Client.UploadDatFileAsync(WebClient, filename, datFileBytes, creationDate, modifiedDate, Logger);
	}
}
