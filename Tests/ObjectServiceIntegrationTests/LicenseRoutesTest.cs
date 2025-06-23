using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ObjectServiceIntegrationTests
{
	[TestFixture]
	public class LicenseRoutesTest : BaseReferenceDataTableTestFixture<DtoLicenceEntry, TblLicence>
	{
		protected override IEnumerable<DtoLicenceEntry> SeedData
			=> [new(1, "Gandalf-EULA", "You shall not pass"),
				new(2, "Vader-TOS", "I am your father")];

		protected override DtoLicenceEntry ExtraSeedDatum
			=> new(3, "Constitution", "Do no evil");

		public override string BaseRoute
			=> Routes.Licences;

		protected override DbSet<TblLicence> GetTable(LocoDbContext context)
			=> context.Licences;

		protected override TblLicence ToRowFunc(DtoLicenceEntry request)
			=> request.ToTable();
	}
}
