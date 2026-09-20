// The raw SQL below uses hard-coded table-name arrays from the test itself, never user input.
#pragma warning disable EF1002

using Definitions.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Identity;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Verifies the scenario-pack table rename so databases created before the rename keep working.
/// </summary>
[TestFixture]
public class ScenarioPackTableInitializerTests
{
	[TestCase("SC5FilePacks", "ScenarioPacks")]
	[TestCase("sc5filepacks", "ScenarioPacks")]
	[TestCase("TblAuthorTblSC5FilePack", "TblAuthorTblScenarioPack")]
	[TestCase("TblSC5FilePackTblTag", "TblScenarioPackTblTag")]
	[TestCase("TblSC5File", null)]
	[TestCase("Scenarios", null)]
	public void GetRenamedTableName_MapsLegacyNames(string table, string? expected)
		=> Assert.That(ScenarioPackTableInitializer.GetRenamedTableName(table), Is.EqualTo(expected));

	[Test]
	public async Task EnsureRenamedAsync_RenamesLegacyTables()
	{
		var root = Directory.CreateTempSubdirectory("scenario-pack-rename").FullName;
		try
		{
			var dbPath = Path.Combine(root, "loco.db");
			var options = new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite($"Data Source={dbPath}")
				.Options;

			using var db = new LocoDbContext(options);
			_ = db.Database.EnsureCreated();

			// Simulate the legacy schema by replacing the scenario-pack tables and join tables with
			// the names they used to have.
			foreach (var table in new[] { "ScenarioPacks", "TblAuthorTblScenarioPack", "TblScenarioPackTblTag", "TblScenarioPackTblSC5File" })
			{
				_ = await db.Database.ExecuteSqlRawAsync($"DROP TABLE IF EXISTS \"{table}\"");
			}

			foreach (var table in new[] { "SC5FilePacks", "TblAuthorTblSC5FilePack", "TblSC5FilePackTblTag", "TblSC5FilePackTblSC5File" })
			{
				_ = await db.Database.ExecuteSqlRawAsync($"CREATE TABLE \"{table}\" (\"Id\" INTEGER NOT NULL PRIMARY KEY)");
			}

			// act
			await ScenarioPackTableInitializer.EnsureRenamedAsync(db, NullLogger.Instance);

			var tables = await db.Database
				.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table'")
				.ToListAsync();
			var tableSet = tables.ToHashSet(StringComparer.OrdinalIgnoreCase);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(tableSet, Does.Contain("ScenarioPacks"));
				Assert.That(tableSet, Does.Contain("TblAuthorTblScenarioPack"));
				Assert.That(tableSet, Does.Contain("TblScenarioPackTblTag"));
				Assert.That(tableSet, Does.Contain("TblScenarioPackTblSC5File"));

				Assert.That(tableSet, Does.Not.Contain("SC5FilePacks"));
				Assert.That(tableSet, Does.Not.Contain("TblAuthorTblSC5FilePack"));
			}

			// Running it again must be a no-op.
			await ScenarioPackTableInitializer.EnsureRenamedAsync(db, NullLogger.Instance);
			Assert.That(
				(await db.Database.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table'").ToListAsync())
					.ToHashSet(StringComparer.OrdinalIgnoreCase),
				Does.Contain("ScenarioPacks"));
		}
		finally
		{
			SqliteConnection.ClearAllPools();
			Directory.Delete(root, recursive: true);
		}
	}
}
