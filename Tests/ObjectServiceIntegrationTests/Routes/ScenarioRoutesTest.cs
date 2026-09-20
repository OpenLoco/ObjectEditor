using Common;
using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService;
using ObjectService.Tests.Integration;
using System.Net;
using System.Net.Http.Json;

namespace Tests.ObjectServiceIntegrationTests.Routes;

/// <summary>
/// Integration tests for the merged, database-backed scenario routes (<c>/v2/scenarios</c>).
/// Scenario metadata lives in the database while the files themselves live on disk.
/// </summary>
[TestFixture]
public class ScenarioRoutesTest : BaseRouteHandlerTestFixture
{
	const string Custom = ServerFolderManager.CustomFolderName;

	readonly (string RelativePath, byte[] Bytes)[] scenarios =
	[
		(Path.Combine(Custom, "zulu.SC5"), [9, 9, 9]),
		(Path.Combine(Custom, "alpha.SC5"), [1, 2, 3]),
	];

	public override string BaseRoute
		=> Definitions.Web.Routes.Scenarios;

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

		// Metadata rows; the file download route resolves these names under the Scenarios folder.
		await db.Scenarios.AddRangeAsync(
		[
			new TblScenario { Id = 1, Name = Path.Combine(Custom, "zulu.SC5") },
			new TblScenario { Id = 2, Name = Path.Combine(Custom, "alpha.SC5") },
		]);
	}

	[Test]
	public override async Task ListAsync()
	{
		var results = (await Client.GetScenariosAsync(HttpClient!)).ToList();

		using (Assert.EnterMultipleScope())
		{
			// ordered by name: alpha (id 2), zulu (id 1)
			Assert.That(results.Select(x => x.Id), Is.EqualTo([2UL, 1UL]));
			Assert.That(results.Select(x => x.Name), Is.EqualTo(
			[
				Path.Combine(Custom, "alpha.SC5"),
				Path.Combine(Custom, "zulu.SC5"),
			]));
		}
	}

	[Test]
	public override async Task PostAsync()
	{
		using var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}", new { });

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
	}

	[Test]
	public override async Task GetAsync()
	{
		var result = await Client.GetScenarioAsync(HttpClient!, 1);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Id, Is.EqualTo(1));
			Assert.That(result.Name, Is.EqualTo(Path.Combine(Custom, "zulu.SC5")));
			Assert.That(result.ObjectSource, Is.EqualTo(ObjectSource.Custom));
		}
	}

	[Test]
	public override async Task PutAsync()
	{
		var request = new DtoScenarioDescriptor(
			1,
			Path.Combine(Custom, "updated.SC5"),
			"updated description",
			ObjectSource.OpenLoco,
			null,
			null,
			DateOnly.UtcToday,
			null,
			[],
			[],
			[]);

		using var response = await HttpClient!.PutAsJsonAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/1", request);
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		using var db = GetDbContext();
		var updated = await db.Scenarios.SingleAsync(x => x.Id == 1);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(updated.Name, Is.EqualTo(Path.Combine(Custom, "updated.SC5")));
			Assert.That(updated.Description, Is.EqualTo("updated description"));
			Assert.That(updated.ObjectSource, Is.EqualTo(ObjectSource.OpenLoco));
		}
	}

	[Test]
	public override async Task DeleteAsync()
	{
		using var response = await HttpClient!.DeleteAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/1");
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		using var db = GetDbContext();
		Assert.That(await db.Scenarios.AnyAsync(x => x.Id == 1), Is.False);
	}

	[Test]
	public async Task GetScenarioFileAsync_ReturnsFileForDatabaseId()
	{
		// Scenario id 2 points at Custom/alpha.SC5.
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/2{Definitions.Web.Routes.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/octet-stream"));
			Assert.That(bytes, Is.EqualTo(new byte[] { 1, 2, 3 }));
		}
	}

	[Test]
	public async Task GetScenarioFileAsync_ReturnsNotFound_WhenFileIsMissingFromDisk()
	{
		using var db = GetDbContext();
		var scenario = await db.Scenarios.SingleAsync(x => x.Id == 1);
		scenario.Name = Path.Combine(Custom, "does-not-exist.SC5");
		_ = await db.SaveChangesAsync();

		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/1{Definitions.Web.Routes.File}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}
}
