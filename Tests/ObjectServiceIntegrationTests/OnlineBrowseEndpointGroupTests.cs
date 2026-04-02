using Definitions.Database;
using Definitions.DTO;
using Definitions.Web;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

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
			Assert.That(results.Select(x => x.Name), Is.EqualTo(["Swiss rails", "UK renewal set"]));
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

		Assert.That(results.Select(x => x.Name), Is.EqualTo(
		[
			Path.Combine(ServerFolderManager.CustomFolderName, "alpine.SC5"),
			Path.Combine(ServerFolderManager.CustomFolderName, "desert.SC5"),
		]));
	}

	[Test]
	public async Task GetListAsync_ReturnsAuthorsTagsAndLicencesFromConfiguredEndpointGroups()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
		await db.Authors.AddRangeAsync(
		[
			new TblAuthor { Id = 1, Name = "Alice" },
			new TblAuthor { Id = 2, Name = "Bob" },
		]);
		await db.Tags.AddRangeAsync(
		[
			new TblTag { Id = 1, Name = "Industrial" },
			new TblTag { Id = 2, Name = "Passenger" },
		]);
		await db.Licences.AddRangeAsync(
		[
			new TblLicence { Id = 1, Name = "CC-BY", Text = "Creative Commons" },
			new TblLicence { Id = 2, Name = "GPL", Text = "GNU GPL" },
		]);
		_ = await db.SaveChangesAsync();

		var authors = await Client.GetListAsync<DtoAuthorEntry>(httpClient!, Client.AuthorsEndpointGroup);
		var tags = await Client.GetListAsync<DtoTagEntry>(httpClient!, Client.TagsEndpointGroup);
		var licences = await Client.GetListAsync<DtoLicenceEntry>(httpClient!, Client.LicencesEndpointGroup);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(authors.Select(x => x.Name), Is.EqualTo(["Alice", "Bob"]));
			Assert.That(tags.Select(x => x.Name), Is.EqualTo(["Industrial", "Passenger"]));
			Assert.That(licences.Select(x => x.Name), Is.EqualTo(["CC-BY", "GPL"]));
		}
	}

	[Test]
	public async Task GetListAsync_ReturnsMissingObjectsFromConfiguredEndpointGroup()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
		await db.ObjectsMissing.AddRangeAsync(
		[
			new TblObjectMissing { Id = 1, DatName = "AIRPORTX", DatChecksum = 123, ObjectType = Definitions.ObjectModels.Types.ObjectType.Airport },
			new TblObjectMissing { Id = 2, DatName = "ROADY", DatChecksum = 456, ObjectType = Definitions.ObjectModels.Types.ObjectType.RoadExtra },
		]);
		_ = await db.SaveChangesAsync();

		var results = await Client.GetListAsync<DtoObjectMissingEntry>(httpClient!, Client.MissingObjectsEndpointGroup);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(results.Select(x => x.DatName), Is.EqualTo(["AIRPORTX", "ROADY"]));
			Assert.That(results.Select(x => x.DatChecksum), Is.EqualTo(new uint[] { 123, 456 }));
		}
	}

	[Test]
	public async Task GetListAsync_ReturnsSC5FilePacksFromConfiguredEndpointGroup()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
		await db.SC5FilePacks.AddRangeAsync(
		[
			new TblSC5FilePack { Id = 1, Name = "Challenge Pack", Description = "Hard scenarios" },
			new TblSC5FilePack { Id = 2, Name = "Starter Pack", Description = "Easy scenarios" },
		]);
		_ = await db.SaveChangesAsync();

		var results = await Client.GetListAsync<DtoItemPackEntry>(httpClient!, Client.SC5FilePacksEndpointGroup);

		Assert.That(results.Select(x => x.Name), Is.EqualTo(["Challenge Pack", "Starter Pack"]));
	}
}
