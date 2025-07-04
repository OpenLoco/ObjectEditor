using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.DTO.Mappers;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ObjectServiceIntegrationTests
{
	[TestFixture]
	public class TagRoutesTest : BaseReferenceDataTableTestFixture<DtoTagEntry, DtoTagEntry, TblTag>
	{
		protected override IEnumerable<TblTag> DbSeedData =>
		[
			new() { Id = 1, Name = "Wet" },
			new() { Id = 2, Name = "Dry" },
		];

		protected override DtoTagEntry PutDto
			=> new(3, "Rough");

		public override string BaseRoute
			=> RoutesV2.Tags;

		protected override DbSet<TblTag> GetTable(LocoDbContext context)
			=> context.Tags;

		protected override TblTag ToRowFunc(DtoTagEntry request)
			=> request.ToTable();

		protected override DtoTagEntry ToDtoEntryFunc(TblTag row)
			=> row.ToDtoEntry();
	}
}
