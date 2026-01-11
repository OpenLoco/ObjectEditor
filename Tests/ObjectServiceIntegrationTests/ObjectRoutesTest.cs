using Common;
using Common.Logging;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Comparers;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Definitions.Web;
using Index;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.IO.Hashing;

namespace ObjectService.Tests.Integration;

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
		=> RoutesV2.Objects;

	protected override IEnumerable<TblObject> DbSeedData =>
	[
		new() { Id = 1, Name = "test-name-1", SubObjectId = 1, ObjectType = ObjectType.Vehicle, Availability = ObjectAvailability.Available },
		new() { Id = 2, Name = "test-name-2", SubObjectId = 2, ObjectType = ObjectType.Vehicle, Availability = ObjectAvailability.Available },
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

	DtoObjectPostResponse ToDtoDescriptor(TblObject row)
		=> new(
				row.Id,
				row.Name,
				row.DatObjects.FirstOrDefault()?.DatName ?? "<--->",
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
				row.StringTable.ToDtoDescriptor(row.Id)
				//SubObject
				);

	static void AssertDtoObjectDescriptorsAreEqual(DtoObjectPostResponse? expected, DtoObjectPostResponse? actual)
	{
		using (Assert.EnterMultipleScope())
		{
			Assert.That(expected, Is.Not.Null);
			Assert.That(actual, Is.Not.Null);

			Assert.That(expected.Id, Is.EqualTo(actual.Id));
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
		var results = await ClientHelpers.GetAsync<DtoObjectPostResponse>(HttpClient!, RoutesV2.Prefix, BaseRoute, id);
		var descriptor = ToDtoDescriptor(DbSeedData.ToList()[id - 1]) with { UploadedDate = DateOnly.UtcToday };

		// assert
		AssertDtoObjectDescriptorsAreEqual(results, descriptor);
	}
	[Test]
	public override async Task DeleteAsync()
	{
		// act
		const int id = 1;
		_ = await ClientHelpers.DeleteAsync(HttpClient!, RoutesV2.Prefix, BaseRoute, id);

		// assert
		using (Assert.EnterMultipleScope())
		{
			var results = await ClientHelpers.GetAsync<DtoObjectPostResponse>(HttpClient!, RoutesV2.Prefix, BaseRoute, id);
			var descriptor = ToDtoDescriptor(DbSeedData.ToList()[id - 1]) with { UploadedDate = DateOnly.UtcToday };

			// assert
			AssertDtoObjectDescriptorsAreEqual(results, descriptor);
		}
	}

	[Test]
	public override async Task PostAsync()
	{
		var objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects"; // this is naughty for a test but it'll do
		var logger = new Logger();
		var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);
		_ = index.TryFind(7051740550869341430, out var entry); // randomly selected and hardcoded object

		var filename = Path.Combine(objDirectory, entry.FileName);
		var bytes = File.ReadAllBytes(filename);
		var xxHash3 = XxHash3.HashToUInt64(bytes);
		var base64Bytes = Convert.ToBase64String(bytes);

		// act
		var dtoUploadDat = new DtoObjectPost(base64Bytes, xxHash3, ObjectAvailability.Available, DateOnly.UtcToday, DateOnly.UtcToday);
		var results = await ClientHelpers.PostAsync<DtoObjectPost, DtoObjectPostResponse>(HttpClient!, RoutesV2.Prefix, BaseRoute, dtoUploadDat);

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
			new DtoStringTableDescriptor(expectedStringTable, 3));

		AssertDtoObjectDescriptorsAreEqual(results, expected);
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
			StringTable: new DtoStringTableDescriptor([], id)
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, RoutesV2.Prefix, BaseRoute, id, updateRequest);

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
			StringTable: new DtoStringTableDescriptor([], objectId)
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, RoutesV2.Prefix, BaseRoute, objectId, updateRequest);

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
			StringTable: new DtoStringTableDescriptor([], objectId)
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, RoutesV2.Prefix, BaseRoute, objectId, updateRequest);

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
			StringTable: new DtoStringTableDescriptor([], objectId)
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, RoutesV2.Prefix, BaseRoute, objectId, updateRequest);

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
			StringTable: new DtoStringTableDescriptor([], objectId)
		);

		// act
		var result = await ClientHelpers.PutAsync<DtoObjectPostResponse, DtoObjectPostResponse>(
			HttpClient!, RoutesV2.Prefix, BaseRoute, objectId, updateRequest);

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
}
