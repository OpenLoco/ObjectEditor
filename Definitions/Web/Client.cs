using OpenLoco.Definitions.DTO;
using System.Net.Http.Json;

namespace OpenLoco.Definitions.Web
{
	public static class Client
	{
		public static async Task<IEnumerable<DtoObjectIndexEntry>?> GetObjectListAsync(HttpClient client, OpenLoco.Common.Logging.ILogger? logger = null)
			=> await SendRequestAsync<IEnumerable<DtoObjectIndexEntry>?>(client, Routes.ListObjects, logger);
		public static async Task<DtoLocoObject?> GetObjectAsync(HttpClient client, int uniqueObjectId, OpenLoco.Common.Logging.ILogger? logger = null)
			=> await SendRequestAsync<DtoLocoObject?>(client, Routes.GetObject + $"?{nameof(uniqueObjectId)}={uniqueObjectId}", logger);
		public static async Task<DtoLocoObject?> GetDatAsync(HttpClient client, string objectName, uint checksum, OpenLoco.Common.Logging.ILogger? logger = null)
			=> await SendRequestAsync<DtoLocoObject?>(client, Routes.GetDat + $"?{nameof(objectName)}={objectName}&{nameof(checksum)}={checksum}", logger);

		public static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, OpenLoco.Common.Logging.ILogger? logger = null)
		{
			try
			{
				route = route.TrimStart('/');
				logger?.Info($"Querying {client.BaseAddress}{route}");
				using var response = await client.GetAsync(route);

				if (!response.IsSuccessStatusCode)
				{
					logger?.Error($"Request failed: {response}");
					return default;
				}

				logger?.Info("Main server queried successfully");

				var data = await response.Content.ReadFromJsonAsync<T?>();
				if (data == null)
				{
					logger?.Error($"Received data but couldn't parse it: {response}");
					return default;
				}
				return data;
			}
			catch (Exception ex)
			{
				logger?.Error(ex);
				return default;
			}
		}
	}
}
