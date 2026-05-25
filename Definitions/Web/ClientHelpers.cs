using Common.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Definitions.Web;

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

	public static async Task<TResponse?> PostAsync<TRequest, TResponse>(HttpClient client, string apiRoute, string route, TRequest request, ILogger? logger = null)
		=> await SendRequestAsync(
			client,
			FormRoute(apiRoute, route, null),
			() => client.PostAsJsonAsync(FormRoute(apiRoute, route, null), request),
			ReadJsonContentAsync<TResponse?>,
			logger) ?? default;

	public static async Task<TResponse?> PutAsync<TRequest, TResponse>(HttpClient client, string apiRoute, string route, UniqueObjectId resourceId, TRequest request, ILogger? logger = null)
		=> await SendRequestAsync(
			client,
			FormRoute(apiRoute, route, resourceId),
			() => client.PutAsJsonAsync(FormRoute(apiRoute, route, resourceId), request),
			ReadJsonContentAsync<TResponse?>,
			logger) ?? default;

	static string FormRoute(string apiRoute, string baseRoute, UniqueObjectId? resourceId)
		=> resourceId == null
			? apiRoute + baseRoute
			: apiRoute + baseRoute + $"/{resourceId}";

	public static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, Func<Task<HttpResponseMessage>> httpFunc, Func<HttpContent, Task<T?>>? contentReaderFunc = null, ILogger? logger = null)
	{
		try
		{
			if (!Uri.TryCreate(client.BaseAddress, route, out var uri))
			{
				logger?.LogError("Unable to create a URI from base=\"{BaseAddress}\" and route=\"{Route}\"", client.BaseAddress, route);
				return default;
			}

			logger?.LogDebug("Sending to {Uri}", uri);
			using var response = await httpFunc();

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(error))
				{
					logger?.LogError("Failed. StatusCode={StatusCode} ReasonPhrase={ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
				}
				else
				{
					logger?.LogError("Failed. Error={Error}", error);
				}

				return default;
			}

			logger?.LogDebug("Received success response: {StatusCode}", response.StatusCode);

			if (contentReaderFunc != null)
			{
				var data = await contentReaderFunc(response.Content);
				if (data == null)
				{
					logger?.LogError("Received data but couldn't parse it: {Response}", response);
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
			logger?.LogError(ex);
			return default;
		}
	}
}
