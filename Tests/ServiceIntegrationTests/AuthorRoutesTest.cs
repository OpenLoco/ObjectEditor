using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	[TestFixture]
	public class AuthorRoutesTest : BaseServiceTestFixture<DtoAuthorEntry, TblAuthor>
	{
		protected override IEnumerable<DtoAuthorEntry> SeedData
			=> [new(1, "Alice"), new(2, "Bob")];

		public override string BaseRoute
			=> Routes.Authors;

		protected override DbSet<TblAuthor> GetTable(LocoDbContext context)
			=> context.Authors;

		protected override TblAuthor ToRowFunc(DtoAuthorEntry request)
			=> request.ToTable();

		[Test]
		public async Task Post()
		{
			// act
			var results = await Client.PostAsync(HttpClient!, BaseRoute, new DtoAuthorEntry(0, "Fred"));

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Id, Is.EqualTo(3));
				Assert.That(results.Name, Is.EqualTo("Fred"));
			});
		}

		[Test]
		public async Task Put()
		{
			// act
			var results = await Client.PutAsync(HttpClient!, BaseRoute, 1, new DtoAuthorEntry(1, "Fred"));

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
			// act
			var results = await Client.DeleteAsync(HttpClient!, BaseRoute, 1);

			// assert
			Assert.Multiple(async () =>
			{
				var results = await Client.GetAsync<IEnumerable<DtoAuthorEntry>>(HttpClient!, Routes.Authors);
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(1));
				Assert.That(results.Last().Name, Is.EqualTo("Bob"));
			});
		}

	}
}
