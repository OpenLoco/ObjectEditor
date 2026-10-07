using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Services;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Verifies that startup reconciliation never deletes a row when its file is missing: the row is kept
/// and its availability is flipped to <see cref="ObjectAvailability.Unavailable"/> (and back to
/// <see cref="ObjectAvailability.Available"/> when the file is present), and - because scenarios and
/// landscapes share the Scenarios table - that neither service touches the other's rows.
/// </summary>
[TestFixture]
public class ReconcileStaleRowTests
{
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
	public async Task MusicReconcile_MarksMissingFileUnavailable_AndRestoresPresentFile()
	{
		var root = Directory.CreateTempSubdirectory("reconcile-music").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var (db, connection) = CreateDb();
			using (connection)
			using (db)
			{
				// The present row starts Unavailable (so we can see it flip to Available) and the missing
				// row starts Available (so we can see it flip to Unavailable).
				var present = new TblMusic { Name = Path.Combine(ServerFolderManager.CustomFolderName, "here.mp3"), ObjectSource = ObjectSource.Custom, Availability = ObjectAvailability.Unavailable };
				var gone = new TblMusic { Name = Path.Combine(ServerFolderManager.CustomFolderName, "gone.mp3"), ObjectSource = ObjectSource.Custom, Availability = ObjectAvailability.Available };
				db.Music.AddRange(present, gone);
				_ = await db.SaveChangesAsync();

				var presentPath = Path.Combine(sfm.MusicFolder, present.Name);
				_ = Directory.CreateDirectory(Path.GetDirectoryName(presentPath)!);
				await File.WriteAllTextAsync(presentPath, "content");

				var service = new MusicFolderService(db, sfm, NullLogger<MusicFolderService>.Instance);
				await service.ReconcileAsync(CancellationToken.None);

				var rows = await db.Music.ToDictionaryAsync(x => x.Name, x => x.Availability);
				using (Assert.EnterMultipleScope())
				{
					Assert.That(rows, Has.Count.EqualTo(2), "both rows must be kept - the database is the source of truth");
					Assert.That(rows[present.Name], Is.EqualTo(ObjectAvailability.Available), "a row whose file exists is available");
					Assert.That(rows[gone.Name], Is.EqualTo(ObjectAvailability.Unavailable), "a row whose file is gone is unavailable, not deleted");
				}
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task ScenarioReconcile_MarksStaleScenarioUnavailable_ButNotLandscapeRows()
	{
		var root = Directory.CreateTempSubdirectory("reconcile-scenario").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var (db, connection) = CreateDb();
			using (connection)
			using (db)
			{
				var staleScenario = new TblScenario { Name = Path.Combine(ServerFolderManager.CustomFolderName, "gone.SC5"), ObjectSource = ObjectSource.Custom, Availability = ObjectAvailability.Available };
				var landscape = new TblScenario { Name = Path.Combine(ServerFolderManager.LandscapesFolderName, ServerFolderManager.CustomFolderName, "keep.SC5"), ObjectSource = ObjectSource.Custom, Availability = ObjectAvailability.Available };
				db.Scenarios.AddRange(staleScenario, landscape);
				_ = await db.SaveChangesAsync();

				var service = new ScenariosFolderService(db, sfm, NullLogger<ScenariosFolderService>.Instance);
				await service.ReconcileAsync(CancellationToken.None);

				var rows = await db.Scenarios.ToDictionaryAsync(x => x.Name, x => x.Availability);
				using (Assert.EnterMultipleScope())
				{
					Assert.That(rows, Has.Count.EqualTo(2), "neither row may be deleted");
					Assert.That(rows[staleScenario.Name], Is.EqualTo(ObjectAvailability.Unavailable), "a scenario whose file is gone is unavailable");
					Assert.That(rows[landscape.Name], Is.EqualTo(ObjectAvailability.Available), "the scenario service must not touch landscape rows");
				}
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}

	[Test]
	public async Task LandscapeReconcile_MarksStaleLandscapeUnavailable_ButNotScenarioRows()
	{
		var root = Directory.CreateTempSubdirectory("reconcile-landscape").FullName;
		try
		{
			var sfm = new ServerFolderManager(root);
			var (db, connection) = CreateDb();
			using (connection)
			using (db)
			{
				var staleLandscape = new TblScenario { Name = Path.Combine(ServerFolderManager.LandscapesFolderName, ServerFolderManager.CustomFolderName, "gone.SC5"), ObjectSource = ObjectSource.Custom, Availability = ObjectAvailability.Available };
				var scenario = new TblScenario { Name = Path.Combine(ServerFolderManager.CustomFolderName, "keep.SC5"), ObjectSource = ObjectSource.Custom, Availability = ObjectAvailability.Available };
				db.Scenarios.AddRange(staleLandscape, scenario);
				_ = await db.SaveChangesAsync();

				var service = new LandscapesFolderService(db, sfm, NullLogger<LandscapesFolderService>.Instance);
				await service.ReconcileAsync(CancellationToken.None);

				var rows = await db.Scenarios.ToDictionaryAsync(x => x.Name, x => x.Availability);
				using (Assert.EnterMultipleScope())
				{
					Assert.That(rows, Has.Count.EqualTo(2), "neither row may be deleted");
					Assert.That(rows[staleLandscape.Name], Is.EqualTo(ObjectAvailability.Unavailable), "a landscape whose file is gone is unavailable");
					Assert.That(rows[scenario.Name], Is.EqualTo(ObjectAvailability.Available), "the landscape service must not touch scenario rows");
				}
			}
		}
		finally
		{
			Directory.Delete(root, recursive: true);
		}
	}
}
