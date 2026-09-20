using Definitions.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using ObjectService.Services;

namespace ObjectService.Tests.Integration;

/// <summary>
/// End-to-end tests for the <see cref="GameDataWatcherService"/>: they start the real hosted service
/// against a temporary server folder and a file-backed SQLite database, then drop files in and
/// delete them to prove each folder's own service reacts correctly.
/// </summary>
[TestFixture]
public class GameDataWatcherServiceTests
{
	private const string SourceObjectsFolder = @"Q:\Games\Locomotion\OriginalObjects\Steam";
	private const string SourceScenariosFolder = @"Q:\Games\Locomotion\Server\GameData\Scenarios";

	private static async Task<bool> WaitUntilAsync(Func<Task<bool>> condition, TimeSpan timeout)
	{
		var deadline = DateTime.UtcNow + timeout;
		while (DateTime.UtcNow < deadline)
		{
			if (await condition())
			{
				return true;
			}

			await Task.Delay(200);
		}

		return await condition();
	}

	private static IReadOnlyList<string> FindSourceFiles(string folder, string extension)
	{
		if (!Directory.Exists(folder))
		{
			return [];
		}

		return
		[
			.. Directory
				.GetFiles(folder, "*", SearchOption.AllDirectories)
				.Where(x => Path.GetExtension(x).Equals(extension, StringComparison.OrdinalIgnoreCase))
				.OrderBy(x => new FileInfo(x).Length)
				.ThenBy(x => x, StringComparer.OrdinalIgnoreCase)
		];
	}

	private static string? FindSourceFile(string folder, string extension)
		=> FindSourceFiles(folder, extension).FirstOrDefault();

	private static ServiceProvider BuildProvider(ServerFolderManager sfm, string dbPath)
	{
		var services = new ServiceCollection();
		_ = services.AddLogging();
		_ = services.AddSingleton(sfm);
		_ = services.AddDbContext<LocoDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));
		_ = services.AddGameDataFileWatchers();

		var provider = services.BuildServiceProvider();

		using (var scope = provider.CreateScope())
		{
			_ = scope.ServiceProvider.GetRequiredService<LocoDbContext>().Database.EnsureCreated();
		}

		return provider;
	}

	private static async Task<IHostedService> StartAsync(ServiceProvider provider)
	{
		var hostedService = provider.GetServices<IHostedService>().Single();
		await hostedService.StartAsync(CancellationToken.None);

		// Let the startup reconciliation of the (empty) folders complete so the assertions below
		// genuinely exercise the live FileSystemWatcher event path, not the reconcile path.
		await Task.Delay(TimeSpan.FromSeconds(2));
		return hostedService;
	}
	[Test]
	public async Task Watcher_ImportsObjectsAndScenarios_AndMarksObjectsUnavailableWhenDeleted()
	{
		var datSource = FindSourceFile(SourceObjectsFolder, ".dat");
		if (datSource == null)
		{
			Assert.Ignore("No source DAT files are available to drop");
		}

		var sc5Source = FindSourceFile(SourceScenariosFolder, ".SC5");

		var root = Directory.CreateTempSubdirectory("game-data-watcher").FullName;
		ServiceProvider? provider = null;
		try
		{
			var sfm = new ServerFolderManager(root);
			provider = BuildProvider(sfm, Path.Combine(root, "loco.db"));
			var hostedService = await StartAsync(provider);

			// --- drop an object DAT ---
			var datDestination = Path.Combine(sfm.ObjectsCustomFolder, Path.GetFileName(datSource!));
			File.Copy(datSource!, datDestination);

			var objectImported = await WaitUntilAsync(
				async () =>
				{
					using var scope = provider.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
					// The index is written after the database row, so wait for both.
					return sfm.ObjectIndex.Objects.Count > 0 && await db.Objects.AnyAsync();
				},
				TimeSpan.FromSeconds(30));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(objectImported, Is.True, "The dropped object DAT was not imported in time");
				Assert.That(sfm.ObjectIndex.Objects, Is.Not.Empty);
			}

			// --- drop a scenario SC5 ---
			if (sc5Source != null)
			{
				var sc5Destination = Path.Combine(sfm.ScenariosCustomFolder, Path.GetFileName(sc5Source));
				File.Copy(sc5Source, sc5Destination);

				var scenarioImported = await WaitUntilAsync(
					async () =>
					{
						using var scope = provider.CreateScope();
						var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
						return await db.Scenarios.AnyAsync();
					},
					TimeSpan.FromSeconds(30));

				Assert.That(scenarioImported, Is.True, "The dropped scenario SC5 was not imported in time");
			}

			// --- delete the object DAT; its database object must become unavailable ---
			File.Delete(datDestination);

			var objectUnavailable = await WaitUntilAsync(
				async () =>
				{
					using var scope = provider.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
					var obj = await db.Objects.FirstOrDefaultAsync();
					return obj != null
						&& obj.Availability == Definitions.ObjectAvailability.Unavailable
						&& sfm.ObjectIndex.Objects.Count == 0;
				},
				TimeSpan.FromSeconds(30));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(objectUnavailable, Is.True, "The deleted object DAT was not marked unavailable in time");
				Assert.That(sfm.ObjectIndex.Objects, Is.Empty);
			}

			await hostedService.StopAsync(CancellationToken.None);
		}
		finally
		{
			if (provider != null)
			{
				await provider.DisposeAsync();
			}

			SqliteConnection.ClearAllPools();
			TryDeleteDirectory(root);
		}
	}


	[Test]
	public async Task Watcher_StoresMusicFilesSeparatelyFromObjects()
	{
		var datSources = FindSourceFiles(SourceObjectsFolder, ".dat");
		if (datSources.Count < 2)
		{
			Assert.Ignore("Need at least two source DAT files to distinguish object from music handling");
		}

		var objectSource = datSources[0];
		var musicSource = datSources[1];

		var root = Directory.CreateTempSubdirectory("game-data-watcher-music").FullName;
		ServiceProvider? provider = null;
		try
		{
			var sfm = new ServerFolderManager(root);
			provider = BuildProvider(sfm, Path.Combine(root, "loco.db"));
			var hostedService = await StartAsync(provider);

			// Drop a genuine object first so we know the watcher service is definitely running.
			var objectDestination = Path.Combine(sfm.ObjectsCustomFolder, Path.GetFileName(objectSource));
			File.Copy(objectSource, objectDestination);

			var objectImported = await WaitUntilAsync(
				async () =>
				{
					using var scope = provider.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
					return sfm.ObjectIndex.Objects.Count == 1 && await db.Objects.AnyAsync();
				},
				TimeSpan.FromSeconds(30));
			Assert.That(objectImported, Is.True, "The dropped object DAT was not imported in time");

			// Now drop a different DAT into Music. Music is a distinct entity, so it must NOT be
			// added to the object index or the object tables - it is stored in its own Music table.
			var musicDestination = Path.Combine(sfm.MusicCustomFolder, Path.GetFileName(musicSource));
			File.Copy(musicSource, musicDestination);

			// Give the Music watcher more than enough time to process the event.
			await Task.Delay(TimeSpan.FromSeconds(3));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(sfm.ObjectIndex.Objects, Has.Count.EqualTo(1));

				var indexEntry = sfm.ObjectIndex.Objects.Single();
				Assert.That(
					Path.GetFullPath(Path.Combine(sfm.ObjectsFolder, indexEntry.FileName!)),
					Is.EqualTo(Path.GetFullPath(objectDestination)));
			}

			using (var scope = provider.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<LocoDbContext>();
				using (Assert.EnterMultipleScope())
				{
					Assert.That(await db.Objects.CountAsync(), Is.EqualTo(1));
					Assert.That(await db.Music.CountAsync(), Is.EqualTo(1));
				}
			}

			await hostedService.StopAsync(CancellationToken.None);
		}
		finally
		{
			if (provider != null)
			{
				await provider.DisposeAsync();
			}

			SqliteConnection.ClearAllPools();
			TryDeleteDirectory(root);
		}
	}

	private static void TryDeleteDirectory(string path)
	{
		try
		{
			Directory.Delete(path, recursive: true);
		}
		catch (IOException)
		{
			// Best-effort cleanup of the temp folder.
		}
		catch (UnauthorizedAccessException)
		{
			// Best-effort cleanup of the temp folder.
		}
	}


}
