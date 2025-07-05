using Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OpenLoco.Dat.Data;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.DTO.Mappers;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ObjectServiceIntegrationTests
{
	[TestFixture]
	public class ObjectRoutesTest : BaseReferenceDataTableTestFixture<DtoObjectEntry, DtoObjectDescriptor, TblObject>
	{
		public override string BaseRoute
			=> RoutesV2.Objects;

		protected override IEnumerable<TblObject> DbSeedData =>
		[
			new() { Id = 1, Name = "test-name-1" },
			new() { Id = 2, Name = "test-name-2"},
		];

		protected override DtoObjectEntry PutDto
			=> new(3, "test-name-3", "display-name-3", 123, "456", ObjectSource.Custom, ObjectType.Vehicle, Dat.Objects.VehicleType.Bus, Definitions.ObjectAvailability.Available, null, null, DateOnly.Now);

		//protected override async Task SeedDataCoreAsync(LocoDbContext db)
		//{
		//	var tblObject = new TblObject
		//	{
		//		Name = "test-name",
		//	};
		//	_ = await db.Objects.AddAsync(tblObject);
		//}

		protected override DbSet<TblObject> GetTable(LocoDbContext context)
			=> context.Objects;

		protected override TblObject ToRowFunc(DtoObjectEntry request)
			=> request.ToTable();

		protected override DtoObjectEntry ToDtoEntryFunc(TblObject row)
			=> row.ToDtoEntry();
	}
}
