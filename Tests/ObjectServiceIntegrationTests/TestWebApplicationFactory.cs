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
static DirectoryInfo? MakeServerFolderManagerTestDirectories()
{
var testDirectory = Directory.CreateTempSubdirectory("ObjectServiceTest");
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
new("ObjectService:EnableWriteRoutes", "True"),
])
.Build();

_ = builder
.UseConfiguration(testConfigurationBuilder)
.ConfigureServices(services =>
{
var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<LocoDbContext>));
if (descriptor != null)
_ = services.Remove(descriptor);

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