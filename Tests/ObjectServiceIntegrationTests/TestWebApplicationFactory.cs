using Definitions.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ObjectService.Tests.Integration;

public class TestWebApplicationFactory<TProgram>
	: WebApplicationFactory<TProgram> where TProgram : class
{
	DirectoryInfo? MakeServerFolderManagerTestDirectories()
	{
		// parent dir
		var testDirectory = Directory.CreateTempSubdirectory("ObjectServiceTest");

		// sub dirs
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Objects"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Objects//Custom"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Objects//Original"));

		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Scenarios"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Scenarios//Custom"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Scenarios//Original"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Scenarios//Original//GoG"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Scenarios//Original//Steam"));

		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "Landscapes"));

		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//Graphics//Custom"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//Graphics//Original"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//Music//Custom"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//Music//Original"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//SoundEffects//Custom"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//SoundEffects//Original"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//Tutorials//Custom"));
		_ = Directory.CreateDirectory(Path.Combine(testDirectory.FullName, "GameData//Tutorials//Original"));

		return testDirectory;
	}

	static void CreateDummyPaletteFile(string path)
	{
		// Create a 16x16 pixel PNG file for testing (palette map expects 16x16)
		using var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(16, 16);
		using var stream = File.Create(path);
		image.Save(stream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var testFolder = MakeServerFolderManagerTestDirectories();
		ArgumentNullException.ThrowIfNull(testFolder, nameof(testFolder));

		// Create a dummy palette file for testing
		var dummyPaletteFile = Path.Combine(testFolder.FullName, "palette.png");
		CreateDummyPaletteFile(dummyPaletteFile);

		var testConfigurationBuilder =
			new ConfigurationBuilder()
				.AddInMemoryCollection(
				[
					new("ObjectService:RootFolder", testFolder.FullName),
					new("ObjectService:PaletteMapFile", dummyPaletteFile),
					new("ObjectService:ShowScalar", "False"),
				])
				.Build();

		_ = builder
			.UseConfiguration(testConfigurationBuilder)
			.ConfigureServices(services =>
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
