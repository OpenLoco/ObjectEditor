using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Tests.ObjectServiceIntegrationTests
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
}
