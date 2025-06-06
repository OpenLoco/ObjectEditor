using Microsoft.AspNetCore.Http;
using OpenLoco.Common.Logging;
using OpenLoco.Definitions.DTO;
using System.IO.Hashing;
using System.Net.Http.Json;

namespace OpenLoco.Definitions.Web
{
	public static class Client
	{
		public static async Task<IEnumerable<DtoObjectEntry>> GetObjectListAsync(HttpClient client, ILogger? logger = null)
			=> await SendRequestAsync<IEnumerable<DtoObjectEntry>?>(client, Routes.Objects, ReadJsonContentAsync<IEnumerable<DtoObjectEntry>?>, logger) ?? [];

		public static async Task<DtoObjectDescriptor?> GetObjectAsync(HttpClient client, int id, ILogger? logger = null)
			=> await SendRequestAsync<DtoObjectDescriptor?>(client, Routes.Objects + $"/{id}", ReadJsonContentAsync<DtoObjectDescriptor?>, logger);

		public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, int id, ILogger? logger = null)
			=> await SendRequestAsync<byte[]?>(client, Routes.Objects + $"/{id}/file", ReadBinaryContentAsync, logger);

		// generic method
		public static async Task<T?> Get<T>(HttpClient client, string route, ILogger? logger = null)
			=> await SendRequestAsync(client, route, ReadJsonContentAsync<T?>, logger) ?? default;

		static async Task<T?> ReadJsonContentAsync<T>(HttpContent content)
			=> await content.ReadFromJsonAsync<T?>();

		static async Task<byte[]?> ReadBinaryContentAsync(HttpContent content)
		{
			await using (var stream = await content.ReadAsStreamAsync())
			await using (var memoryStream = new MemoryStream())
			{
				await stream.CopyToAsync(memoryStream);  // Efficiently copy to memory stream
				return memoryStream.ToArray(); // Get the byte array
			}
		}

		static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, Func<HttpContent, Task<T?>> ContentReaderFunc, ILogger? logger = null)
		{
			try
			{
				if (!Uri.TryCreate(client.BaseAddress, route, out var uri))
				{
					logger?.Error($"Unable to create a URI from base=\"{client.BaseAddress}\" and route=\"{route}\"");
					return default;
				}

				logger?.Debug($"Querying {uri}");
				using var response = await client.GetAsync(uri);

				if (!response.IsSuccessStatusCode)
				{
					logger?.Error($"Request failed: {response}");
					return default;
				}

				logger?.Debug("Main server queried successfully");

				var data = await ContentReaderFunc(response.Content);
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

		public static async Task UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateTimeOffset creationDate, DateTimeOffset modifiedDate, ILogger logger)
		{
			try
			{
				var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
				var route = $"{client.BaseAddress?.OriginalString}{Routes.Objects}";
				logger.Debug($"Posting {filename} to {route}");
				var request = new DtoUploadDat(Convert.ToBase64String(datFileBytes), xxHash3, creationDate, modifiedDate);
				var response = await client.PostAsJsonAsync(Routes.Objects, request);

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync();

					if (string.IsNullOrEmpty(error))
					{
						logger.Error($"Posting {filename} failed. StatusCode={response.StatusCode} ReasonPhrase={response.ReasonPhrase}");
					}
					else
					{
						logger.Error($"Posting {filename} failed. Error={error}");
					}

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
