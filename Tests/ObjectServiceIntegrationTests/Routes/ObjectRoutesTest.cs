using Common;
using Common.Logging;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Comparers;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.Web;
using Index;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService;
using ObjectService.Tests.Integration;
using System.IO.Hashing;
using System.Net;
using System.Net.Http.Json;

namespace Tests.ObjectServiceIntegrationTests.Routes;

[TestFixture]
public class ObjectRoutesTest : BaseReferenceDataTableTestFixture<
	DtoObjectEntry,
	DtoObjectPost,
	DtoObjectPostResponse,
	DtoObjectPostResponse,
	DtoObjectPostResponse,
	TblObject>
{
	public override string BaseRoute
		=> Definitions.Web.Routes.Objects;

	protected override IEnumerable<TblObject> DbSeedData =>
	[
		new() { Id = 1, Name = "test-name-1", ObjectType = ObjectType.Vehicle, Availability = ObjectAvailability.Available },
		new() { Id = 2, Name = "test-name-2", ObjectType = ObjectType.Vehicle, Availability = ObjectAvailability.Available },
	];

	protected override DtoObjectPost PostRequestDto
		=> throw new NotImplementedException();

	protected override DtoObjectPostResponse PostResponseDto
		=> throw new NotImplementedException();

	protected override DtoObjectPostResponse PutResponseDto
		=> throw new NotImplementedException();

	protected override DtoObjectPostResponse PutRequestDto
		=> throw new NotImplementedException("PUT operation uses DtoObjectDescriptor, not DtoUploadDat. Override PutAsync test instead.");

	//protected override DtoUploadDat PostRequestDto
	//	=> new(3, "test-name-3", "display-name-3", 123, "456", ObjectSource.Custom, ObjectType.Vehicle, Dat.Objects.VehicleType.Bus, Definitions.ObjectAvailability.Available, null, null, DateOnly.Today);

	//protected override DtoObjectDescriptor PostResponseDto
	//	=> new(3, "test-name-3", "display-name-3", 123, "456", ObjectSource.Custom, ObjectType.Vehicle, Dat.Objects.VehicleType.Bus, Definitions.ObjectAvailability.Available, null, null, DateOnly.Today);

	//protected override DtoObjectEntry PutDto

	protected override DbSet<TblObject> GetTable(LocoDbContext db)
		=> db.Objects;

	protected override TblObject ToRowFunc(DtoObjectEntry request)
		=> request.ToTable();

	protected override DtoObjectEntry ToDtoEntryFunc(TblObject row)
		=> row.ToDtoEntry() with { UploadedDate = DateOnly.UtcToday };

	static DtoObjectPostResponse ToDtoDescriptor(TblObject row)
		=> new(
				row.Id,
				row.Name,
				row.DatObjects.FirstOrDefault()?.DatName ?? row.DatObjects.FirstOrDefault()?.Object?.Name ?? "<no-display-name>",
				row.DatObjects.FirstOrDefault()?.DatChecksum ?? 0,
				row.Description,
				row.ObjectSource,
				row.ObjectType,
				row.VehicleType,
				row.Availability,
				row.CreatedDate,
				row.ModifiedDate,
				row.UploadedDate,
				row.Licence?.ToDtoEntry(),
				[.. row.Authors.Select(x => x.ToDtoEntry())],
				[.. row.Tags.Select(x => x.ToDtoEntry())],
				[],
				[],
				row.StringTable.ToDtoDescriptor(row.Id),
				null // SubObject
				);

	static void AssertDtoObjectDescriptorsAreEqual(DtoObjectPostResponse? expected, DtoObjectPostResponse? actual)
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(expected, Is.Not.Null);
			Assert.That(actual, Is.Not.Null);

			Assert.That(expected!.Id, Is.EqualTo(actual!.Id));
			Assert.That(expected.Name, Is.EqualTo(actual.Name));
			Assert.That(expected.DisplayName, Is.EqualTo(actual.DisplayName));
			Assert.That(expected.DatChecksum, Is.EqualTo(actual.DatChecksum));
			Assert.That(expected.Description, Is.EqualTo(actual.Description));
			Assert.That(expected.ObjectSource, Is.EqualTo(actual.ObjectSource));
			Assert.That(expected.ObjectType, Is.EqualTo(actual.ObjectType));
			Assert.That(expected.VehicleType, Is.EqualTo(actual.VehicleType));
			Assert.That(expected.Availability, Is.EqualTo(actual.Availability));
			Assert.That(expected.CreatedDate, Is.EqualTo(actual.CreatedDate));
			Assert.That(expected.ModifiedDate, Is.EqualTo(actual.ModifiedDate));
			Assert.That(expected.UploadedDate, Is.EqualTo(actual.UploadedDate));

			Assert.That(actual.Licence, Is.EqualTo(expected.Licence).Using(new DtoLicenceEntryComparer()));
			Assert.That(actual.Authors, Is.EqualTo(expected.Authors).Using(new DtoAuthorEntryComparer()));
			Assert.That(actual.Tags, Is.EqualTo(expected.Tags).Using(new DtoTagEntryComparer()));
			Assert.That(actual.ObjectPacks, Is.EqualTo(expected.ObjectPacks).Using(new DtoItemPackEntryComparer()));
			Assert.That(actual.DatObjects, Is.EqualTo(expected.DatObjects).Using(new DtoDatObjectEntryComparer()));
		}

		AssertDtoStringTableDescriptorsAreEqual(expected.StringTable, actual.StringTable);
	}

	static void AssertDtoStringTableDescriptorsAreEqual(DtoStringTableDescriptor? expected, DtoStringTableDescriptor? actual)
	{
		ArgumentNullException.ThrowIfNull(expected);
		ArgumentNullException.ThrowIfNull(actual);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(actual, Is.Not.Null);
			Assert.That(expected.ObjectId, Is.EqualTo(actual.ObjectId), "Object Id");

			foreach (var z1 in expected.Table.Zip(actual.Table))
			{
				Assert.That(z1.First.Key, Is.EqualTo(z1.Second.Key), "StringName");

				foreach (var z2 in z1.First.Value.Zip(z1.Second.Value))
				{
					Assert.That(z2.First.Key, Is.EqualTo(z2.Second.Key), $"{z1.First.Key}-{z2.First.Key}-Language");
					Assert.That(z2.First.Value, Is.EqualTo(z2.Second.Value), $"{z1.First.Key}-{z2.First.Key}-Text");
				}
			}
		}
	}

	[Test]
	public override async Task GetAsync()
	{
		// act
		const int id = 2;
		var results = await ClientHelpers.GetAsync<DtoObjectPostResponse>(HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, id);
		var descriptor = ToDtoDescriptor(DbSeedData.ToList()[id - 1]) with { UploadedDate = DateOnly.UtcToday };

		// assert
		AssertDtoObjectDescriptorsAreEqual(results, descriptor);
	}
	[Test]
	public override async Task DeleteAsync()
	{
		// act - removal keeps the row (so curated metadata and references survive), parks any files under
		// GameData/Objects/Removed and marks the object unavailable.
		const int id = 1;
		var deleted = await ClientHelpers.DeleteAsync(HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, id);

		// assert
		using var db = GetDbContext();
		var row = await db.Objects.AsNoTracking().SingleAsync(x => x.Id == id);
		var results = await ClientHelpers.GetAsync<DtoObjectPostResponse>(HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, id);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(deleted, Is.True);
			Assert.That(row.Availability, Is.EqualTo(ObjectAvailability.Unavailable));
			Assert.That(results, Is.Not.Null);
			Assert.That(results!.Availability, Is.EqualTo(ObjectAvailability.Unavailable));
		}
	}

	[Test]
	public override async Task PostAsync()
	{
		var objDirectory = @"Q:\Games\Locomotion\Server\GameData\Objects"; // this is naughty for a test but it'll do
		var logger = new Logger();
		var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);
		_ = index.TryFind(7051740550869341430, out var entry); // randomly selected and hardcoded object
		Assert.That(entry, Is.Not.Null);

		var filename = Path.Combine(objDirectory, entry!.FileName ?? string.Empty);
		var bytes = File.ReadAllBytes(filename);
		var xxHash3 = XxHash3.HashToUInt64(bytes);
		var base64Bytes = Convert.ToBase64String(bytes);

		// act
		var dtoUploadDat = new DtoObjectPost(base64Bytes, xxHash3, ObjectAvailability.Available, DateOnly.UtcToday, DateOnly.UtcToday);
		var results = await ClientHelpers.PostAsync<DtoObjectPost, DtoObjectPostResponse>(HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, dtoUploadDat);

		// assert
		var expectedStringTable = new Dictionary<string, Dictionary<LanguageId, string>>()
		{
			{
				"Name",
				new Dictionary<LanguageId, string>()
				{
					{ LanguageId.English_UK, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.English_US, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.French, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.German, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.Spanish, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.Italian, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.Dutch, string.Empty },
					{ LanguageId.Swedish, string.Empty },
					{ LanguageId.Japanese, string.Empty },
					{ LanguageId.Korean, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.Chinese_Simplified, string.Empty },
					{ LanguageId.Chinese_Traditional, "AZ Voith Gravita 15 BB Northrail" },
					{ LanguageId.id_12, string.Empty },
					{ LanguageId.Portuguese, string.Empty },
				}
			},
		};

		var expected = new DtoObjectPostResponse(
			3,
			"AZVOG15C_3072098364",
			entry.DisplayName,
			3072098364,
			string.Empty,
			ObjectSource.Custom,
			ObjectType.Vehicle,
			entry.VehicleType,
			ObjectAvailability.Available,
			DateOnly.UtcToday,
			DateOnly.UtcToday,
			DateOnly.UtcToday,
			null, // licence
			[], // authors
			[], // tags
			[], // object packs
			[new DtoDatObjectEntry(1, "AZVOG15C", 3072098364, 7051740550869341430, 3)], // dat objects
			new DtoStringTableDescriptor(expectedStringTable, 3),
			null); // SubObject

		AssertDtoObjectDescriptorsAreEqual(results, expected);

		// The uploaded file must be indexed with a path relative to the Objects folder (not an absolute
		// path), so that file reads and object-pack downloads resolve consistently.
		using var scope = testWebAppFactory.Services.CreateScope();
		var sfm = scope.ServiceProvider.GetRequiredService<ServerFolderManager>();
		Assert.That(sfm.ObjectIndex.TryFind((entry.DisplayName, 3072098364), out var uploadedEntry), Is.True);
		Assert.That(uploadedEntry, Is.Not.Null);
		Assert.That(Path.IsPathRooted(uploadedEntry!.FileName), Is.False);
		Assert.That(File.Exists(Path.Combine(sfm.ObjectsFolder, uploadedEntry.FileName!)), Is.True);
	}

	[Test]
	public async Task PutAsync_WithSubObject_PersistsSubObjectChanges()
	{
		// arrange - an Airport object with an existing sub-object row
		const ulong objectId = 100;
		using (var seedDb = GetDbContext())
		{
			var airportObject = new TblObject
			{
				Id = objectId,
				Name = "airport-object",
				ObjectType = ObjectType.Airport,
				ObjectSource = ObjectSource.Custom,
				Availability = ObjectAvailability.Available,
			};

			_ = await seedDb.Objects.AddAsync(airportObject);
			_ = await seedDb.SaveChangesAsync();

			_ = await seedDb.ObjAirport.AddAsync(new TblObjectAirport { Parent = airportObject, MinX = 1 });
			_ = await seedDb.SaveChangesAsync();
		}

		var request = new DtoObjectPostResponse(
			Id: objectId,
			Name: "airport-object",
			DisplayName: "airport-object",
			DatChecksum: null,
			Description: "updated via sub-object",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Airport,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: new DtoObjectAirport { Id = 1, MinX = -5, RequiredClearEdges = 7 });

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, request);

		// assert
		using var verifyDb = GetDbContext();
		var subObject = await verifyDb.ObjAirport.AsNoTracking().SingleAsync(x => x.Parent.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(subObject.MinX, Is.EqualTo((sbyte)-5), "the sub-object edit must be persisted, not discarded");
			Assert.That(subObject.RequiredClearEdges, Is.EqualTo(7u));
		}
	}

	[Test]
	public async Task PutAsync_WithoutSubObject_RemovesTheExistingSubObject()
	{
		// arrange - PUT replaces the whole resource, so omitting the sub-object must remove it
		const ulong objectId = 103;
		using (var seedDb = GetDbContext())
		{
			var airportObject = new TblObject
			{
				Id = objectId,
				Name = "airport-remove-object",
				ObjectType = ObjectType.Airport,
				ObjectSource = ObjectSource.Custom,
				Availability = ObjectAvailability.Available,
			};

			_ = await seedDb.Objects.AddAsync(airportObject);
			_ = await seedDb.SaveChangesAsync();

			_ = await seedDb.ObjAirport.AddAsync(new TblObjectAirport { Parent = airportObject, MinX = 4 });
			_ = await seedDb.SaveChangesAsync();
		}

		var request = new DtoObjectPostResponse(
			Id: objectId,
			Name: "airport-remove-object",
			DisplayName: "airport-remove-object",
			DatChecksum: null,
			Description: "sub-object omitted",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Airport,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: null);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, request);

		// assert
		using var verifyDb = GetDbContext();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(await verifyDb.ObjAirport.AsNoTracking().AnyAsync(x => x.Parent.Id == objectId), Is.False);
		}
	}

	[Test]
	public async Task PutAsync_WithStringTable_AddsUpdatesAndRemovesRows()
	{
		// arrange - an object with two existing string-table rows
		const ulong objectId = 101;
		using (var seedDb = GetDbContext())
		{
			_ = await seedDb.Objects.AddAsync(new TblObject
			{
				Id = objectId,
				Name = "stringtable-object",
				ObjectType = ObjectType.Vehicle,
				ObjectSource = ObjectSource.Custom,
				Availability = ObjectAvailability.Available,
			});
			_ = await seedDb.SaveChangesAsync();

			await seedDb.StringTable.AddRangeAsync(
				new TblStringTableRow { Name = "Name", Language = LanguageId.English_UK, Text = "old name", ObjectId = objectId },
				new TblStringTableRow { Name = "Removed", Language = LanguageId.English_UK, Text = "delete me", ObjectId = objectId });
			_ = await seedDb.SaveChangesAsync();
		}

		var stringTable = new DtoStringTableDescriptor(
			new Dictionary<string, Dictionary<LanguageId, string>>
			{
				["Name"] = new() { [LanguageId.English_UK] = "new name" },
				["Extra"] = new() { [LanguageId.French] = "bonjour" },
			},
			objectId);

		var request = new DtoObjectPostResponse(
			Id: objectId,
			Name: "stringtable-object",
			DisplayName: "stringtable-object",
			DatChecksum: null,
			Description: "string table edit",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Vehicle,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: stringTable,
			SubObject: null);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, request);

		// assert
		using var verifyDb = GetDbContext();
		var rows = await verifyDb.StringTable.AsNoTracking().Where(r => r.ObjectId == objectId).ToListAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(rows, Has.Count.EqualTo(2));
			Assert.That(rows.Single(r => r.Name == "Name" && r.Language == LanguageId.English_UK).Text, Is.EqualTo("new name"));
			Assert.That(rows.Single(r => r.Name == "Extra" && r.Language == LanguageId.French).Text, Is.EqualTo("bonjour"));
			Assert.That(rows.Any(r => r.Name == "Removed"), Is.False, "rows the request omits must be removed");
		}
	}

	[Test]
	public async Task PutAsync_WithEmptyStringTable_ClearsTheRows()
	{
		// arrange - PUT replaces the whole resource, so omitting every row must clear the table (clients
		// are expected to send the rows they want to keep).
		const ulong objectId = 102;
		using (var seedDb = GetDbContext())
		{
			_ = await seedDb.Objects.AddAsync(new TblObject
			{
				Id = objectId,
				Name = "stringtable-preserve-object",
				ObjectType = ObjectType.Vehicle,
				ObjectSource = ObjectSource.Custom,
				Availability = ObjectAvailability.Available,
			});
			_ = await seedDb.SaveChangesAsync();

			_ = await seedDb.StringTable.AddAsync(
				new TblStringTableRow { Name = "Name", Language = LanguageId.English_UK, Text = "keep me", ObjectId = objectId });
			_ = await seedDb.SaveChangesAsync();
		}

		var request = new DtoObjectPostResponse(
			Id: objectId,
			Name: "stringtable-preserve-object",
			DisplayName: "stringtable-preserve-object",
			DatChecksum: null,
			Description: "metadata only edit",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Vehicle,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: null);

		// act
		_ = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, request);

		// assert
		using var verifyDb = GetDbContext();
		var rows = await verifyDb.StringTable.AsNoTracking().Where(r => r.ObjectId == objectId).ToListAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(rows, Is.Empty, "an empty string table means the client sent no rows, so all rows are removed");
		}
	}

	[Test]
	public override async Task PutAsync()
	{
		// arrange
		const int id = 2;
		var existingObj = DbSeedData.ToList()[id - 1];
		var updatedDescription = "Updated description";
		var updatedCreatedDate = DateOnly.FromDateTime(new DateTime(2020, 1, 1));
		var updatedModifiedDate = DateOnly.FromDateTime(new DateTime(2024, 12, 15));

		var updateRequest = new DtoObjectPostResponse(
			Id: id,
			Name: existingObj.Name,
			DisplayName: "test-display-name-2",
			DatChecksum: null,
			Description: updatedDescription,
			ObjectSource: existingObj.ObjectSource,
			ObjectType: existingObj.ObjectType,
			VehicleType: existingObj.VehicleType,
			Availability: ObjectAvailability.Available,
			CreatedDate: updatedCreatedDate,
			ModifiedDate: updatedModifiedDate,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], id),
			SubObject: null
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, id, updateRequest);

		// assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Id, Is.EqualTo(id));
			Assert.That(result.Description, Is.EqualTo(updatedDescription));
			Assert.That(result.CreatedDate, Is.EqualTo(updatedCreatedDate));
			Assert.That(result.ModifiedDate, Is.EqualTo(updatedModifiedDate));
			Assert.That(result.Availability, Is.EqualTo(ObjectAvailability.Available));
		}
	}

	[Test]
	public async Task PutAsync_UpdatesLicence()
	{
		// arrange
		const int objectId = 2;

		// Create a test licence
		var licence = new TblLicence { Id = 1, Name = "Test Licence", Text = "Test licence text" };
		using (var db = GetDbContext())
		{
			_ = await db.Licences.AddAsync(licence);
			_ = await db.SaveChangesAsync();
		}

		var updateRequest = new DtoObjectPostResponse(
			Id: objectId,
			Name: "test-name-2",
			DisplayName: "test-display-name-2",
			DatChecksum: null,
			Description: "Test description",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Vehicle,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: new DtoLicenceEntry(licence.Id, licence.Name, licence.Text),
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: null
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, updateRequest);

		// assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Licence, Is.Not.Null);
			Assert.That(result.Licence!.Id, Is.EqualTo(licence.Id));
			Assert.That(result.Licence.Name, Is.EqualTo(licence.Name));
		}
	}

	[Test]
	public async Task PutAsync_UpdatesAuthors()
	{
		// arrange
		const int objectId = 2;

		// Create test authors
		var author1 = new TblAuthor { Id = 1, Name = "Test Author 1" };
		var author2 = new TblAuthor { Id = 2, Name = "Test Author 2" };
		using (var db = GetDbContext())
		{
			_ = await db.Authors.AddAsync(author1);
			_ = await db.Authors.AddAsync(author2);
			_ = await db.SaveChangesAsync();
		}

		var updateRequest = new DtoObjectPostResponse(
			Id: objectId,
			Name: "test-name-2",
			DisplayName: "test-display-name-2",
			DatChecksum: null,
			Description: "Test description",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Vehicle,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [
				new DtoAuthorEntry(author1.Id, author1.Name),
				new DtoAuthorEntry(author2.Id, author2.Name)
			],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: null
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, updateRequest);

		// assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Authors, Is.Not.Null);
			Assert.That(result.Authors.Count, Is.EqualTo(2));
			Assert.That(result.Authors.Any(a => a.Id == author1.Id), Is.True);
			Assert.That(result.Authors.Any(a => a.Id == author2.Id), Is.True);
		}
	}

	[Test]
	public async Task PutAsync_UpdatesTags()
	{
		// arrange
		const int objectId = 2;

		// Create test tags
		var tag1 = new TblTag { Id = 1, Name = "Test Tag 1" };
		var tag2 = new TblTag { Id = 2, Name = "Test Tag 2" };
		using (var db = GetDbContext())
		{
			_ = await db.Tags.AddAsync(tag1);
			_ = await db.Tags.AddAsync(tag2);
			_ = await db.SaveChangesAsync();
		}

		var updateRequest = new DtoObjectPostResponse(
			Id: objectId,
			Name: "test-name-2",
			DisplayName: "test-display-name-2",
			DatChecksum: null,
			Description: "Test description",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Vehicle,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [
				new DtoTagEntry(tag1.Id, tag1.Name),
				new DtoTagEntry(tag2.Id, tag2.Name)
			],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: null
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, updateRequest);

		// assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.Tags, Is.Not.Null);
			Assert.That(result.Tags.Count, Is.EqualTo(2));
			Assert.That(result.Tags.Any(t => t.Id == tag1.Id), Is.True);
			Assert.That(result.Tags.Any(t => t.Id == tag2.Id), Is.True);
		}
	}

	[Test]
	public async Task PutAsync_UpdatesObjectPacks()
	{
		// arrange
		const int objectId = 2;

		// Create test object packs
		var pack1 = new TblObjectPack { Id = 1, Name = "Test Pack 1", Description = "Test pack 1 description" };
		var pack2 = new TblObjectPack { Id = 2, Name = "Test Pack 2", Description = "Test pack 2 description" };
		using (var db = GetDbContext())
		{
			_ = await db.ObjectPacks.AddAsync(pack1);
			_ = await db.ObjectPacks.AddAsync(pack2);
			_ = await db.SaveChangesAsync();
		}

		var updateRequest = new DtoObjectPostResponse(
			Id: objectId,
			Name: "test-name-2",
			DisplayName: "test-display-name-2",
			DatChecksum: null,
			Description: "Test description",
			ObjectSource: ObjectSource.Custom,
			ObjectType: ObjectType.Vehicle,
			VehicleType: null,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [
				new DtoItemPackEntry(pack1.Id, pack1.Name, pack1.Description, null, null, DateOnly.UtcToday, null),
				new DtoItemPackEntry(pack2.Id, pack2.Name, pack2.Description, null, null, DateOnly.UtcToday, null)
			],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], objectId),
			SubObject: null
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, updateRequest);

		// assert
		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result!.ObjectPacks, Is.Not.Null);
			Assert.That(result.ObjectPacks.Count, Is.EqualTo(2));
			Assert.That(result.ObjectPacks.Any(p => p.Id == pack1.Id), Is.True);
			Assert.That(result.ObjectPacks.Any(p => p.Id == pack2.Id), Is.True);
		}
	}

	[Test]
	public async Task GetObjectImageAsync_ReturnsForbidden_ForRestrictedObjectSource()
	{
		// arrange - a GoG-sourced object cannot expose its images
		using (var db = GetDbContext())
		{
			_ = await db.Objects.AddAsync(new TblObject
			{
				Id = 3,
				Name = "restricted-name-3",
				ObjectType = ObjectType.Vehicle,
				ObjectSource = ObjectSource.LocomotionGoG,
				Availability = ObjectAvailability.Available,
			});
			_ = await db.SaveChangesAsync();
		}

		// act
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/3{Definitions.Web.Routes.Images}/0");

		// assert - Forbid proves the generic /images/{imageId} route reached the handler
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
	}

	[Test]
	public async Task GetObjectImageAsync_ReturnsNotFound_WhenImageIndexDoesNotExist()
	{
		// act - object 1 exists but has no DatObjects, so it has no images
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/1{Definitions.Web.Routes.Images}/0");

		// assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetObjectImageAsync_ReturnsNotFound_WhenObjectDoesNotExist()
	{
		// act
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/9999{Definitions.Web.Routes.Images}/0");

		// assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetObjectImageAsync_ReturnsNotFound_ForNonNumericImageId()
	{
		// act - the {imageId:int} route constraint rejects non-numeric ids
		using var response = await HttpClient!.GetAsync($"{Definitions.Web.Routes.Prefix}{BaseRoute}/1{Definitions.Web.Routes.Images}/not-a-number");

		// assert
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	/// <summary>
	/// Builds a PUT body. Every <c>Objects</c> column and the sub-object are the request's business;
	/// which DAT file(s) an object is built from is not, so <c>DatObjects</c> is empty here and the server
	/// ignores it.
	/// </summary>
	static DtoObjectPostResponse PutRequest(
		ulong id,
		string name,
		ObjectType objectType,
		ObjectSource objectSource = ObjectSource.Custom,
		VehicleType? vehicleType = null,
		IDtoSubObject? subObject = null,
		string? description = null)
		=> new(
			Id: id,
			Name: name,
			DisplayName: name,
			DatChecksum: null,
			Description: description,
			ObjectSource: objectSource,
			ObjectType: objectType,
			VehicleType: vehicleType,
			Availability: ObjectAvailability.Available,
			CreatedDate: null,
			ModifiedDate: null,
			UploadedDate: DateOnly.UtcToday,
			Licence: null,
			Authors: [],
			Tags: [],
			ObjectPacks: [],
			DatObjects: [],
			StringTable: new DtoStringTableDescriptor([], id),
			SubObject: subObject);

	async Task SeedObjectAsync(ulong id, string name, ObjectType objectType = ObjectType.Vehicle, ObjectSource objectSource = ObjectSource.Custom)
	{
		using var seedDb = GetDbContext();
		_ = await seedDb.Objects.AddAsync(new TblObject
		{
			Id = id,
			Name = name,
			ObjectType = objectType,
			ObjectSource = objectSource,
			Availability = ObjectAvailability.Available,
		});
		_ = await seedDb.SaveChangesAsync();
	}

	string ObjectRoute(ulong id)
		=> $"{Definitions.Web.Routes.Prefix}{BaseRoute}/{id}";

	[Test]
	public async Task PutAsync_AppliesEveryChangeableHeaderRowColumn()
	{
		// arrange - PUT replaces the whole Objects row, not just the curated metadata (ObjectSource is the
		// one exception, see PutAsync_DoesNotChangeTheObjectSource)
		const ulong objectId = 110;
		await SeedObjectAsync(objectId, "before-rename", ObjectType.Airport);

		var request = PutRequest(objectId, "after-rename", ObjectType.Vehicle, vehicleType: VehicleType.Train, description: "header rewrite");

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, request);

		// assert
		using var verifyDb = GetDbContext();
		var row = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(row.Name, Is.EqualTo("after-rename"), "Name must be applied");
			Assert.That(row.ObjectType, Is.EqualTo(ObjectType.Vehicle), "ObjectType must be applied");
			Assert.That(row.ObjectSource, Is.EqualTo(ObjectSource.Custom), "ObjectSource stays as the server set it");
			Assert.That(row.VehicleType, Is.EqualTo(VehicleType.Train), "VehicleType must be applied");
			Assert.That(row.Description, Is.EqualTo("header rewrite"));
			Assert.That(result!.Name, Is.EqualTo("after-rename"));
			Assert.That(result.VehicleType, Is.EqualTo(VehicleType.Train));
		}
	}

	[Test]
	public async Task PutAsync_ChangingObjectType_MovesTheSubObjectToTheNewTypeTable()
	{
		// arrange - an Airport object with a sub-object row of its own type
		const ulong objectId = 111;
		await SeedObjectAsync(objectId, "type-change-object", ObjectType.Airport);

		using (var seedDb = GetDbContext())
		{
			var parent = await seedDb.Objects.SingleAsync(x => x.Id == objectId);
			_ = await seedDb.ObjAirport.AddAsync(new TblObjectAirport { Parent = parent, MinX = 3 });
			_ = await seedDb.SaveChangesAsync();
		}

		var request = PutRequest(objectId, "type-change-object", ObjectType.Vehicle,
			subObject: new DtoObjectVehicle { Id = 6, Type = VehicleType.Bus, NumCarComponents = 4 });

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, Definitions.Web.Routes.Prefix, BaseRoute, objectId, request);

		// assert
		using var verifyDb = GetDbContext();
		var vehicle = await verifyDb.ObjVehicle.AsNoTracking().SingleAsync(x => x.Parent.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(result, Is.Not.Null, "the type change must be accepted");
			Assert.That(await verifyDb.ObjAirport.AsNoTracking().AnyAsync(x => x.Parent.Id == objectId), Is.False, "the previous type's row must not be left behind");
			Assert.That(vehicle.NumCarComponents, Is.EqualTo((byte)4));
			Assert.That(result!.SubObject, Is.TypeOf<DtoObjectVehicle>());
		}
	}

	[Test]
	public async Task PutAsync_WithAnotherTypesSubObject_ReturnsBadRequest()
	{
		// arrange - writing vehicle data into an airport object would leave an orphan row behind
		const ulong objectId = 112;
		await SeedObjectAsync(objectId, "mismatched-sub-object", ObjectType.Airport);

		var request = PutRequest(objectId, "renamed-by-rejected-request", ObjectType.Airport,
			description: "must not be applied",
			subObject: new DtoObjectVehicle { Id = 7, Type = VehicleType.Train });

		// act
		using var response = await HttpClient!.PutAsJsonAsync(ObjectRoute(objectId), request);

		// assert
		using var verifyDb = GetDbContext();
		var row = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.That(await verifyDb.ObjVehicle.AsNoTracking().AnyAsync(x => x.Parent.Id == objectId), Is.False);
			Assert.That(row.Name, Is.EqualTo("mismatched-sub-object"), "a rejected request must not half-apply");
			Assert.That(row.Description, Is.Null, "a rejected request must not half-apply");
		}
	}

	[Test]
	public async Task PutAsync_WithTakenName_ReturnsConflict()
	{
		// arrange - Objects.Name is unique
		const ulong objectId = 113;
		const ulong takenByObjectId = 114;
		await SeedObjectAsync(objectId, "first-object");
		await SeedObjectAsync(takenByObjectId, "second-object");

		var request = PutRequest(objectId, "second-object", ObjectType.Vehicle);

		// act
		using var response = await HttpClient!.PutAsJsonAsync(ObjectRoute(objectId), request);

		// assert
		using var verifyDb = GetDbContext();
		var row = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
			Assert.That(row.Name, Is.EqualTo("first-object"), "the stored name must survive a rejected rename");
		}
	}

	[Test]
	public async Task PutAsync_WithoutName_ReturnsBadRequest()
	{
		const ulong objectId = 115;
		await SeedObjectAsync(objectId, "name-less-request");

		var request = PutRequest(objectId, "   ", ObjectType.Vehicle);

		// act
		using var response = await HttpClient!.PutAsJsonAsync(ObjectRoute(objectId), request);

		// assert
		using var verifyDb = GetDbContext();
		var row = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
			Assert.That(row.Name, Is.EqualTo("name-less-request"));
		}
	}

	[Test]
	public async Task PutAsync_VanillaObject_ReturnsForbidden()
	{
		// arrange - vanilla (original Locomotion) objects can never be edited by anyone
		const ulong objectId = 116;
		await SeedObjectAsync(objectId, "vanilla-object", ObjectType.Vehicle, ObjectSource.LocomotionSteam);

		var request = PutRequest(objectId, "renamed-vanilla", ObjectType.Vehicle, ObjectSource.Custom);

		// act
		using var response = await HttpClient!.PutAsJsonAsync(ObjectRoute(objectId), request);

		// assert
		using var verifyDb = GetDbContext();
		var row = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == objectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			Assert.That(row.Name, Is.EqualTo("vanilla-object"));
			Assert.That(row.ObjectSource, Is.EqualTo(ObjectSource.LocomotionSteam));
		}
	}

	[Test]
	public async Task PutAsync_DoesNotChangeTheObjectSource()
	{
		// arrange - the source says where an object came from, which only the server knows: uploads always
		// store Custom, and Steam/GoG/OpenLoco objects are placed in the server folders by hand
		const ulong openLocoObjectId = 117;
		const ulong customObjectId = 118;
		await SeedObjectAsync(openLocoObjectId, "openloco-object", ObjectType.Vehicle, ObjectSource.OpenLoco);
		await SeedObjectAsync(customObjectId, "custom-object", ObjectType.Vehicle, ObjectSource.Custom);

		// act - a request claiming a different source, in both directions
		using var demotedToCustom = await HttpClient!.PutAsJsonAsync(ObjectRoute(openLocoObjectId),
			PutRequest(openLocoObjectId, "openloco-object", ObjectType.Vehicle, ObjectSource.Custom));
		using var promotedToOpenLoco = await HttpClient!.PutAsJsonAsync(ObjectRoute(customObjectId),
			PutRequest(customObjectId, "custom-object", ObjectType.Vehicle, ObjectSource.OpenLoco));

		// assert
		using var verifyDb = GetDbContext();
		var demotedRow = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == openLocoObjectId);
		var promotedRow = await verifyDb.Objects.AsNoTracking().SingleAsync(x => x.Id == customObjectId);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(demotedToCustom.StatusCode, Is.EqualTo(HttpStatusCode.OK), "the request is applied, only the source is ignored");
			Assert.That(promotedToOpenLoco.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(demotedRow.ObjectSource, Is.EqualTo(ObjectSource.OpenLoco), "an OpenLoco object cannot be demoted to Custom");
			Assert.That(promotedRow.ObjectSource, Is.EqualTo(ObjectSource.Custom), "a custom object cannot be promoted to OpenLoco");
		}
	}
}
