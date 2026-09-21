using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Services;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Exercises the SC5 (scenario/landscape) side of the file watcher: files dropped into
/// <c>GameData/Scenarios</c> must be recorded in the database, and removed again when deleted.
/// </summary>
[TestFixture]
public class ScenariosFolderServiceTests
{
	// The server's own scenario repository is used as the source of realistic test data.
	private const string ScenarioSourceFolder = @"Q:\Games\Locomotion\Server\GameData\Scenarios";

	private static string? FindSourceFile(string pattern)
	{
		if (!Directory.Exists(ScenarioSourceFolder))
		{
			return null;
		}

		return Directory
			.GetFiles(ScenarioSourceFolder, "*", SearchOption.AllDirectories)
			.Where(x => Path.GetExtension(x).Equals(pattern, StringComparison.OrdinalIgnoreCase))
			.OrderBy(x => new FileInfo(x).Length)
			.FirstOrDefault();
	}

	private static (LocoDbContext Db, SqliteConnection Connection) CreateDb()
	{
		var connection = new SqliteConnection("DataSource=:memory:");
		connection.Open();

		var options = new DbContextOptionsBuilder<LocoDbContext>()
			.UseSqlite(connection)
			.Options;

		var db = new LocoDbContext(options);
		_ = db.Database.EnsureCreated();
		return (db, connection);
	}

	[Test]
	public async Task ImportAsync_AddsScenarioToDatabase_AndRemovesItOnDelete()
	{
		var source = FindSourceFile(".SC5");
		if (source == null)
		{
			Assert.Ignore("No source scenarios are available to import");
		}

		var root = Directory.CreateTempSubdirectory("sc5-file-import").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var destination = Path.Combine(sfm.ScenariosCustomFolder, Path.GetFileName(source!));
			File.Copy(source!, destination);

			var (db, connection) = CreateDb();
			using (connection)
			using (db)
			{
				var service = new ScenariosFolderService(db, sfm, NullLogger<ScenariosFolderService>.Instance);

				var import = await service.ImportAsync(destination, CancellationToken.None);

				using (Assert.EnterMultipleScope())
				{
					Assert.That(import.Status, Is.EqualTo(GameDataImportStatus.Added));
					Assert.That(db.Scenarios.Count(), Is.EqualTo(1));
				}

				var row = await db.Scenarios.SingleAsync();
				using (Assert.EnterMultipleScope())
				{
					Assert.That(row.Name, Is.EqualTo(Path.GetRelativePath(sfm.ScenariosFolder, destination)));
					Assert.That(row.ObjectSource, Is.EqualTo(ObjectSource.Custom));
				}

				// re-importing an unchanged file must not create a duplicate row
				var reimport = await service.ImportAsync(destination, CancellationToken.None);
				Assert.That(reimport.Status, Is.EqualTo(GameDataImportStatus.Skipped));
				Assert.That(db.Scenarios.Count(), Is.EqualTo(1));

				File.Delete(destination);
				var remove = await service.RemoveAsync(destination, CancellationToken.None);

				using (Assert.EnterMultipleScope())
				{
					Assert.That(remove.Status, Is.EqualTo(GameDataImportStatus.Removed));
					Assert.That(db.Scenarios.Any(), Is.False);
				}
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task ImportAsync_SkipsSavegames()
	{
		var source = FindSourceFile(".SV5");
		if (source == null)
		{
			Assert.Ignore("No source SV5 savegames are available to import");
		}

		var root = Directory.CreateTempSubdirectory("sc5-file-savegame").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var destination = Path.Combine(sfm.ScenariosCustomFolder, Path.GetFileName(source!));
			File.Copy(source!, destination);

			var (db, connection) = CreateDb();
			using (connection)
			using (db)
			{
				var service = new ScenariosFolderService(db, sfm, NullLogger<ScenariosFolderService>.Instance);

				var result = await service.ImportAsync(destination, CancellationToken.None);

				using (Assert.EnterMultipleScope())
				{
					Assert.That(result.Status, Is.EqualTo(GameDataImportStatus.Skipped));
					Assert.That(db.Scenarios.Any(), Is.False);
				}
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}
}
