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
				Directory.CreateDirectory(directory);
			}

			await File.WriteAllBytesAsync(fullPath, bytes);
		}
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
	public async Task GetScenarioFileAsync_ReturnsFileMatchingSortedListOrder()
	{
		var list = (await Client.GetScenariosAsync(HttpClient!)).ToList();
		var firstScenario = list.First();

		using var response = await HttpClient!.GetAsync($"{RoutesV2.Prefix}{BaseRoute}/{firstScenario.Id}{RoutesV2.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(firstScenario.Name, Is.EqualTo(Path.Combine(ServerFolderManager.CustomFolderName, "alpha.SC5")));
			Assert.That(bytes, Is.EqualTo(new byte[] { 1, 2, 3 }));
		}
	}
}
