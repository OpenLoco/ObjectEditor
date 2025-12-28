using Definitions;
using Definitions.Database;
using Definitions.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ObjectService.Tests.Integration;

public abstract class BaseRouteHandlerTestFixture
{
	protected HttpClient? HttpClient;
	protected TestWebApplicationFactory<Program> testWebAppFactory;

	public abstract string BaseRoute { get; }

	[Test] public abstract Task ListAsync();
	[Test] public abstract Task PostAsync();
	[Test] public abstract Task GetAsync();
	[Test] public abstract Task PutAsync();
	[Test] public abstract Task DeleteAsync();

	protected LocoDbContext GetDbContext()
		=> testWebAppFactory
			.Services
			.CreateScope()
			.ServiceProvider
			.GetRequiredService<LocoDbContext>();

	async Task SeedDataAsync()
	{
		using var dbContext = GetDbContext();
		await SeedDataCoreAsync(dbContext);
		_ = await dbContext.SaveChangesAsync();

	}

	protected abstract Task SeedDataCoreAsync(LocoDbContext db);

	[SetUp]
	public async Task SetUp()
	{
		testWebAppFactory = new TestWebApplicationFactory<Program>();
		using (Assert.EnterMultipleScope())
		{
			var config = testWebAppFactory.Services.GetRequiredService<IConfiguration>();
			Assert.That(config["ObjectService:RootFolder"].StartsWith(Path.GetTempPath()));
			Assert.That(Directory.Exists(config["ObjectService:RootFolder"]), Is.True);
			Assert.That(config.GetValue<bool>("ObjectService:ShowScalar"), Is.False);
		}

		HttpClient = testWebAppFactory.CreateClient();

		await SeedDataAsync();
		var healthResponse = await HttpClient.GetAsync("/health");
		var health = await healthResponse.Content.ReadAsStringAsync();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(healthResponse.IsSuccessStatusCode, Is.True);
			Assert.That(health, Is.EqualTo("Healthy"));
		}
	}

	[TearDown]
	public void TearDown()
	{
		HttpClient?.Dispose();
		testWebAppFactory?.Dispose();
	}
}

public abstract class BaseReferenceDataTableTestFixture<TGetDto, TRequestDto, TResponseDto, TRow> : BaseRouteHandlerTestFixture
	where TGetDto : class, IHasId
	where TRequestDto : class // POST doens't have an ID, and PUT has its id as part of the route, not the body/object
	where TResponseDto : class, IHasId
	where TRow : class, IHasId
{
	protected abstract DbSet<TRow> GetTable(LocoDbContext db);
	protected abstract TRow ToRowFunc(TGetDto request);
	protected abstract TGetDto ToDtoEntryFunc(TRow row);

	protected abstract IEnumerable<TRow> DbSeedData { get; }
	protected abstract TRequestDto PostRequestDto { get; }
	protected abstract TResponseDto PostResponseDto { get; }
	protected abstract TRequestDto PutRequestDto { get; }
	protected abstract TResponseDto PutResponseDto { get; }

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
		using (Assert.EnterMultipleScope())
		{
			Assert.That(results?.Count(), Is.EqualTo(2));
			Assert.That(results, Is.EquivalentTo(DbSeedData.Select(ToDtoEntryFunc)));
		}
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
		_ = await ClientHelpers.DeleteAsync(HttpClient!, RoutesV2.Prefix, BaseRoute, id);

		// assert
		using (Assert.EnterMultipleScope())
		{
			var results = await ClientHelpers.GetAsync<IEnumerable<TGetDto>>(HttpClient!, RoutesV2.Prefix, BaseRoute);
			Assert.That(results.First(), Is.EqualTo(ToDtoEntryFunc(DbSeedData.ToList()[id])));
		}
	}

	[Test]
	public override async Task PostAsync()
	{
		// act
		var results = await ClientHelpers.PostAsync<TRequestDto, TResponseDto>(HttpClient!, RoutesV2.Prefix, BaseRoute, PostRequestDto);

		// assert
		Assert.That(results, Is.EqualTo(PostResponseDto));
	}

	[Test]
	public override async Task PutAsync()
	{
		// act
		const int id = 1;
		var results = await ClientHelpers.PutAsync<TRequestDto, TResponseDto>(HttpClient!, RoutesV2.Prefix, BaseRoute, id, PutRequestDto);

		// assert
		Assert.That(results, Is.EqualTo(PutResponseDto));
	}
}
