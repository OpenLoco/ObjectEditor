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
public class ObjectRoutesTest : BaseReferenceDataTableTestFixture<DtoObjectEntry, DtoUploadDat, DtoObjectDescriptor, TblObject>
{
	public override string BaseRoute
		=> RoutesV2.Objects;

	protected override IEnumerable<TblObject> DbSeedData =>
	[
		new() { Id = 1, Name = "test-name-1", SubObjectId = 1, ObjectType = ObjectType.Vehicle, Availability = ObjectAvailability.Available },
		new() { Id = 2, Name = "test-name-2", SubObjectId = 2, ObjectType = ObjectType.Vehicle, Availability = ObjectAvailability.Available },
	];

	protected override DtoUploadDat PostRequestDto
		=> throw new NotImplementedException();

	protected override DtoObjectDescriptor PostResponseDto
		=> throw new NotImplementedException();

	protected override DtoObjectDescriptor PutResponseDto
		=> throw new NotImplementedException();

	protected override DtoUploadDat PutRequestDto
		=> throw new NotImplementedException();

	//protected override DtoUploadDat PostRequestDto
	//	=> new(3, "test-name-3", "display-name-3", 123, "456", ObjectSource.Custom, ObjectType.Vehicle, Dat.Objects.VehicleType.Bus, Definitions.ObjectAvailability.Available, null, null, DateOnly.Today);

	//protected override DtoObjectDescriptor PostResponseDto
	//	=> new(3, "test-name-3", "display-name-3", 123, "456", ObjectSource.Custom, ObjectType.Vehicle, Dat.Objects.VehicleType.Bus, Definitions.ObjectAvailability.Available, null, null, DateOnly.Today);

	//protected override DtoObjectEntry PutDto

	protected override DbSet<TblObject> GetTable(LocoDbContext context)
		=> context.Objects;

	protected override TblObject ToRowFunc(DtoObjectEntry request)
		=> request.ToTable();

	protected override DtoObjectEntry ToDtoEntryFunc(TblObject row)
		=> row.ToDtoEntry() with { UploadedDate = DateOnly.UtcToday };

	DtoObjectDescriptor ToDtoDescriptor(TblObject row)
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

	static void AssertDtoObjectDescriptorsAreEqual(DtoObjectDescriptor? expected, DtoObjectDescriptor? actual)
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
		var results = await ClientHelpers.GetAsync<DtoObjectDescriptor>(HttpClient!, RoutesV2.Prefix, BaseRoute, id);
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
			var results = await ClientHelpers.GetAsync<DtoObjectDescriptor>(HttpClient!, RoutesV2.Prefix, BaseRoute, id);
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
		var dtoUploadDat = new DtoUploadDat(base64Bytes, xxHash3, ObjectAvailability.Available, DateOnly.UtcToday, DateOnly.UtcToday);
		var results = await ClientHelpers.PostAsync<DtoUploadDat, DtoObjectDescriptor>(HttpClient!, RoutesV2.Prefix, BaseRoute, dtoUploadDat);

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

		var expected = new DtoObjectDescriptor(
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
	public async Task AddMissingObjectAsync()
	{
		// arrange
		var missingEntry = new DtoMissingObjectEntry("TESTOBJ1", 123456789, ObjectType.Vehicle);

		// act
		var result = await Client.AddMissingObjectAsync(HttpClient!, missingEntry, new Logger());

		// assert
		Assert.That(result, Is.Not.Zero, "Adding missing object should return the new unique id for that object");

		// verify the object was added to the database
		using var dbContext = GetDbContext();
		var addedObject = await dbContext.Objects
			.Include(x => x.DatObjects)
			.FirstOrDefaultAsync(x => x.Name == $"{missingEntry.DatName}_{missingEntry.DatChecksum}");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(addedObject, Is.Not.Null, "Object should exist in database");
			Assert.That(addedObject!.Availability, Is.EqualTo(ObjectAvailability.Missing));
			Assert.That(addedObject.ObjectType, Is.EqualTo(ObjectType.Vehicle));
			Assert.That(addedObject.DatObjects.Count, Is.EqualTo(1));
			Assert.That(addedObject.DatObjects.First().DatName, Is.EqualTo(missingEntry.DatName));
			Assert.That(addedObject.DatObjects.First().DatChecksum, Is.EqualTo(missingEntry.DatChecksum));
		}
	}
}
