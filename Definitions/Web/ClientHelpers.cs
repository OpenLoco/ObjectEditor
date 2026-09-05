using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Definitions.Web;

public static class ClientHelpers
{
	public static async Task<byte[]?> ReadBinaryContentAsync(HttpContent content, CancellationToken cancellationToken = default)
	{
		await using (var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
		await using (var memoryStream = new MemoryStream())
		{
			await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
			return memoryStream.ToArray();
		}
	}

	internal static async Task<T?> ReadJsonContentAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
		=> await content.ReadFromJsonAsync<T?>(cancellationToken).ConfigureAwait(false);

	public static async Task<T?> GetAsync<T>(HttpClient client, string apiRoute, string route, UniqueObjectId? resourceId = null, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await SendRequestAsync(
			client,
			FormRoute(apiRoute, route, resourceId),
			ct => client.GetAsync(FormRoute(apiRoute, route, resourceId), ct),
			ReadJsonContentAsync<T?>,
			logger,
			cancellationToken).ConfigureAwait(false) ?? default;

	public static async Task<bool> DeleteAsync(HttpClient client, string apiRoute, string route, UniqueObjectId resourceId, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await SendRequestAsync<bool?>(
			client,
			FormRoute(apiRoute, route, resourceId),
			ct => client.DeleteAsync(FormRoute(apiRoute, route, resourceId), ct),
			null,
			logger,
			cancellationToken).ConfigureAwait(false) != null;

	public static async Task<TResponse?> PostAsync<TRequest, TResponse>(HttpClient client, string apiRoute, string route, TRequest request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await SendRequestAsync(
			client,
			FormRoute(apiRoute, route, null),
			ct => client.PostAsJsonAsync(FormRoute(apiRoute, route, null), request, ct),
			ReadJsonContentAsync<TResponse?>,
			logger,
			cancellationToken).ConfigureAwait(false) ?? default;

	public static async Task<TResponse?> PutAsync<TRequest, TResponse>(HttpClient client, string apiRoute, string route, UniqueObjectId resourceId, TRequest request, ILogger? logger = null, CancellationToken cancellationToken = default)
		=> await SendRequestAsync(
			client,
			FormRoute(apiRoute, route, resourceId),
			ct => client.PutAsJsonAsync(FormRoute(apiRoute, route, resourceId), request, ct),
			ReadJsonContentAsync<TResponse?>,
			logger,
			cancellationToken).ConfigureAwait(false) ?? default;

	static string FormRoute(string apiRoute, string baseRoute, UniqueObjectId? resourceId)
		=> resourceId == null
			? apiRoute + baseRoute
			: apiRoute + baseRoute + $"/{resourceId}";

	public static async Task<T?> SendRequestAsync<T>(HttpClient client, string route, Func<CancellationToken, Task<HttpResponseMessage>> httpFunc, Func<HttpContent, CancellationToken, Task<T?>>? contentReaderFunc = null, ILogger? logger = null, CancellationToken cancellationToken = default)
	{
		try
		{
			if (!Uri.TryCreate(client.BaseAddress, route, out var uri))
			{
				logger?.LogError("Unable to create a URI from base=\"{BaseAddress}\" and route=\"{Route}\"", client.BaseAddress, route);
				return default;
			}

			logger?.LogDebug("Sending to {Uri}", uri);
			using var response = await httpFunc(cancellationToken).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

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
				var data = await contentReaderFunc(response.Content, cancellationToken).ConfigureAwait(false);
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
		catch (HttpRequestException ex)
		{
			logger?.LogError(ex, "HTTP request failed.");
			return default;
		}
		catch (TaskCanceledException ex)
		{
			logger?.LogDebug("Request was cancelled: {Message}", ex.Message);
			return default;
		}
		catch (JsonException ex)
		{
			logger?.LogError(ex, "Failed to deserialize response.");
			return default;
		}
		catch (InvalidOperationException ex)
		{
			logger?.LogError(ex, "Invalid operation during request.");
			return default;
		}
	}
}
