using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	[TestFixture]
	public class TagRoutesTest : BaseServiceTestFixture
	{
		void TestData(LocoDbContext context)
		{
			_ = context.Tags.Add(new() { Name = "Wet" });
			_ = context.Tags.Add(new() { Name = "Dry" });
		}

		[Test]
		public async Task TagsList()
		{
			// arrange
			await SeedTestData(TestData);

			// act
			var results = await Client.Get<IEnumerable<DtoTagEntry>>(HttpClient!, Routes.Tags);

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(2));
				Assert.That(results.First().Name, Is.EqualTo("Wet"));
				Assert.That(results.Last().Name, Is.EqualTo("Dry"));
			});
		}

		[Test]
		public async Task Get()
		{
			// arrange
			await SeedTestData(TestData);

			// act
			var results = await Client.Get<DtoTagEntry>(HttpClient!, Routes.Tags + "/2");

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Id, Is.EqualTo(2));
				Assert.That(results.Name, Is.EqualTo("Dry"));
			});
		}
	}
}
