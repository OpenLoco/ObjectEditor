using Common;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService;
using ObjectService.Tests.Integration;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using UniqueObjectId = System.UInt64;

namespace Tests.ObjectServiceIntegrationTests.Routes;

[TestFixture]
public class ScenarioPackRoutesTests : BaseRouteHandlerTestFixture
{
	const UniqueObjectId AlphaPackId = 1;

	public override string BaseRoute
		=> Definitions.Web.Routes.ScenarioPacks;

	protected override async Task SeedDataCoreAsync(LocoDbContext db)
	{
		using var scope = testWebAppFactory.Services.CreateScope();
		var sp = scope.ServiceProvider;
		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var config = sp.GetRequiredService<IConfiguration>();
		var rootFolder = config["ObjectService:RootFolder"];
		ArgumentNullException.ThrowIfNull(rootFolder);

		var safeRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "pack-safe.SC5");
		var secondSafeRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "pack-extra.SC5");
		await File.WriteAllBytesAsync(Path.Combine(sfm.ScenariosFolder, safeRelativePath), [1, 2, 3, 4]);
		await File.WriteAllBytesAsync(Path.Combine(sfm.ScenariosFolder, secondSafeRelativePath), [4, 3, 2, 1]);

		await File.WriteAllBytesAsync(Path.Combine(rootFolder, "outside.SC5"), [7, 7, 7]);
		await File.WriteAllBytesAsync(Path.Combine(sfm.ScenariosFolder, @"..\outside-windows.SC5"), [6, 6, 6]);

		await db.ScenarioPacks.AddRangeAsync(
		[
			new TblScenarioPack
			{
				Id = AlphaPackId,
				Name = "Alpha Scenario Pack",
				Description = "Safe and unsafe scenarios",
				Scenarios =
				[
					new TblScenario { Id = 1, Name = safeRelativePath },
					new TblScenario { Id = 2, Name = Path.Combine("..", "outside.SC5") },
					new TblScenario { Id = 3, Name = @"..\outside-windows.SC5" },
				],
			},
			new TblScenarioPack
			{
				Id = 2,
				Name = "Zulu Scenario Pack",
				Description = "Only safe scenarios",
				Scenarios =
				[
					new TblScenario { Id = 4, Name = secondSafeRelativePath },
				],
			},
		]);
	}

	[Test]
	public override async Task ListAsync()
	{
		var results = (await Client.GetScenarioPacksAsync(HttpClient!)).ToList();
		using var db = GetDbContext();
		var expectedRows = await db.ScenarioPacks
			.Include(x => x.Licence)
			.OrderBy(x => x.Name)
			.ToListAsync();
		var expected = expectedRows
			.Select(x => new DtoItemPackEntry(x.Id, x.Name, x.Description, x.CreatedDate, x.ModifiedDate, x.UploadedDate, x.Licence?.ToDtoEntry()))
			.ToList();

		Assert.That(results, Is.EqualTo(expected));
	}

	[Test]
	public override async Task PostAsync()
	{
		var request = new DtoItemPackDescriptor<DtoScenarioEntry>(
			0,
			"New scenario pack",
			null,
			null,
			null,
			DateOnly.UtcToday,
			[],
			[],
			[],
			null);

		using var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}", request);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
	}

	[Test]
	public override async Task GetAsync()
	{
		var result = await Client.GetScenarioPackAsync(HttpClient!, AlphaPackId);
		using var db = GetDbContext();
		var expected = (await db.ScenarioPacks
			.Include(x => x.Scenarios)
			.Include(x => x.Authors)
			.Include(x => x.Tags)
			.Include(x => x.Licence)
			.SingleAsync(x => x.Id == AlphaPackId))
			.ToDtoEntry();

		AssertPackDescriptorEqual(result, expected);
	}

	[Test]
	public override async Task PutAsync()
	{
		var request = new DtoScenarioPackDescriptor(
			AlphaPackId,
			"Updated scenario pack",
			"Updated description",
			null,
			null,
			DateOnly.UtcToday,
			null,
			[],
			[],
			[]);

		using var response = await HttpClient!.PutAsJsonAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/{AlphaPackId}", request);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

			using var db = GetDbContext();
			var updated = await db.ScenarioPacks.SingleAsync(x => x.Id == AlphaPackId);
			Assert.That(updated.Name, Is.EqualTo("Updated scenario pack"));
			Assert.That(updated.Description, Is.EqualTo("Updated description"));
		}
	}

	[Test]
	public override async Task DeleteAsync()
	{
		using var response = await HttpClient!.DeleteAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/{AlphaPackId}");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

			using var db = GetDbContext();
			Assert.That(await db.ScenarioPacks.AnyAsync(x => x.Id == AlphaPackId), Is.False);
		}
	}

	[Test]
	public async Task GetScenarioPackFileAsync_ReturnsZipWithOnlySafeScenarioEntries()
	{
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/{AlphaPackId}{Definitions.Web.Routes.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();
		using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/zip"));
			Assert.That(archive.Entries.Select(x => x.FullName), Is.EqualTo([Path.Combine(ServerFolderManager.CustomFolderName, "pack-safe.SC5").Replace('\\', '/')]));
			await using var entryStream = archive.Entries.Single().Open();
			using var entryMemoryStream = new MemoryStream();
			await entryStream.CopyToAsync(entryMemoryStream);
			Assert.That(entryMemoryStream.ToArray(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
		}
	}

	[Test]
	public async Task GetScenarioPackAsync_WithUnknownId_ReturnsNotFound()
	{
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/9999");

		Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
	}
	static void AssertPackDescriptorEqual(DtoItemPackDescriptor<DtoScenarioEntry>? actual, DtoItemPackDescriptor<DtoScenarioEntry> expected)
	{
		Assert.That(actual, Is.Not.Null);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(actual!.Id, Is.EqualTo(expected.Id));
			Assert.That(actual.Name, Is.EqualTo(expected.Name));
			Assert.That(actual.Description, Is.EqualTo(expected.Description));
			Assert.That(actual.CreatedDate, Is.EqualTo(expected.CreatedDate));
			Assert.That(actual.ModifiedDate, Is.EqualTo(expected.ModifiedDate));
			Assert.That(actual.UploadedDate, Is.EqualTo(expected.UploadedDate));
			Assert.That(actual.Items, Is.EquivalentTo(expected.Items));
			Assert.That(actual.Authors, Is.EquivalentTo(expected.Authors));
			Assert.That(actual.Tags, Is.EquivalentTo(expected.Tags));
			Assert.That(actual.Licence, Is.EqualTo(expected.Licence));
		}
	}
}
