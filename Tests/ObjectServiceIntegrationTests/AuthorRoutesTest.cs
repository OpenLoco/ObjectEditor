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
	public class AuthorRoutesTest : BaseReferenceDataTableTestFixture<DtoAuthorEntry, TblAuthor>
	{
		protected override IEnumerable<DtoAuthorEntry> SeedData
			=> [new(1, "Alice"), new(2, "Bob")];

		protected override DtoAuthorEntry ExtraSeedDatum
			=> new(3, "Charles");

		public override string BaseRoute
			=> RoutesV2.Authors;

		protected override DbSet<TblAuthor> GetTable(LocoDbContext context)
			=> context.Authors;

		protected override TblAuthor ToRowFunc(DtoAuthorEntry request)
			=> request.ToTable();
	}
}
