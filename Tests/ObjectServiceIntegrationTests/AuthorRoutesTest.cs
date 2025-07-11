using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;

namespace Tests.ObjectServiceIntegrationTests;

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
