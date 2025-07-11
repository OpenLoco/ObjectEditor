using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;

namespace Tests.ObjectServiceIntegrationTests;

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
