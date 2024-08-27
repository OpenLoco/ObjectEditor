using OpenLoco.Common.Logging;
using OpenLoco.Definitions.DTO;
using System.Net.Http.Json;

namespace OpenLoco.Definitions.Web
{
	public static class Client
	{
		public static async Task<IEnumerable<DtoObjectIndexEntry>?> GetObjectListAsync(HttpClient client, ILogger? logger = null)
			=> await SendRequestAsync<IEnumerable<DtoObjectIndexEntry>?>(client, Routes.ListObjects, logger);

		public static async Task<DtoLocoObject?> GetDatAsync(HttpClient client, string objectName, uint checksum, bool returnObjBytes, ILogger? logger = null)
			=> await SendRequestAsync<DtoLocoObject?>(client, Routes.GetDat + $"?{nameof(objectName)}={objectName}&{nameof(checksum)}={checksum}&{nameof(returnObjBytes)}={returnObjBytes}", logger);

		public static async Task<DtoLocoObject?> GetObjectAsync(HttpClient client, int uniqueObjectId, bool returnObjBytes, ILogger? logger = null)
			=> await SendRequestAsync<DtoLocoObject?>(client, Routes.GetObject + $"?{nameof(uniqueObjectId)}={uniqueObjectId}&{nameof(returnObjBytes)}={returnObjBytes}", logger);

		public static async Task<DtoLocoObject?> GetDatFileAsync(HttpClient client, string objectName, uint checksum, ILogger? logger = null)
			=> await SendRequestAsync<DtoLocoObject?>(client, Routes.GetDatFile + $"?{nameof(objectName)}={objectName}&{nameof(checksum)}={checksum}", logger);

		public static async Task<DtoLocoObject?> GetObjectFileAsync(HttpClient client, int uniqueObjectId, ILogger? logger = null)
			=> await SendRequestAsync<DtoLocoObject?>(client, Routes.GetDatFile + $"?{nameof(uniqueObjectId)}={uniqueObjectId}", logger);


		public static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, ILogger? logger = null)
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
