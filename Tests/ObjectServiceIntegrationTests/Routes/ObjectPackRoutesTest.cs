using Common;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Index;
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
public class ObjectPackRoutesTest : BaseRouteHandlerTestFixture
{
	const UniqueObjectId AlphaPackId = 1;

	public override string BaseRoute
		=> Definitions.Web.Routes.ObjectPacks;

	protected override async Task SeedDataCoreAsync(LocoDbContext db)
	{
		using var scope = testWebAppFactory.Services.CreateScope();
		var sp = scope.ServiceProvider;
		var sfm = sp.GetRequiredService<ServerFolderManager>();
		var config = sp.GetRequiredService<IConfiguration>();
		var rootFolder = config["ObjectService:RootFolder"];
		ArgumentNullException.ThrowIfNull(rootFolder);

		var safeRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "safe-object.dat");
		var secondSafeRelativePath = Path.Combine(ServerFolderManager.CustomFolderName, "safe-object-2.dat");
		await File.WriteAllBytesAsync(Path.Combine(sfm.ObjectsFolder, safeRelativePath), [4, 3, 2, 1]);
		await File.WriteAllBytesAsync(Path.Combine(sfm.ObjectsFolder, secondSafeRelativePath), [1, 1, 2, 2]);

		await File.WriteAllBytesAsync(Path.Combine(rootFolder, "outside.dat"), [8, 8, 8]);
		await File.WriteAllBytesAsync(Path.Combine(sfm.ObjectsFolder, @"..\outside-windows.dat"), [5, 5, 5]);

		sfm.ObjectIndex.AddEntry(new ObjectIndexEntry("SAFEOBJ", safeRelativePath, null, 111, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));
		sfm.ObjectIndex.AddEntry(new ObjectIndexEntry("UNSAFEOBJ", Path.Combine("..", "outside.dat"), null, 222, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));
		sfm.ObjectIndex.AddEntry(new ObjectIndexEntry("WINDOWSOBJ", @"..\outside-windows.dat", null, 333, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));
		sfm.ObjectIndex.AddEntry(new ObjectIndexEntry("SAFEOBJ2", secondSafeRelativePath, null, 444, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));

		await db.ObjectPacks.AddRangeAsync(
		[
			new TblObjectPack
			{
				Id = AlphaPackId,
				Name = "Alpha Object Pack",
				Description = "Safe and unsafe objects",
				Objects =
				[
					new TblObject
					{
						Id = 1,
						Name = "safe-obj",
						ObjectType = ObjectType.Vehicle,
						ObjectSource = ObjectSource.Custom,
						Availability = ObjectAvailability.Available,
						DatObjects =
						[
							new TblDatObject { Id = 1, DatName = "SAFEOBJ", DatChecksum = 111, xxHash3 = 1, ObjectId = 1 },
							new TblDatObject { Id = 2, DatName = "UNSAFEOBJ", DatChecksum = 222, xxHash3 = 2, ObjectId = 1 },
							new TblDatObject { Id = 3, DatName = "WINDOWSOBJ", DatChecksum = 333, xxHash3 = 3, ObjectId = 1 },
						],
					},
				],
			},
			new TblObjectPack
			{
				Id = 2,
				Name = "Zulu Object Pack",
				Description = "Only safe objects",
				Objects =
				[
					new TblObject
					{
						Id = 2,
						Name = "safe-obj-2",
						ObjectType = ObjectType.Vehicle,
						ObjectSource = ObjectSource.Custom,
						Availability = ObjectAvailability.Available,
						DatObjects =
						[
							new TblDatObject { Id = 4, DatName = "SAFEOBJ2", DatChecksum = 444, xxHash3 = 4, ObjectId = 2 },
						],
					},
				],
			},
		]);
	}

	[Test]
	public override async Task ListAsync()
	{
		var results = (await Client.GetObjectPacksAsync(HttpClient!)).ToList();
		using var db = GetDbContext();
		var expectedRows = await db.ObjectPacks
			.Include(x => x.Licence)
			.OrderBy(x => x.Name)
			.ToListAsync();
		var expected = expectedRows.Select(x => x.ToDtoEntry()).ToList();

		Assert.That(results, Is.EqualTo(expected));
	}

	[Test]
	public override async Task PostAsync()
	{
		var request = new DtoItemPackDescriptor<DtoObjectEntry>(
			0,
			"New object pack",
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
	public async Task PostAsync_WithEmptyName_ReturnsBadRequest()
	{
		var request = new DtoItemPackDescriptor<DtoObjectEntry>(
			0, "   ", null, null, null, DateOnly.UtcToday, [], [], [], null);

		using var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}", request);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
	}

	[Test]
	public async Task PostAsync_WithDuplicateName_ReturnsConflict()
	{
		// "Alpha Object Pack" is seeded by SeedDataCoreAsync.
		var request = new DtoItemPackDescriptor<DtoObjectEntry>(
			0, "Alpha Object Pack", null, null, null, DateOnly.UtcToday, [], [], [], null);

		using var response = await HttpClient!.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}", request);

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
	}

	[Test]
	public override async Task GetAsync()
	{
		var result = await Client.GetObjectPackAsync(HttpClient!, AlphaPackId);
		using var db = GetDbContext();
		var expected = (await db.ObjectPacks
			.Include(x => x.Objects)
			.Include(x => x.Authors)
			.Include(x => x.Tags)
			.Include(x => x.Licence)
			.SingleAsync(x => x.Id == AlphaPackId))
			.ToDtoDescriptor();

		AssertPackDescriptorEqual(result, expected);
	}

	[Test]
	public override async Task PutAsync()
	{
		var request = new DtoObjectPackDescriptor(
			AlphaPackId,
			"Updated object pack",
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
			var updated = await db.ObjectPacks.SingleAsync(x => x.Id == AlphaPackId);
			Assert.That(updated.Name, Is.EqualTo("Updated object pack"));
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
			Assert.That(await db.ObjectPacks.AnyAsync(x => x.Id == AlphaPackId), Is.False);
		}
	}

	[Test]
	public async Task GetObjectPackFileAsync_ReturnsZipWithOnlySafeIndexedObjectEntries()
	{
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/{AlphaPackId}{Definitions.Web.Routes.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();
		using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/zip"));
			Assert.That(archive.Entries.Select(x => x.FullName), Is.EqualTo([Path.Combine(ServerFolderManager.CustomFolderName, "safe-object.dat").Replace('\\', '/')]));
			await using var entryStream = archive.Entries.Single().Open();
			using var entryMemoryStream = new MemoryStream();
			await entryStream.CopyToAsync(entryMemoryStream);
			Assert.That(entryMemoryStream.ToArray(), Is.EqualTo(new byte[] { 4, 3, 2, 1 }));
		}
	}

	[Test]
	public async Task GetObjectPackFileAsync_IncludesObjectStoredWithAbsoluteIndexPath()
	{
		const UniqueObjectId packId = 10;
		const string objectFileName = "uploaded-absolute.dat";

		using var scope = testWebAppFactory.Services.CreateScope();
		var sfm = scope.ServiceProvider.GetRequiredService<ServerFolderManager>();

		// Uploaded objects are indexed with an absolute file path (unlike scanned ones, which are relative).
		var absolutePath = Path.Combine(sfm.ObjectsCustomFolder, objectFileName);
		await File.WriteAllBytesAsync(absolutePath, [9, 9, 9, 9]);
		sfm.ObjectIndex.AddEntry(new ObjectIndexEntry("ABSOLUTEOBJ", absolutePath, null, 999, null, ObjectType.Vehicle, ObjectSource.Custom, null, null));

		using (var db = GetDbContext())
		{
			await db.ObjectPacks.AddAsync(new TblObjectPack
			{
				Id = packId,
				Name = "Absolute path pack",
				Objects =
				[
					new TblObject
					{
						Id = 10,
						Name = "absolute-obj",
						ObjectType = ObjectType.Vehicle,
						ObjectSource = ObjectSource.Custom,
						Availability = ObjectAvailability.Available,
						DatObjects =
						[
							new TblDatObject { Id = 10, DatName = "ABSOLUTEOBJ", DatChecksum = 999, xxHash3 = 10, ObjectId = 10 },
						],
					},
				],
			});
			_ = await db.SaveChangesAsync();
		}

		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/{packId}{Definitions.Web.Routes.File}");
		var bytes = await response.Content.ReadAsByteArrayAsync();
		using var archive = new ZipArchive(new MemoryStream(bytes), ZipArchiveMode.Read);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.IsSuccessStatusCode, Is.True);
			Assert.That(archive.Entries.Select(x => x.FullName), Is.EqualTo([$"{ServerFolderManager.CustomFolderName}/{objectFileName}"]));
			await using var entryStream = archive.Entries.Single().Open();
			using var entryMemoryStream = new MemoryStream();
			await entryStream.CopyToAsync(entryMemoryStream);
			Assert.That(entryMemoryStream.ToArray(), Is.EqualTo(new byte[] { 9, 9, 9, 9 }));
		}
	}

	[Test]
	public async Task GetObjectPackAsync_WithUnknownId_ReturnsNotFound()
	{
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/9999");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}
	static void AssertPackDescriptorEqual(DtoItemPackDescriptor<DtoObjectEntry>? actual, DtoItemPackDescriptor<DtoObjectEntry> expected)
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
