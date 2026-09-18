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
	/// <summary>Overridden by tests that need to start the app with write routes disabled.</summary>
	protected virtual bool BackendReadOnly => false;

	/// <summary>Overridden by tests that need to start the app with the frontend in read-only mode.</summary>
	protected virtual bool FrontendReadOnly => false;

	static DirectoryInfo? MakeServerFolderManagerTestDirectories()
	{
		// The ServerFolderManager now creates the full
		// GameData/<category>/{Original,Custom,OpenLoco} structure on construction,
		// so tests only need a writable root directory.
		return Directory.CreateTempSubdirectory("ObjectServiceTest");
	}

	static void CreateDummyPaletteFile(string path)
	{
		using var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(16, 16);
		using var stream = File.Create(path);
		image.Save(stream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var testFolder = MakeServerFolderManagerTestDirectories();
		ArgumentNullException.ThrowIfNull(testFolder, nameof(testFolder));
		var dummyPaletteFile = Path.Combine(testFolder.FullName, "palette.png");
		CreateDummyPaletteFile(dummyPaletteFile);

		var testConfigurationBuilder = new ConfigurationBuilder()
			.AddInMemoryCollection([
				new("ObjectService:RootFolder", testFolder.FullName),
				new("ObjectService:PaletteMapFile", dummyPaletteFile),
				new("ObjectService:ShowScalar", "False"),
				new("ObjectService:DisableAuthentication", "True"),
				new("ObjectService:FrontendReadOnly", FrontendReadOnly.ToString()),
				new("ObjectService:BackendReadOnly", BackendReadOnly.ToString()),
			])
			.Build();

		_ = builder
		.UseConfiguration(testConfigurationBuilder)
		.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LocoDbContext>));
			if (descriptor != null)
			{
				_ = services.Remove(descriptor);
			}

			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();
			_ = services.AddDbContext<LocoDbContext>(options => _ = options.UseSqlite(connection));

			_ = services.AddLogging(loggingBuilder =>
			{
				_ = loggingBuilder.ClearProviders();
				_ = loggingBuilder.SetMinimumLevel(LogLevel.Critical);
			});

			var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
			_ = db.Database.EnsureCreated();
		});
	}
}
