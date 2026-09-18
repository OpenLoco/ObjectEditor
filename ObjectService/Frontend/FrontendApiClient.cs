using System.Net.Http.Headers;

namespace ObjectService.Frontend;

/// <summary>
/// Creates <see cref="HttpClient"/> instances that talk to this application's own public API
/// (<c>/v2/...</c>). The base address is derived from the current request so the client works
/// regardless of host/port, and the bearer token stored in the <c>access_token</c> cookie is
/// forwarded so authenticated API calls succeed.
/// <para>
/// All frontend server-side code should use this client rather than reaching into the database
/// or the backend services directly.
/// </para>
/// </summary>
public sealed class FrontendApiClient
{
	readonly IHttpClientFactory _httpClientFactory;
	readonly IHttpContextAccessor _httpContextAccessor;

	public FrontendApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;
		_httpContextAccessor = httpContextAccessor;
	}

	public HttpClient CreateClient()
	{
		var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("An active HTTP request is required to create the ObjectService API client.");
		var client = _httpClientFactory.CreateClient();
		client.BaseAddress = new Uri($"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/");

		// Forward the bearer token from the access_token cookie to API calls.
		var accessToken = httpContext.Request.Cookies["access_token"];
		if (!string.IsNullOrEmpty(accessToken))
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		}

		return client;
	}
}
