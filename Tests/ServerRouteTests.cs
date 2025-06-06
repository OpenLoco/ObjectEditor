using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Internal;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;

namespace OpenLoco.Dat.Tests
{
	public class TestWebApplicationFactory<TProgram>
		: WebApplicationFactory<TProgram> where TProgram : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
			=> _ = builder.ConfigureServices(services =>
			{
				// Remove the app's original DbContext registration
				var descriptor = services.SingleOrDefault(
					d => d.ServiceType == typeof(DbContextOptions<LocoDbContext>));

				if (descriptor != null)
				{
					_ = services.Remove(descriptor);
				}

				// Create a SQLite in-memory connection
				var connection = new SqliteConnection("DataSource=:memory:");
				connection.Open();

				// Add DbContext using an in-memory SQLite database
				_ = services.AddDbContext<LocoDbContext>(options => _ = options.UseSqlite(connection));

				// Configure logging for tests ---
				_ = services.AddLogging(loggingBuilder =>
				{
					// Clear all existing providers (like console, debug, etc.)
					_ = loggingBuilder.ClearProviders();

					// Optional: Add a specific provider if you want some logs, e.g., to a test output helper

					_ = loggingBuilder.SetMinimumLevel(LogLevel.Critical); // Only log critical errors
				});

				// Build the service provider.
				var sp = services.BuildServiceProvider();

				// Create a scope to obtain a reference to the database contexts
				using (var scope = sp.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var db = scopedServices.GetRequiredService<LocoDbContext>();
					_ = db.Database.EnsureCreated(); // Ensure the database is created for each test run
				}
			});
	}

	[TestFixture]
	public class ServerRouteTests
	{
		HttpClient? HttpClient;
		TestWebApplicationFactory<Program> _factory;

		[SetUp]
		public void SetUp()
		{
			_factory = new TestWebApplicationFactory<Program>();
			HttpClient = _factory.CreateClient();
		}

		[TearDown]
		public void TearDown()
		{
			HttpClient?.Dispose();
			_factory?.Dispose();
		}

		protected async Task SeedTestData(Action<LocoDbContext> seedAction)
		{
			using (var scope = _factory.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<LocoDbContext>();

				// Seed the specific data for the current test
				seedAction(dbContext);
				_ = await dbContext.SaveChangesAsync();
			}
		}

		[Test]
		public async Task TestAuthorsList()
		{
			// Arrange
			await SeedTestData(dbContext =>
			{
				_ = dbContext.Authors.Add(new() { Name = "Alice" });
				_ = dbContext.Authors.Add(new() { Name = "Bob" });
			});

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
		public async Task TestTagsList()
		{
			// Arrange
			await SeedTestData(dbContext =>
			{
				_ = dbContext.Tags.Add(new() { Name = "Wet" });
				_ = dbContext.Tags.Add(new() { Name = "Dry" });
			});

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
		public async Task TestLicenceList()
		{
			// Arrange
			await SeedTestData(dbContext =>
			{
				_ = dbContext.Licences.Add(new() { Name = "Gandalf-EULA", Text = "You shall not pass" });
				_ = dbContext.Licences.Add(new() { Name = "Vader-TOS", Text = "I am your father" });
			});

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
	}
}
