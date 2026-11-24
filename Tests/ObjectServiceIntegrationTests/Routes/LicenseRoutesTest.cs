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
public class LicenseRoutesTest : BaseReferenceDataTableTestFixture<
	DtoLicenceEntry,
	DtoLicenceEntry,
	DtoLicenceEntry,
	DtoLicenceEntry,
	DtoLicenceEntry,
	TblLicence>
{
	protected override IEnumerable<TblLicence> DbSeedData =>
	[
		new() { Id = 1, Name = "Gandalf-EULA", Text = "You shall not pass" },
		new() { Id = 2, Name = "Vader-TOS", Text = "I am your father" },
	];

	protected override DtoLicenceEntry PostRequestDto
		=> new(3, "Constitution", "Do no evil");

	protected override DtoLicenceEntry PostResponseDto
		=> new(3, "Constitution", "Do no evil");

	protected override DtoLicenceEntry PutRequestDto
		=> new(1, "Constitution", "Do no evil");

	protected override DtoLicenceEntry PutResponseDto
		=> new(1, "Constitution", "Do no evil");

	public override string BaseRoute
		=> Definitions.Web.Routes.Licences;

	protected override DbSet<TblLicence> GetTable(LocoDbContext context)
		=> context.Licences;

	protected override TblLicence ToRowFunc(DtoLicenceEntry request)
		=> request.ToTable();

	protected override DtoLicenceEntry ToDtoEntryFunc(TblLicence row)
		=> row.ToDtoEntry();

	[Test]
	public async Task Descriptor_ReturnsLicenceWithRelationships()
	{
		var descriptor = await Client.GetLicenceDescriptorAsync(HttpClient!, 1);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(descriptor, Is.Not.Null);
			Assert.That(descriptor!.Id, Is.EqualTo(1));
			Assert.That(descriptor.Name, Is.EqualTo("Gandalf-EULA"));
			Assert.That(descriptor.Text, Is.EqualTo("You shall not pass"));
			Assert.That(descriptor.Objects, Is.Empty);
			Assert.That(descriptor.ObjectPacks, Is.Empty);
		}
	}
}
