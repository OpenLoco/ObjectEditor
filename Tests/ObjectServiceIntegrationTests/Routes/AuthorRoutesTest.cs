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
public class AuthorRoutesTest : BaseReferenceDataTableTestFixture<
	DtoAuthorEntry,
	DtoAuthorEntry,
	DtoAuthorEntry,
	DtoAuthorEntry,
	DtoAuthorEntry,
	TblAuthor>
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
		=> Definitions.Web.Routes.Authors;

	protected override DbSet<TblAuthor> GetTable(LocoDbContext context)
		=> context.Authors;

	protected override TblAuthor ToRowFunc(DtoAuthorEntry request)
		=> request.ToTable();

	protected override DtoAuthorEntry ToDtoEntryFunc(TblAuthor row)
		=> row.ToDtoEntry();

	[Test]
	public async Task Descriptor_ReturnsAuthorWithRelationships()
	{
		var descriptor = await Client.GetAuthorDescriptorAsync(HttpClient!, 1);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor!.Id, Is.EqualTo(1));
			Assert.That(descriptor.Name, Is.EqualTo("Alice"));
			Assert.That(descriptor.Objects, Is.Empty);
			Assert.That(descriptor.ObjectPacks, Is.Empty);
			Assert.That(descriptor.SC5Files, Is.Empty);
			Assert.That(descriptor.ScenarioPacks, Is.Empty);
		}
	}

}
