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
	public class TagRoutesTest : BaseReferenceDataTableTestFixture<DtoTagEntry, TblTag>
	{
		protected override IEnumerable<DtoTagEntry> SeedData
			=> [new(1, "Wet"),
				new(2, "Dry")];

		protected override DtoTagEntry ExtraSeedDatum
			=> new(3, "Rough");

		public override string BaseRoute
			=> RoutesV2.Tags;

		protected override DbSet<TblTag> GetTable(LocoDbContext context)
			=> context.Tags;

		protected override TblTag ToRowFunc(DtoTagEntry request)
			=> request.ToTable();
	}
}
