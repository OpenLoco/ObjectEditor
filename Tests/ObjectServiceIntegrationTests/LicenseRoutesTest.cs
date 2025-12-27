using Definitions.Database;
using Definitions.DTO;
using Definitions.DTO.Mappers;
using Definitions.Web;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ObjectService.Tests.Integration;

[TestFixture]
public class LicenseRoutesTest : BaseReferenceDataTableTestFixture<DtoLicenceEntry, DtoLicenceEntry, DtoLicenceEntry, TblLicence>
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
		=> RoutesV2.Licences;

	protected override DbSet<TblLicence> GetTable(LocoDbContext context)
		=> context.Licences;

	protected override TblLicence ToRowFunc(DtoLicenceEntry request)
		=> request.ToTable();

	protected override DtoLicenceEntry ToDtoEntryFunc(TblLicence row)
		=> row.ToDtoEntry();
}
