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
	public class AuthorRoutesTest : BaseReferenceDataTableTestFixture<DtoAuthorEntry, DtoAuthorEntry, TblAuthor>
	{
		protected override IEnumerable<TblAuthor> DbSeedData =>
		[
			new() { Id = 1, Name = "Alice" },
			new() { Id = 2, Name = "Bob" },
		];

		protected override DtoAuthorEntry PutDto
			=> new(3, "Charles");

		public override string BaseRoute
			=> RoutesV2.Authors;

		protected override DbSet<TblAuthor> GetTable(LocoDbContext context)
			=> context.Authors;

		protected override TblAuthor ToRowFunc(DtoAuthorEntry request)
			=> request.ToTable();

		protected override DtoAuthorEntry ToDtoEntryFunc(TblAuthor row)
			=> row.ToDtoEntry();
	}
}
