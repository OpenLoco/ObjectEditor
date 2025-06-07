using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	[TestFixture]
	public class LicenseRoutesTest : BaseServiceTestFixture<DtoLicenceEntry, TblLicence>
	{
		protected override IEnumerable<DtoLicenceEntry> SeedData
			=> [new(1, "Gandalf-EULA", "You shall not pass"),
				new(2, "Vader-TOS", "I am your father")];

		public override string BaseRoute
			=> Routes.Licences;

		protected override DbSet<TblLicence> GetTable(LocoDbContext context)
			=> context.Licences;

		protected override TblLicence ToRowFunc(DtoLicenceEntry request)
			=> request.ToTable();
	}
}
