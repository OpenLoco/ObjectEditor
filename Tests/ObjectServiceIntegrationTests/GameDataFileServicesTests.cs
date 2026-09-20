using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Services;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Verifies that each of the four file-based entity services persists files into its own table and
/// removes them again - confirming music, sound effects, tutorials and graphics are stored as
/// distinct entities rather than as game objects.
/// </summary>
[TestFixture]
public class GameDataFileServicesTests
{
	private static (ServerFolderManager Sfm, LocoDbContext Db, SqliteConnection Connection, string Root) CreateServer(string prefix)
	{
		var root = Directory.CreateTempSubdirectory(prefix).FullName;
		var sfm = new ServerFolderManager(root);

		var connection = new SqliteConnection("DataSource=:memory:");
		connection.Open();

		var options = new DbContextOptionsBuilder<LocoDbContext>()
			.UseSqlite(connection)
			.Options;

		var db = new LocoDbContext(options);
		_ = db.Database.EnsureCreated();

		return (sfm, db, connection, root);
	}

	private static async Task AssertPersistsAndRemovesAsync(IGameDataFileService service, string categoryFolder, IQueryable<DbCoreObject> query)
	{
		var path = Path.Combine(categoryFolder, ServerFolderManager.CustomFolderName, "file.dat");
		await File.WriteAllTextAsync(path, "content");

		// act - import
		var import = await service.ImportAsync(path, CancellationToken.None);
		Assert.That(import.Status, Is.EqualTo(GameDataImportStatus.Added));

		var row = await query.SingleAsync();
		using (Assert.EnterMultipleScope())
		{
			Assert.That(row.Name, Is.EqualTo(Path.GetRelativePath(categoryFolder, path)));
			Assert.That(row.ObjectSource, Is.EqualTo(ObjectSource.Custom));
		}

		// re-importing an unchanged file is a no-op
		var reimport = await service.ImportAsync(path, CancellationToken.None);
		Assert.That(reimport.Status, Is.EqualTo(GameDataImportStatus.Skipped));
		Assert.That(await query.CountAsync(), Is.EqualTo(1));

		// act - delete
		File.Delete(path);
		var remove = await service.RemoveAsync(path, CancellationToken.None);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(remove.Status, Is.EqualTo(GameDataImportStatus.Removed));
			Assert.That(await query.AnyAsync(), Is.False);
		}
	}

	[Test]
	public async Task FileServices_PersistIntoTheirOwnTables()
	{
		var (sfm, db, connection, root) = CreateServer("game-data-file-services");
		using (connection)
		using (db)
		{
			try
			{
				var music = new MusicFolderService(db, sfm, NullLogger<MusicFolderService>.Instance);
				var soundEffects = new SoundEffectsFolderService(db, sfm, NullLogger<SoundEffectsFolderService>.Instance);
				var tutorials = new TutorialsFolderService(db, sfm, NullLogger<TutorialsFolderService>.Instance);
				var graphics = new GraphicsFolderService(db, sfm, NullLogger<GraphicsFolderService>.Instance);

				await AssertPersistsAndRemovesAsync(music, sfm.MusicFolder, db.Music);
				await AssertPersistsAndRemovesAsync(soundEffects, sfm.SoundEffectsFolder, db.SoundEffects);
				await AssertPersistsAndRemovesAsync(tutorials, sfm.TutorialsFolder, db.Tutorials);
				await AssertPersistsAndRemovesAsync(graphics, sfm.GraphicsFolder, db.Graphics);

				// A file dropped into Music must never end up in the object tables.
				Assert.That(db.Objects.Any(), Is.False);
			}
			finally
			{
				Directory.Delete(root, recursive: true);
			}
		}
	}
}
