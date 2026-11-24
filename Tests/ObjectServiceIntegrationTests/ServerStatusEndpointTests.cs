using Definitions.Web;
using NUnit.Framework;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class ServerStatusEndpointTests
{
	HttpClient? httpClient;
	TestWebApplicationFactory<Program>? testWebAppFactory;

	[SetUp]
	public void SetUp()
	{
		testWebAppFactory = new TestWebApplicationFactory<Program>();
		httpClient = testWebAppFactory.CreateClient();
	}

	[TearDown]
	public void TearDown()
	{
		httpClient?.Dispose();
		testWebAppFactory?.Dispose();
	}

	[Test]
	public async Task GetServerStatusAsync_WhenWritable_ReportsNotReadOnly()
	{
		var status = await Client.GetServerStatusAsync(httpClient!);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(status, Is.Not.Null);
			Assert.That(status!.FrontendReadOnly, Is.False);
			Assert.That(status.BackendReadOnly, Is.False);
			Assert.That(status.IsReadOnly, Is.False);
		}
	}

	[Test]
	public async Task GetServerStatusAsync_WhenBackendReadOnly_ReportsReadOnly()
	{
		using var readOnlyFactory = new ReadOnlyTestWebApplicationFactory();
		using var readOnlyClient = readOnlyFactory.CreateClient();

		var status = await Client.GetServerStatusAsync(readOnlyClient);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(status, Is.Not.Null);
			Assert.That(status!.BackendReadOnly, Is.True);
			Assert.That(status.IsReadOnly, Is.True);
		}
	}

	[Test]
	public async Task ServerStatusRoute_IsPubliclyAccessible()
	{
		using var response = await httpClient!.GetAsync(Client.ApiVersion + Routes.Server + Routes.Status);

		Assert.That(response.IsSuccessStatusCode, Is.True);
	}

	sealed class ReadOnlyTestWebApplicationFactory : TestWebApplicationFactory<Program>
	{
		protected override bool BackendReadOnly => true;
	}
}
