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
			=> await ClientHelpers.GetAsync<IEnumerable<DtoObjectEntry>>(
				client,
				Routes.Objects,
				null,
				logger) ?? [];

		public static async Task<DtoObjectDescriptor?> GetObjectAsync(HttpClient client, int id, ILogger? logger = null)
			=> await ClientHelpers.GetAsync<DtoObjectDescriptor>(
				client,
				Routes.Objects,
				id,
				logger);

		public static async Task<byte[]?> GetObjectFileAsync(HttpClient client, int id, ILogger? logger = null)
			=> await ClientHelpers.SendRequestAsync(
				client,
				Routes.Objects + $"/{id}/file",
				() => client.GetAsync(Routes.Objects + $"/{id}/file"),
				ClientHelpers.ReadBinaryContentAsync,
				logger) ?? default;

		public static async Task UploadDatFileAsync(HttpClient client, string filename, byte[] datFileBytes, DateTimeOffset creationDate, DateTimeOffset modifiedDate, ILogger logger)
		{
			var xxHash3 = XxHash3.HashToUInt64(datFileBytes);
			logger.Debug($"Posting {filename} to {client.BaseAddress?.OriginalString}{Routes.Objects}");
			var request = new DtoUploadDat(Convert.ToBase64String(datFileBytes), xxHash3, creationDate, modifiedDate);
			_ = await ClientHelpers.PostAsync(client, Routes.Objects, request);
		}
	}

	public static class ClientHelpers
	{
		public static async Task<byte[]?> ReadBinaryContentAsync(HttpContent content)
		{
			await using (var stream = await content.ReadAsStreamAsync())
			await using (var memoryStream = new MemoryStream())
			{
				await stream.CopyToAsync(memoryStream);  // Efficiently copy to memory stream
				return memoryStream.ToArray(); // Get the byte array
			}
		}

		internal static async Task<T?> ReadJsonContentAsync<T>(HttpContent content)
			=> await content.ReadFromJsonAsync<T?>();

		public static async Task<T?> GetAsync<T>(HttpClient client, string route, int? resourceId = null, ILogger? logger = null)
			=> await SendRequestAsync(
				client,
				FormRoute(route, resourceId),
				() => client.GetAsync(FormRoute(route, resourceId)),
				ReadJsonContentAsync<T?>,
				logger) ?? default;

		public static async Task<bool> DeleteAsync(HttpClient client, string route, int resourceId, ILogger? logger = null)
			=> await SendRequestAsync<bool?>(
				client,
				FormRoute(route, resourceId),
				() => client.DeleteAsync(FormRoute(route, resourceId)),
				null,
				logger) != null;

		public static async Task<T?> PostAsync<T>(HttpClient client, string route, T request, ILogger? logger = null)
			=> await SendRequestAsync(
				client,
				FormRoute(route, null),
				() => client.PostAsJsonAsync(FormRoute(route, null), request),
				ReadJsonContentAsync<T?>,
				logger) ?? default;

		public static async Task<T?> PutAsync<T>(HttpClient client, string route, int resourceId, T request, ILogger? logger = null)
			=> await SendRequestAsync(
				client,
				FormRoute(route, resourceId),
				() => client.PutAsJsonAsync(FormRoute(route, resourceId), request),
				ReadJsonContentAsync<T?>,
				logger) ?? default;

		static string FormRoute(string baseRoute, int? resourceId)
			=> resourceId == null
				? baseRoute
				: baseRoute + $"/{resourceId}";

		internal static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, Func<Task<HttpResponseMessage>> httpFunc, Func<HttpContent, Task<T?>>? contentReaderFunc = null, ILogger? logger = null)
		{
			try
			{
				if (!Uri.TryCreate(client.BaseAddress, route, out var uri))
				{
					logger?.Error($"Unable to create a URI from base=\"{client.BaseAddress}\" and route=\"{route}\"");
					return default;
				}

				logger?.Debug($"Sending to {uri}");
				using var response = await httpFunc();

				if (!response.IsSuccessStatusCode)
				{
					var error = await response.Content.ReadAsStringAsync();

					if (string.IsNullOrEmpty(error))
					{
						logger?.Error($"Failed. StatusCode={response.StatusCode} ReasonPhrase={response.ReasonPhrase}");
					}
					else
					{
						logger?.Error($"Failed. Error={error}");
					}

					return default;
				}

				logger?.Debug("Received success response");

				if (contentReaderFunc != null)
				{
					var data = await contentReaderFunc(response.Content);
					if (data == null)
					{
						logger?.Error($"Received data but couldn't parse it: {response}");
						return default;
					}

					return data;
				}
				else
				{
					return default;
				}
			}
			catch (Exception ex)
			{
				logger?.Error(ex);
				return default;
			}
		}
	}
}
