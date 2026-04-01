using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Index;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.IO.Compression;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class DownloadRoutesTest
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
	public async Task GetScenarioFileAsync_ReturnsFileMatchingSortedListOrder()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var sfm = scope.ServiceProvider.GetRequiredService<ServerFolderManager>();

		var alphaRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "alpha.SC5");
		var zuluRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "zulu.SC5");
		var alphaPath = Path.Combine(sfm.ScenariosFolder, alphaRelativePath);
		var zuluPath = Path.Combine(sfm.ScenariosFolder, zuluRelativePath);

		await File.WriteAllBytesAsync(zuluPath, [9, 9, 9]);
		await File.WriteAllBytesAsync(alphaPath, [1, 2, 3]);

		var list = await Client.GetListAsync<Definitions.DTO.DtoScenarioEntry>(httpClient!, Client.ScenariosEndpointGroup);
		var firstScenario = list.First();

		using var response = await httpClient!.GetAsync($"{RoutesV2.Prefix}{RoutesV2.Scenarios}/{firstScenario.Id}{RoutesV2.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(firstScenario.Name, Is.EqualTo(alphaRelativePath));
			Assert.That(bytes, Is.EqualTo(new byte[] { 1, 2, 3 }));
		}
	}

	[Test]
	public async Task GetSC5FilePackFileAsync_ReturnsZipWithOnlySafeScenarioEntries()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var sp = scope.ServiceProvider;
		var db = sp.GetRequiredService<LocoDbContext>();
		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var config = sp.GetRequiredService<IConfiguration>();
		var rootFolder = config["ObjectService:RootFolder"];
		ArgumentNullException.ThrowIfNull(rootFolder);

		var safeRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "pack-safe.SC5");
		var safePath = Path.Combine(sfm.ScenariosFolder, safeRelativePath);
		await File.WriteAllBytesAsync(safePath, [1, 2, 3, 4]);

		var outsidePath = Path.Combine(rootFolder, "outside.SC5");
		await File.WriteAllBytesAsync(outsidePath, [7, 7, 7]);
		await File.WriteAllBytesAsync(Path.Combine(sfm.ScenariosFolder, @"..\outside-windows.SC5"), [6, 6, 6]);

		var pack = new TblSC5FilePack
		{
			Id = 1,
			Name = "Scenario/Pack\r\nbad",
			SC5Files =
			[
				new TblSC5File { Id = 1, Name = safeRelativePath },
				new TblSC5File { Id = 2, Name = Path.Combine("..", "outside.SC5") },
				new TblSC5File { Id = 3, Name = @"..\outside-windows.SC5" },
			],
		};

		_ = await db.SC5FilePacks.AddAsync(pack);
		_ = await db.SaveChangesAsync();

		using var response = await httpClient!.GetAsync($"{RoutesV2.Prefix}{RoutesV2.SC5FilePacks}/{pack.Id}{RoutesV2.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();
		using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/zip"));
			Assert.That(response.Content.Headers.ContentDisposition?.FileName?.Trim('"'), Is.EqualTo("Scenario_Pack__bad.zip"));
			Assert.That(archive.Entries.Select(x => x.FullName), Is.EqualTo(new[] { safeRelativePath.Replace('\\', '/') }));
			await using var entryStream = archive.Entries.Single().Open();
			using var entryMemoryStream = new MemoryStream();
			await entryStream.CopyToAsync(entryMemoryStream);
			Assert.That(entryMemoryStream.ToArray(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
		}
	}

	[Test]
	public async Task GetObjectPackFileAsync_ReturnsZipWithOnlySafeIndexedObjectEntries()
	{
		using var scope = testWebAppFactory!.Services.CreateScope();
		var sp = scope.ServiceProvider;
		var db = sp.GetRequiredService<LocoDbContext>();
		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var config = sp.GetRequiredService<IConfiguration>();
		var rootFolder = config["ObjectService:RootFolder"];
		ArgumentNullException.ThrowIfNull(rootFolder);

		var safeRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "safe-object.dat");
		var safePath = Path.Combine(sfm.ObjectsFolder, safeRelativePath);
		await File.WriteAllBytesAsync(safePath, [4, 3, 2, 1]);

		var outsidePath = Path.Combine(rootFolder, "outside.dat");
		await File.WriteAllBytesAsync(outsidePath, [8, 8, 8]);
		await File.WriteAllBytesAsync(Path.Combine(sfm.ObjectsFolder, @"..\outside-windows.dat"), [5, 5, 5]);

		sfm.ObjectIndex.Objects.Add(new ObjectIndexEntry("SAFEOBJ", safeRelativePath, null, 111, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));
		sfm.ObjectIndex.Objects.Add(new ObjectIndexEntry("UNSAFEOBJ", Path.Combine("..", "outside.dat"), null, 222, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));
		sfm.ObjectIndex.Objects.Add(new ObjectIndexEntry("WINDOWSOBJ", @"..\outside-windows.dat", null, 333, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));

		var obj = new TblObject
		{
			Id = 1,
			Name = "safe-obj",
			SubObjectId = 1,
			ObjectType = ObjectType.Vehicle,
			ObjectSource = ObjectSource.Custom,
			Availability = ObjectAvailability.Available,
			DatObjects =
			[
				new TblDatObject { Id = 1, DatName = "SAFEOBJ", DatChecksum = 111, xxHash3 = 1, ObjectId = 1 },
				new TblDatObject { Id = 2, DatName = "UNSAFEOBJ", DatChecksum = 222, xxHash3 = 2, ObjectId = 1 },
				new TblDatObject { Id = 3, DatName = "WINDOWSOBJ", DatChecksum = 333, xxHash3 = 3, ObjectId = 1 },
			],
		};

		var pack = new TblObjectPack
		{
			Id = 1,
			Name = "Object\\Pack:unsafe",
			Objects = [obj],
		};

		_ = await db.ObjectPacks.AddAsync(pack);
		_ = await db.SaveChangesAsync();

		using var response = await httpClient!.GetAsync($"{RoutesV2.Prefix}{RoutesV2.ObjectPacks}/{pack.Id}{RoutesV2.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();
		using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/zip"));
			Assert.That(response.Content.Headers.ContentDisposition?.FileName?.Trim('"'), Is.EqualTo("Object_Pack_unsafe.zip"));
			Assert.That(archive.Entries.Select(x => x.FullName), Is.EqualTo(new[] { safeRelativePath.Replace('\\', '/') }));
			await using var entryStream = archive.Entries.Single().Open();
			using var entryMemoryStream = new MemoryStream();
			await entryStream.CopyToAsync(entryMemoryStream);
			Assert.That(entryMemoryStream.ToArray(), Is.EqualTo(new byte[] { 4, 3, 2, 1 }));
		}
	}
}
