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
	public class LicenseRoutesTest : BaseReferenceDataTableTestFixture<DtoLicenceEntry, DtoLicenceEntry, TblLicence>
	{
		protected override IEnumerable<TblLicence> DbSeedData =>
		[
			new() { Id = 1, Name = "Gandalf-EULA", Text="You shall not pass" },
			new() { Id = 2, Name = "Vader-TOS", Text="I am your father" },
		];

		protected override DtoLicenceEntry PutDto
			=> new(3, "Constitution", "Do no evil");

		public override string BaseRoute
			=> RoutesV2.Licences;

		protected override DbSet<TblLicence> GetTable(LocoDbContext context)
			=> context.Licences;

		protected override TblLicence ToRowFunc(DtoLicenceEntry request)
			=> request.ToTable();

		protected override DtoLicenceEntry ToDtoEntryFunc(TblLicence row)
			=> row.ToDtoEntry();
	}
}
