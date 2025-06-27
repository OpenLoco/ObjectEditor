using Microsoft.AspNetCore.Http;
using OpenLoco.Common.Logging;
using System.Net.Http.Json;

namespace OpenLoco.Definitions.Web
{
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

		//static JsonSerializerOptions GetDefaultSerializerOptions()
		//{
		//	var options = new JsonSerializerOptions();
		//	//options.Converters.Add(new JsonStringEnumConverter());
		//	return options;
		//}

		public static async Task<T?> GetAsync<T>(HttpClient client, string apiRoute, string route, UniqueObjectId? resourceId = null, ILogger? logger = null)
			=> await SendRequestAsync(
				client,
				FormRoute(apiRoute, route, resourceId),
				() => client.GetAsync(FormRoute(apiRoute, route, resourceId)),
				ReadJsonContentAsync<T?>,
				logger) ?? default;

		public static async Task<bool> DeleteAsync(HttpClient client, string apiRoute, string route, UniqueObjectId resourceId, ILogger? logger = null)
			=> await SendRequestAsync<bool?>(
				client,
				FormRoute(apiRoute, route, resourceId),
				() => client.DeleteAsync(FormRoute(apiRoute, route, resourceId)),
				null,
				logger) != null;

		public static async Task<T?> PostAsync<T>(HttpClient client, string apiRoute, string route, T request, ILogger? logger = null)
			=> await SendRequestAsync(
				client,
				FormRoute(apiRoute, route, null),
				() => client.PostAsJsonAsync(FormRoute(apiRoute, route, null), request),
				ReadJsonContentAsync<T?>,
				logger) ?? default;

		public static async Task<T?> PutAsync<T>(HttpClient client, string apiRoute, string route, UniqueObjectId resourceId, T request, ILogger? logger = null)
			=> await SendRequestAsync(
				client,
				FormRoute(apiRoute, route, resourceId),
				() => client.PutAsJsonAsync(FormRoute(apiRoute, route, resourceId), request),
				ReadJsonContentAsync<T?>,
				logger) ?? default;

		static string FormRoute(string apiRoute, string baseRoute, UniqueObjectId? resourceId)
			=> resourceId == null
				? apiRoute + baseRoute
				: apiRoute + baseRoute + $"/{resourceId}";

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

				logger?.Debug($"Received success response: {response.StatusCode}");

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
