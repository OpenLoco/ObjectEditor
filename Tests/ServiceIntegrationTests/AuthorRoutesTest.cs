using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	[TestFixture]
	public class AuthorRoutesTest : BaseServiceTestFixture
	{
		void TestData(LocoDbContext context)
		{
			_ = context.Authors.Add(new() { Name = "Alice" });
			_ = context.Authors.Add(new() { Name = "Bob" });
		}

		[Test]
		public async Task List()
		{
			// arrange
			await SeedTestData(TestData);

			// act
			var results = await Client.Get<IEnumerable<DtoAuthorEntry>>(HttpClient!, Routes.Authors);

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(2));
				Assert.That(results.First().Name, Is.EqualTo("Alice"));
				Assert.That(results.Last().Name, Is.EqualTo("Bob"));
			});
		}

		[Test]
		public async Task Get()
		{
			// arrange
			await SeedTestData(TestData);

			// act
			var results = await Client.Get<DtoAuthorEntry>(HttpClient!, Routes.Authors + "/2");

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Id, Is.EqualTo(2));
				Assert.That(results.Name, Is.EqualTo("Bob"));
			});
		}

		[Test]
		public async Task Post()
		{
			// arrange
			var preResults = await Client.Get<IEnumerable<DtoAuthorEntry>>(HttpClient!, Routes.Authors);
			Assert.That(preResults.Count(), Is.Zero);

			// act
			var postResult = await Client.Post(HttpClient!, Routes.Authors, new DtoAuthorEntry(0, "Fred"));
			Assert.That(postResult);

			// act
			var results = await Client.Get<DtoAuthorEntry>(HttpClient!, Routes.Authors + "/1");

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Id, Is.EqualTo(1));
				Assert.That(results.Name, Is.EqualTo("Fred"));
			});
		}

		[Test]
		public async Task Delete()
		{
			// arrange
			await SeedTestData(TestData);

			// pre-assert
			Assert.Multiple(async () =>
			{
				var results = await Client.Get<IEnumerable<DtoAuthorEntry>>(HttpClient!, Routes.Authors);
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(2));
				Assert.That(results.First().Name, Is.EqualTo("Alice"));
				Assert.That(results.Last().Name, Is.EqualTo("Bob"));
			});

			// act
			var results = await Client.Delete<DtoAuthorEntry>(HttpClient!, Routes.Authors + "/1");

			// assert
			Assert.Multiple(async () =>
			{
				var results = await Client.Get<IEnumerable<DtoAuthorEntry>>(HttpClient!, Routes.Authors);
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(1));
				Assert.That(results.Last().Name, Is.EqualTo("Bob"));
			});
		}
	}
}
