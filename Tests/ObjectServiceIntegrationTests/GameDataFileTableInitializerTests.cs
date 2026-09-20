// The raw SQL below uses hard-coded table-name arrays from the test itself, never user input.
#pragma warning disable EF1002


using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Identity;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Verifies that the schema upgrade adds the per-entity game-data file tables (and their join
/// tables) to a database created before those entities existed.
/// </summary>
[TestFixture]
public class GameDataFileTableInitializerTests
{
	[Test]
	public async Task EnsureTablesAsync_CreatesTablesMissingFromAnOlderDatabase()
	{
		var root = Directory.CreateTempSubdirectory("game-data-schema").FullName;
		try
		{
			var dbPath = Path.Combine(root, "loco.db");
			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite($"Data Source={dbPath}")
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			var requiredTables = GameDataFileTableInitializer.GetRequiredTables(db);
			Assert.That(requiredTables, Is.Not.Empty);

			// Simulate a database created before these entities existed. Join tables are dropped
			// first so their foreign keys never dangle.
			var joinTables = requiredTables
				.Where(t => t.Contains("TblAuthor", StringComparison.OrdinalIgnoreCase) || t.Contains("TblTag", StringComparison.OrdinalIgnoreCase))
				.ToList();
			var entityTables = requiredTables.Except(joinTables);

			foreach (var table in joinTables.Concat(entityTables))
			{
				_ = await db.Database.ExecuteSqlRawAsync($"DROP TABLE \"{table}\"");
			}

			// act
			await GameDataFileTableInitializer.EnsureTablesAsync(db, NullLogger.Instance);

			// assert - the tables (including their join tables) now exist and the EF model
			// round-trips them, proving the generated DDL matches what EF expects.
			var author = new TblAuthor { Name = "Author" };
			var tag = new TblTag { Name = "Tag" };

			var music = new TblMusic { Name = "Custom/a.dat", ObjectSource = ObjectSource.Custom };
			music.Authors.Add(author);
			music.Tags.Add(tag);
			_ = db.Music.Add(music);
			_ = db.SoundEffects.Add(new TblSoundEffect { Name = "Custom/b.dat", ObjectSource = ObjectSource.Custom });
			_ = db.Tutorials.Add(new TblTutorial { Name = "Custom/c.dat", ObjectSource = ObjectSource.Custom });
			_ = db.Graphics.Add(new TblGraphics { Name = "Custom/d.dat", ObjectSource = ObjectSource.Custom });
			_ = await db.SaveChangesAsync();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(await db.Music.CountAsync(), Is.EqualTo(1));
				Assert.That(await db.SoundEffects.CountAsync(), Is.EqualTo(1));
				Assert.That(await db.Tutorials.CountAsync(), Is.EqualTo(1));
				Assert.That(await db.Graphics.CountAsync(), Is.EqualTo(1));
			}

			var reloaded = await db.Music.Include(m => m.Authors).Include(m => m.Tags).SingleAsync();
			using (Assert.EnterMultipleScope())
			{
				Assert.That(reloaded.Authors, Has.Count.EqualTo(1));
				Assert.That(reloaded.Tags, Has.Count.EqualTo(1));
				// The computed UploadedDate must have been filled in by the table default.
				Assert.That(reloaded.UploadedDate, Is.Not.EqualTo(default(DateOnly)));
			}

			// Running it again must be a no-op.
			await GameDataFileTableInitializer.EnsureTablesAsync(db, NullLogger.Instance);
			Assert.That(await db.Music.CountAsync(), Is.EqualTo(1));
		}
		finally
		{
			SqliteConnection.ClearAllPools();
			Directory.Delete(root, recursive: true);
		}
	}
}
