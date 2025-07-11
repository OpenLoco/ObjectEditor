using Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Common.Logging;
using Dat.Data;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;
using System.IO.Hashing;

namespace Tests.ObjectServiceIntegrationTests
{
	[TestFixture]
	public class ObjectRoutesTest : BaseReferenceDataTableTestFixture<DtoObjectEntry, DtoObjectDescriptor, TblObject>
	{
		public override string BaseRoute
			=> RoutesV2.Objects;

		//protected override async Task SeedDataCoreAsync(LocoDbContext db)
		//{
		//	foreach (var (tblObject, subObject) in DbSeedData.Zip(DbSeedDataSub))
		//	{
		//		var tblObj = await GetTable(db).AddAsync(tblObject);
		//		//subObject.Parent = tblObj.Entity;
		//		//_ = await db.ObjVehicle.AddAsync((TblObjectVehicle)subObject);
		//	}

		//	_ = db.SaveChanges();
		//}

		protected override IEnumerable<TblObject> DbSeedData =>
		[
			new() { Id = 1, Name = "test-name-1", SubObjectId = 1, ObjectType = ObjectType.Vehicle, Availability = Definitions.ObjectAvailability.Available },
			new() { Id = 2, Name = "test-name-2", SubObjectId = 2, ObjectType = ObjectType.Vehicle, Availability = Definitions.ObjectAvailability.Available },
		];

		//IEnumerable<IDbSubObject> DbSeedDataSub =>
		//[
		//	new TblObjectVehicle() { Id = 1, Parent = DbSeedData.ElementAt(0) },
		//	new TblObjectVehicle() { Id = 2, Parent = DbSeedData.ElementAt(1) },
		//];

		protected override DtoObjectEntry PutDto
			=> new(3, "test-name-3", "display-name-3", 123, "456", ObjectSource.Custom, ObjectType.Vehicle, Dat.Objects.VehicleType.Bus, Definitions.ObjectAvailability.Available, null, null, DateOnly.Today);

		protected override DbSet<TblObject> GetTable(LocoDbContext context)
			=> context.Objects;

		protected override TblObject ToRowFunc(DtoObjectEntry request)
			=> request.ToTable();

		protected override DtoObjectEntry ToDtoEntryFunc(TblObject row)
			=> row.ToDtoEntry() with { UploadedDate = DateOnly.Today };

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

		[Test]
		public override async Task GetAsync()
		{
			// act
			const int id = 2;
			var results = await ClientHelpers.GetAsync<DtoObjectDescriptor>(HttpClient!, RoutesV2.Prefix, BaseRoute, id);
			var descriptor = ToDtoDescriptor(DbSeedData.ToList()[id - 1]) with { UploadedDate = DateOnly.Today };
			// assert
			Assert.That(results, Is.EqualTo(descriptor));
		}

		[Test]
		public override async Task PostAsync()
		{
			var objDirectory = "Q:\\Games\\Locomotion\\Server\\Objects";
			var logger = new Logger();
			var index = ObjectIndex.LoadOrCreateIndex(objDirectory, logger);
			var entry = index.Objects.First();

			try
			{
				var filename = Path.Combine(objDirectory, entry.FileName);
				var bytes = File.ReadAllBytes(filename);
				var xxHash3 = XxHash3.HashToUInt64(bytes);
				var base64Bytes = Convert.ToBase64String(bytes);

				// act
				var dtoUploadDat = new DtoUploadDat(base64Bytes, xxHash3, Definitions.ObjectAvailability.Available, DateOnly.Today, DateOnly.Today);
				var results = await ClientHelpers.PostAsync(HttpClient!, RoutesV2.Prefix, BaseRoute, dtoUploadDat);

				// assert
				Assert.That(results, Is.Not.Null);
			}
			catch (Exception ex)
			{
				//Console.WriteLine($"{obj.FileName} - {ex.Message}");
			}
		}
	}
}
