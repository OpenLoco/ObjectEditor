using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class AuthorRoutesTest : BaseReferenceDataTableTestFixture<DtoAuthorEntry, DtoAuthorEntry, DtoAuthorEntry, TblAuthor>
{
	protected override IEnumerable<TblAuthor> DbSeedData =>
	[
		new() { Id = 1, Name = "Alice" },
		new() { Id = 2, Name = "Bob" },
	];

	protected override DtoAuthorEntry PostRequestDto
		=> new(3, "Charles");

	protected override DtoAuthorEntry PostResponseDto
		=> new(3, "Charles");

	protected override DtoAuthorEntry PutRequestDto
		=> new(1, "Charles");

	protected override DtoAuthorEntry PutResponseDto
		=> new(1, "Charles");


	public override string BaseRoute
		=> RoutesV2.Authors;

	protected override DbSet<TblAuthor> GetTable(LocoDbContext context)
		=> context.Authors;

	protected override TblAuthor ToRowFunc(DtoAuthorEntry request)
		=> request.ToTable();

	protected override DtoAuthorEntry ToDtoEntryFunc(TblAuthor row)
		=> row.ToDtoEntry();

}
