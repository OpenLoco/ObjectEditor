using Common;
using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ObjectService;
using ObjectService.Tests.Integration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tests.ObjectServiceIntegrationTests.Routes;

/// <summary>
/// Integration tests for the game-data file routes (<c>/v2/music</c>, <c>/v2/soundeffects</c>,
/// <c>/v2/tutorials</c>, <c>/v2/graphics</c>) that back the Music, SoundEffects, Tutorials and
/// Graphics tables.
/// </summary>
[TestFixture]
public class GameDataFileRoutesTests
{
	private TestWebApplicationFactory<Program> _factory = null!;
	private HttpClient _http = null!;

	[SetUp]
	public async Task SetUp()
	{
		_factory = new TestWebApplicationFactory<Program>();
		_http = _factory.CreateClient();

		using var db = GetDbContext();
		var sfm = GetServices().GetRequiredService<ServerFolderManager>();

		await AddFilesAsync(db, sfm);
	}

	[TearDown]
	public void TearDown()
	{
		_http.Dispose();
		_factory.Dispose();
	}

	private LocoDbContext GetDbContext()
		=> _factory.Services.CreateScope().ServiceProvider.GetRequiredService<LocoDbContext>();

	private IServiceProvider GetServices()
		=> _factory.Services;

	private static async Task AddFilesAsync(LocoDbContext db, ServerFolderManager sfm)
	{
		await WriteFileAsync(sfm.MusicFolder, "Custom/music.dat", db.Music, new TblMusic { Id = 1, Name = "Custom/music.dat", ObjectSource = ObjectSource.Custom });
		await WriteFileAsync(sfm.SoundEffectsFolder, "Custom/sound.dat", db.SoundEffects, new TblSoundEffect { Id = 1, Name = "Custom/sound.dat", ObjectSource = ObjectSource.Custom });
		await WriteFileAsync(sfm.TutorialsFolder, "Custom/tutorial.dat", db.Tutorials, new TblTutorial { Id = 1, Name = "Custom/tutorial.dat", ObjectSource = ObjectSource.Custom });
		await WriteFileAsync(sfm.GraphicsFolder, "Custom/graphics.dat", db.Graphics, new TblGraphics { Id = 1, Name = "Custom/graphics.dat", ObjectSource = ObjectSource.Custom });

		_ = await db.SaveChangesAsync();
	}

	private static async Task WriteFileAsync<TEntity>(string folder, string relativeName, DbSet<TEntity> set, TEntity entity)
		where TEntity : DbCoreObject
	{
		var fullPath = Path.Combine(folder, relativeName);
		_ = Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
		await File.WriteAllBytesAsync(fullPath, [1, 2, 3, 4]);
		_ = await set.AddAsync(entity);
	}

	[TestCase(Definitions.Web.Routes.Music, "music")]
	[TestCase(Definitions.Web.Routes.SoundEffects, "sound")]
	[TestCase(Definitions.Web.Routes.Tutorials, "tutorial")]
	[TestCase(Definitions.Web.Routes.Graphics, "graphics")]
	public async Task ListAsync_ReturnsTheSeededRow(string route, string kind)
	{
		using var response = await _http.GetAsync($"{Definitions.Web.Routes.Prefix}{route}");

		using (Assert.EnterMultipleScope())
		{
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}

		using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
		var entries = document.RootElement;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(entries.GetArrayLength(), Is.EqualTo(1));
			Assert.That(entries[0].GetProperty("name").GetString(), Is.EqualTo($"Custom/{kind}.dat"));
			Assert.That(entries[0].GetProperty("authorCount").GetInt32(), Is.EqualTo(0));
			Assert.That(entries[0].GetProperty("tagCount").GetInt32(), Is.EqualTo(0));
		}
	}
	[Test]
	public async Task GetAsync_ReturnsTheMusicDescriptor()
	{
		using var response = await _http.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}/1");
		response.EnsureSuccessStatusCode();

		using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
		var root = document.RootElement;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(root.GetProperty("id").GetUInt64(), Is.EqualTo(1));
			Assert.That(root.GetProperty("name").GetString(), Is.EqualTo("Custom/music.dat"));
			Assert.That(root.GetProperty("objectSource").GetInt32(), Is.EqualTo((int)ObjectSource.Custom));
			Assert.That(root.GetProperty("authors").GetArrayLength(), Is.EqualTo(0));
			Assert.That(root.GetProperty("tags").GetArrayLength(), Is.EqualTo(0));
		}
	}

	[Test]
	public async Task PutAsync_UpdatesMusicMetadata()
	{
		var request = new DtoMusicDescriptor(
			1,
			"Custom/renamed.dat",
			"updated description",
			ObjectSource.OpenLoco,
			DateOnly.FromDateTime(new DateTime(2020, 1, 1)),
			DateOnly.FromDateTime(new DateTime(2024, 12, 15)),
			DateOnly.UtcToday,
			null,
			[],
			[]);

		using var response = await _http.PutAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}/1", request);
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		using var db = GetDbContext();
		var updated = await db.Music.SingleAsync(x => x.Id == 1);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(updated.Name, Is.EqualTo("Custom/renamed.dat"));
			Assert.That(updated.Description, Is.EqualTo("updated description"));
			Assert.That(updated.ObjectSource, Is.EqualTo(ObjectSource.OpenLoco));
		}
	}

	[Test]
	public async Task DeleteAsync_RemovesTheMusicRow()
	{
		using var response = await _http.DeleteAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}/1");
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

		using var db = GetDbContext();
		Assert.That(await db.Music.AnyAsync(x => x.Id == 1), Is.False);
	}

	[Test]
	public async Task GetFileAsync_ReturnsTheFileBytes()
	{
		using var response = await _http.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}/1{Definitions.Web.Routes.File}");
		response.EnsureSuccessStatusCode();

		Assert.That(await response.Content.ReadAsByteArrayAsync(), Is.EqualTo(new byte[] { 1, 2, 3, 4 }));
	}

	[Test]
	public async Task GetFileAsync_ReturnsNotFound_WhenFileIsMissingFromDisk()
	{
		var sfm = GetServices().GetRequiredService<ServerFolderManager>();
		File.Delete(Path.Combine(sfm.MusicFolder, "Custom", "music.dat"));

		using var response = await _http.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}/1{Definitions.Web.Routes.File}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task GetFileAsync_ReturnsNotFound_ForPathTraversalName()
	{
		using (var db = GetDbContext())
		{
			var music = await db.Music.SingleAsync(x => x.Id == 1);
			music.Name = "../outside.dat";
			_ = await db.SaveChangesAsync();
		}

		using var response = await _http.GetAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}/1{Definitions.Web.Routes.File}");

		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
	}

	[Test]
	public async Task PostAsync_IsNotImplemented()
	{
		using var response = await _http.PostAsJsonAsync($"{Definitions.Web.Routes.Prefix}{Definitions.Web.Routes.Music}", new { });
		Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotImplemented));
	}


}
