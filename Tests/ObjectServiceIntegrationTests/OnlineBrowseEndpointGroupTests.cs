using Definitions.Database;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class OnlineBrowseEndpointGroupTests
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
	public async Task GetListAsync_ReturnsObjectPacksFromConfiguredEndpointGroup()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
		await db.ObjectPacks.AddRangeAsync(
		[
			new TblObjectPack { Id = 1, Name = "UK renewal set", Description = "British rolling stock" },
			new TblObjectPack { Id = 2, Name = "Swiss rails", Description = "Swiss rolling stock" },
		]);
		_ = await db.SaveChangesAsync();

		var results = await Client.GetListAsync<DtoItemPackEntry>(httpClient!, Client.ObjectPacksEndpointGroup);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(results.Select(x => x.Name), Is.EqualTo(new[] { "Swiss rails", "UK renewal set" }));
			Assert.That(results.All(x => x.Id > 0), Is.True);
		}
	}

	[Test]
	public async Task GetListAsync_ReturnsScenariosFromConfiguredEndpointGroup()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var sfm = scope.ServiceProvider.GetRequiredService<ServerFolderManager>();
		var customFolder = Path.Combine(sfm.ScenariosFolder, ServerFolderManager.CustomFolderName);

		await File.WriteAllTextAsync(Path.Combine(customFolder, "alpine.SC5"), "scenario");
		await File.WriteAllTextAsync(Path.Combine(customFolder, "desert.SC5"), "scenario");

		var results = await Client.GetListAsync<DtoScenarioEntry>(httpClient!, Client.ScenariosEndpointGroup);

		Assert.That(results.Select(x => x.Name), Is.EqualTo(new[]
		{
			Path.Combine(ServerFolderManager.CustomFolderName, "alpine.SC5"),
			Path.Combine(ServerFolderManager.CustomFolderName, "desert.SC5"),
		}));
	}
}
