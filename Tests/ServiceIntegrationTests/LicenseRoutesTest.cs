using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	[TestFixture]
	public class LicenseRoutesTest : BaseServiceTestFixture
	{
		void TestData(LocoDbContext context)
		{
			_ = context.Licences.Add(new() { Name = "Gandalf-EULA", Text = "You shall not pass" });
			_ = context.Licences.Add(new() { Name = "Vader-TOS", Text = "I am your father" });
		}

		[Test]
		public async Task LicenceList()
		{
			// arrange
			await SeedTestData(TestData);

			// act
			var results = await Client.Get<IEnumerable<DtoLicenceEntry>>(HttpClient!, Routes.Licences);

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(2));

				Assert.That(results.First().Name, Is.EqualTo("Gandalf-EULA"));
				Assert.That(results.First().LicenceText, Is.EqualTo("You shall not pass"));

				Assert.That(results.Last().Name, Is.EqualTo("Vader-TOS"));
				Assert.That(results.Last().LicenceText, Is.EqualTo("I am your father"));
			});
		}

		[Test]
		public async Task Get()
		{
			// arrange
			await SeedTestData(TestData);

			// act
			var results = await Client.Get<DtoLicenceEntry>(HttpClient!, Routes.Licences + "/2");

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Id, Is.EqualTo(2));
				Assert.That(results.Name, Is.EqualTo("Vader-TOS"));
				Assert.That(results.LicenceText, Is.EqualTo("I am your father"));
			});
		}
	}
}
