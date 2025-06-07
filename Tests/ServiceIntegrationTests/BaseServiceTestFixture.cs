using Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ServiceIntegrationTests
{
	public abstract class BaseServiceTestFixture<TDto, TRow>
		where TDto : class, IHasId
		where TRow : class, IHasId
	{
		protected HttpClient? HttpClient;
		protected TestWebApplicationFactory<Program> _factory;

		protected abstract IEnumerable<TDto> SeedData { get; }
		public abstract string BaseRoute { get; }
		protected abstract DbSet<TRow> GetTable(LocoDbContext db);
		protected abstract TRow ToRowFunc(TDto request);

		[SetUp]
		public async Task SetUp()
		{
			_factory = new TestWebApplicationFactory<Program>();
			HttpClient = _factory.CreateClient();
			await SeedDataCore();
		}

		[TearDown]
		public void TearDown()
		{
			HttpClient?.Dispose();
			_factory?.Dispose();
		}

		protected async Task SeedDataCore()
		{
			using (var scope = _factory.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<LocoDbContext>();

				// Seed data
				foreach (var x in SeedData)
				{
					_ = GetTable(dbContext).Add(ToRowFunc(x));
				}

				_ = await dbContext.SaveChangesAsync();
			}
		}

		[Test]
		public virtual async Task ListAsync()
		{
			// act
			var results = await Client.GetAsync<IEnumerable<TDto>>(HttpClient!, BaseRoute);

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results.Count(), Is.EqualTo(2));
				Assert.That(results, Is.EquivalentTo(SeedData));
			});
		}

		[Test]
		public async Task GetAsync()
		{
			// act
			const int id = 2;
			var results = await Client.GetAsync<TDto>(HttpClient!, BaseRoute, id);

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results, Is.Not.Null);
				Assert.That(results, Is.EqualTo(SeedData.ToList()[id - 1]));
			});
		}
	}
}
