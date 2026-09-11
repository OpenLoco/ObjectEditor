using Definitions.Database;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService;
using ObjectService.Tests.Integration;
using System.Net;
using System.Net.Http.Json;

namespace Tests.ObjectServiceIntegrationTests.Routes;

[TestFixture]
public class ScenarioRoutesTest : BaseRouteHandlerTestFixture
{
	readonly (string RelativePath, byte[] Bytes)[] scenarios =
	[
		(Path.Combine(ServerFolderManager.CustomFolderName, "zulu.SC5"), [9, 9, 9]),
		(Path.Combine(ServerFolderManager.CustomFolderName, "alpha.SC5"), [1, 2, 3]),
	];

	public override string BaseRoute
		=> RoutesV2.Scenarios;

	protected override async Task SeedDataCoreAsync(LocoDbContext db)
	{
		using var scope = testWebAppFactory.Services.CreateScope();
		var sfm = scope.ServiceProvider.GetRequiredService<ServerFolderManager>();

		foreach (var (relativePath, bytes) in scenarios)
		{
			var fullPath = Path.Combine(sfm.ScenariosFolder, relativePath);
			var directory = Path.GetDirectoryName(fullPath);
			if (!string.IsNullOrEmpty(directory))
			{
				_ = Directory.CreateDirectory(directory);
			}

			await File.WriteAllBytesAsync(fullPath, bytes);
		}

		// The download endpoint resolves scenarios by their database id, so seed a
		// DB row that points at one of the files written above.
		await db.SC5Files.AddAsync(new TblSC5File { Id = 1, Name = Path.Combine(ServerFolderManager.CustomFolderName, "alpha.SC5") });
	}

	[Test]
	public override async Task ListAsync()
	{
		var results = (await Client.GetScenariosAsync(HttpClient!)).ToList();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(results.Select(x => x.Id), Is.EqualTo([0UL, 1UL]));
			Assert.That(results.Select(x => x.Name), Is.EqualTo(scenarios.Select(x => x.RelativePath).OrderBy(x => x, StringComparer.Ordinal)));
		}
	}

	[Test]
	public override async Task PostAsync()
	{
		using var response = await HttpClient!.PostAsJsonAsync($"{RoutesV2.Prefix}{BaseRoute}", new DtoScenarioEntry(0, "new-scenario.SC5"));

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
	}

	[Test]
	public override async Task GetAsync()
	{
		using var response = await HttpClient!.GetAsync($"{RoutesV2.Prefix}{BaseRoute}/0");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
	}

	[Test]
	public override async Task PutAsync()
	{
		using var response = await HttpClient!.PutAsJsonAsync($"{RoutesV2.Prefix}{BaseRoute}/0", new DtoScenarioEntry(0, "updated-scenario.SC5"));

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
	}

	[Test]
	public override async Task DeleteAsync()
	{
		using var response = await HttpClient!.DeleteAsync($"{RoutesV2.Prefix}{BaseRoute}/0");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
	}

	[Test]
	public async Task GetScenarioFileAsync_ReturnsFileForDatabaseId()
	{
		// Scenario id 1 is seeded in SeedDataCoreAsync and points at Custom/alpha.SC5.
		using var response = await HttpClient!.GetAsync($"{RoutesV2.Prefix}{BaseRoute}/1{RoutesV2.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/octet-stream"));
			Assert.That(bytes, Is.EqualTo(new byte[] { 1, 2, 3 }));
		}
	}
}
