using Microsoft.AspNetCore.Http;
using OpenLoco.Common.Logging;
using OpenLoco.Definitions.DTO;
using System.Net.Http.Json;

namespace OpenLoco.Definitions.Web
{
	public static class Client
	{
		public static async Task<IEnumerable<DtoObjectIndexEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null)
			=> await SendRequestAsync<IEnumerable<DtoObjectIndexEntry>?>(client, Routes.ListObjects, logger) ?? [];

		public static async Task<DtoDatObjectWithMetadata?> GetDatAsync(HttpClient client, string objectName, uint checksum, bool returnObjBytes, ILogger? logger = null)
			=> await SendRequestAsync<DtoDatObjectWithMetadata?>(client, Routes.GetDat + $"?{nameof(objectName)}={objectName}&{nameof(checksum)}={checksum}&{nameof(returnObjBytes)}={returnObjBytes}", logger);

		public static async Task<DtoDatObjectWithMetadata?> GetObjectAsync(HttpClient client, int uniqueObjectId, bool returnObjBytes, ILogger? logger = null)
			=> await SendRequestAsync<DtoDatObjectWithMetadata?>(client, Routes.GetObject + $"?{nameof(uniqueObjectId)}={uniqueObjectId}&{nameof(returnObjBytes)}={returnObjBytes}", logger);

		public static async Task<DtoDatObjectWithMetadata?> GetDatFileAsync(HttpClient client, string objectName, uint checksum, ILogger? logger = null)
			=> await SendRequestAsync<DtoDatObjectWithMetadata?>(client, Routes.GetDatFile + $"?{nameof(objectName)}={objectName}&{nameof(checksum)}={checksum}", logger);

		public static async Task<DtoDatObjectWithMetadata?> GetObjectFileAsync(HttpClient client, int uniqueObjectId, ILogger? logger = null)
			=> await SendRequestAsync<DtoDatObjectWithMetadata?>(client, Routes.GetDatFile + $"?{nameof(uniqueObjectId)}={uniqueObjectId}", logger);

		static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, ILogger? logger = null)
		{
			try
			{
				route = route.TrimStart('/');
				logger?.Debug($"Querying {client.BaseAddress}{route}");
				using var response = await client.GetAsync(route);

				if (!response.IsSuccessStatusCode)
				{
					logger?.Error($"Request failed: {response}");
					return default;
				}

				logger?.Debug("Main server queried successfully");

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

		public static async Task UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateTimeOffset creationDate, ILogger logger)
		{
			try
			{
				var route = $"{client.BaseAddress}{Routes.UploadDat}".Replace("//", "'/");
				logger.Debug($"Posting {filename} to {route}");
				var request = new DtoUploadDat(Convert.ToBase64String(datFileBytes), creationDate);
				var response = await client.PostAsJsonAsync(Routes.UploadDat, request);

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync();
					logger.Error($"Posting {filename} failed. Error={error}");
					return;
				}

				logger.Debug($"Uploaded {filename} to main server successfully");
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
		}
	}
}
