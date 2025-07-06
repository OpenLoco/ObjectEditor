using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Tests.ObjectServiceIntegrationTests
{
	public abstract class BaseRouteHandlerTestFixture
	{
		protected HttpClient? HttpClient;
		protected TestWebApplicationFactory<Program> _factory;

		public abstract string BaseRoute { get; }

		[Test] public abstract Task ListAsync();
		[Test] public abstract Task PostAsync();
		[Test] public abstract Task GetAsync();
		[Test] public abstract Task PutAsync();
		[Test] public abstract Task DeleteAsync();

		async Task SeedDataAsync()
		{
			using (var scope = _factory.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<LocoDbContext>();

				// Seed data
				await SeedDataCoreAsync(dbContext);
				_ = await dbContext.SaveChangesAsync();
			}
		}

		protected abstract Task SeedDataCoreAsync(LocoDbContext db);

		[SetUp]
		public async Task SetUp()
		{
			_factory = new TestWebApplicationFactory<Program>()
				.WithConfiguration("ObjectService:RootFolder", Path.GetTempPath());
			HttpClient = _factory.CreateClient();

			await SeedDataAsync();
			var healthResponse = await HttpClient.GetAsync("/health");
			var health = await healthResponse.Content.ReadAsStringAsync();

			Assert.Multiple(() =>
			{
				Assert.That(healthResponse.IsSuccessStatusCode, Is.True);
				Assert.That(health, Is.EqualTo("Healthy"));
			});
		}

		[TearDown]
		public void TearDown()
		{
			HttpClient?.Dispose();
			_factory?.Dispose();
		}
	}

	public abstract class BaseReferenceDataTableTestFixture<TGetDto, TPostDto, TRow> : BaseRouteHandlerTestFixture
		where TGetDto : class, IHasId
		where TPostDto : class, IHasId
		where TRow : class, IHasId
	{
		protected abstract DbSet<TRow> GetTable(LocoDbContext db);
		protected abstract TRow ToRowFunc(TGetDto request);
		protected abstract TGetDto ToDtoEntryFunc(TRow row);

		protected abstract IEnumerable<TRow> DbSeedData { get; }
		protected abstract TGetDto PutDto { get; }

		protected override async Task SeedDataCoreAsync(LocoDbContext db)
		{
			foreach (var x in DbSeedData)
			{
				_ = await GetTable(db).AddAsync(x);
			}
		}

		[Test]
		public override async Task ListAsync()
		{
			// act
			var results = await ClientHelpers.GetAsync<IEnumerable<TGetDto>>(HttpClient!, RoutesV2.Prefix, BaseRoute);

			// assert
			Assert.Multiple(() =>
			{
				Assert.That(results?.Count(), Is.EqualTo(2));
				Assert.That(results, Is.EquivalentTo(DbSeedData.Select(ToDtoEntryFunc)));
			});
		}

		[Test]
		public override async Task GetAsync()
		{
			// act
			const int id = 2;
			var results = await ClientHelpers.GetAsync<TGetDto>(HttpClient!, RoutesV2.Prefix, BaseRoute, id);

			// assert
			Assert.That(results, Is.EqualTo(ToDtoEntryFunc(DbSeedData.ToList()[id - 1])));
		}

		[Test]
		public override async Task DeleteAsync()
		{
			// act
			const int id = 1;
			var results = await ClientHelpers.DeleteAsync(HttpClient!, RoutesV2.Prefix, BaseRoute, id);

			// assert
			Assert.Multiple(async () =>
			{
				var results = await ClientHelpers.GetAsync<IEnumerable<TGetDto>>(HttpClient!, RoutesV2.Prefix, BaseRoute);
				Assert.That(results.First(), Is.EqualTo(ToDtoEntryFunc(DbSeedData.ToList()[id])));
			});
		}

		[Test]
		public override async Task PostAsync()
		{
			// act
			var results = await ClientHelpers.PostAsync(HttpClient!, RoutesV2.Prefix, BaseRoute, PutDto);

			// assert
			Assert.That(results, Is.EqualTo(PutDto));
		}

		[Test]
		public override async Task PutAsync()
		{
			// act
			const int id = 1;
			var results = await ClientHelpers.PutAsync(HttpClient!, RoutesV2.Prefix, BaseRoute, id, PutDto);

			// assert
			Assert.That(results.Id, Is.EqualTo(id));
		}
	}
}
