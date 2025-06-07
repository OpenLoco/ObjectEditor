using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	[TestFixture]
	public class TagRoutesTest : BaseServiceTestFixture<DtoTagEntry, TblTag>
	{
		protected override IEnumerable<DtoTagEntry> SeedData
			=> [new(1, "Wet"),
				new(2, "Dry")];

		public override string BaseRoute
			=> Routes.Tags;

		protected override DbSet<TblTag> GetTable(LocoDbContext context)
			=> context.Tags;

		protected override TblTag ToRowFunc(DtoTagEntry request)
			=> request.ToTable();
	}
}
