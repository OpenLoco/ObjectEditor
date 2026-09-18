using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ObjectService.Tests.Integration;

namespace Tests.ObjectServiceIntegrationTests.Routes;

[TestFixture]
public class TagRoutesTest : BaseReferenceDataTableTestFixture<
	DtoTagEntry,
	DtoTagEntry,
	DtoTagEntry,
	DtoTagEntry,
	DtoTagEntry,
	TblTag>
{
	protected override IEnumerable<TblTag> DbSeedData =>
	[
		new() { Id = 1, Name = "Wet" },
		new() { Id = 2, Name = "Dry" },
	];

	protected override DtoTagEntry PostRequestDto
		=> new(3, "Rough");

	protected override DtoTagEntry PostResponseDto
		=> new(3, "Rough");

	protected override DtoTagEntry PutRequestDto
		=> new(1, "Rough");

	protected override DtoTagEntry PutResponseDto
		=> new(1, "Rough");

	public override string BaseRoute
		=> Definitions.Web.Routes.Tags;

	protected override DbSet<TblTag> GetTable(LocoDbContext context)
		=> context.Tags;

	protected override TblTag ToRowFunc(DtoTagEntry request)
		=> request.ToTable();

	protected override DtoTagEntry ToDtoEntryFunc(TblTag row)
		=> row.ToDtoEntry();

	[Test]
	public async Task Descriptor_ReturnsTagWithRelationships()
	{
		var descriptor = await Client.GetTagDescriptorAsync(HttpClient!, 1);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor!.Id, Is.EqualTo(1));
			Assert.That(descriptor.Name, Is.EqualTo("Wet"));
			Assert.That(descriptor.Objects, Is.Empty);
			Assert.That(descriptor.ObjectPacks, Is.Empty);
		}
	}
}
