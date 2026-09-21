using Definitions.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Identity;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Verifies the migration baseline behaves correctly for the three database states we care about:
/// brand new, created before migrations were adopted (no <c>__EFMigrationsHistory</c>), and already
/// migration-managed.
/// </summary>
[TestFixture]
public class DatabaseMigrationTests
{
	static string CreateTempDbPath()
	{
		var dir = Path.Combine(Path.GetTempPath(), $"migration-test-{Guid.NewGuid():N}");
		_ = Directory.CreateDirectory(dir);
		return Path.Combine(dir, "loco.db");
	}

	static void CleanUp(string dbPath)
	{
		SqliteConnection.ClearAllPools();
		var dir = Path.GetDirectoryName(dbPath);
		if (dir is not null && Directory.Exists(dir))
		{
			Directory.Delete(dir, recursive: true);
		}
	}

	static LocoDbContext CreateContext(string dbPath)
		=> new(new DbContextOptionsBuilder<LocoDbContext>()
			.UseSqlite($"Data Source={dbPath}")
			.Options);

	/// <summary>Reproduces a database created before migrations were adopted.</summary>
	static async Task CreateLegacyDatabaseAsync(string dbPath)
	{
		await using var legacy = CreateContext(dbPath);
		_ = await legacy.Database.EnsureCreatedAsync();
	}

	static async Task<HashSet<string>> GetTableNamesAsync(LocoDbContext db)
	{
		var names = await db.Database
			.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table'")
			.ToListAsync();

		return new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
	}

	static async Task<List<string>> GetHistoryRowsAsync(LocoDbContext db)
		=> await db.Database
			.SqlQueryRaw<string>($"SELECT \"MigrationId\" AS \"Value\" FROM \"{MigrationInitializer.HistoryTableName}\"")
			.ToListAsync();

	[Test]
	public async Task Migrate_CreatesFullSchemaForFreshDatabase()
	{
		var dbPath = CreateTempDbPath();
		try
		{
			await using var db = CreateContext(dbPath);

			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);
			await db.Database.MigrateAsync();

			var tables = await GetTableNamesAsync(db);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(tables, Does.Contain("Objects"));
				Assert.That(tables, Does.Contain("ScenarioPacks"));
				Assert.That(tables, Does.Contain("Music"));
				Assert.That(tables, Does.Contain("SoundEffects"));
				Assert.That(tables, Does.Contain("Tutorials"));
				Assert.That(tables, Does.Contain("Graphics"));
				Assert.That(tables, Does.Contain(MigrationInitializer.HistoryTableName));
				Assert.That(await db.Database.GetPendingMigrationsAsync(), Is.Empty);
			}
		}
		finally
		{
			CleanUp(dbPath);
		}
	}

	[Test]
	public async Task EnsureBaselineHistoryAsync_DoesNotTouchBrandNewDatabase()
	{
		var dbPath = CreateTempDbPath();
		try
		{
			await using var db = CreateContext(dbPath);
			await db.Database.OpenConnectionAsync();

			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);

			// Nothing to seed - Migrate() will create the schema from the baseline.
			Assert.That(await GetTableNamesAsync(db), Does.Not.Contain(MigrationInitializer.HistoryTableName));
		}
		finally
		{
			CleanUp(dbPath);
		}
	}

	[Test]
	public async Task EnsureBaselineHistoryAsync_SeedsBaselineForLegacyDatabase()
	{
		var dbPath = CreateTempDbPath();
		try
		{
			await CreateLegacyDatabaseAsync(dbPath);

			await using (var legacy = CreateContext(dbPath))
			{
				Assert.That(await GetTableNamesAsync(legacy), Does.Not.Contain(MigrationInitializer.HistoryTableName));
			}

			await using var db = CreateContext(dbPath);
			var baseline = MigrationInitializer.GetBaselineMigrationId(db);
			Assert.That(baseline, Is.Not.Null);

			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(await GetHistoryRowsAsync(db), Is.EqualTo(new[] { baseline }));
			}

			// Migrate() must now be a no-op that leaves the existing schema in place.
			await db.Database.MigrateAsync();

			var tables = await GetTableNamesAsync(db);
			using (Assert.EnterMultipleScope())
			{
				Assert.That(tables, Does.Contain("Objects"));
				Assert.That(tables, Does.Contain("ScenarioPacks"));
				Assert.That(await db.Database.GetPendingMigrationsAsync(), Is.Empty);
			}
		}
		finally
		{
			CleanUp(dbPath);
		}
	}

	[Test]
	public async Task EnsureBaselineHistoryAsync_IsIdempotent()
	{
		var dbPath = CreateTempDbPath();
		try
		{
			await CreateLegacyDatabaseAsync(dbPath);

			await using var db = CreateContext(dbPath);

			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);
			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);

			Assert.That(await GetHistoryRowsAsync(db), Has.Count.EqualTo(1));
		}
		finally
		{
			CleanUp(dbPath);
		}
	}

	[Test]
	public async Task EnsureBaselineHistoryAsync_IsNoOpWhenAlreadyMigrated()
	{
		var dbPath = CreateTempDbPath();
		try
		{
			await using var db = CreateContext(dbPath);
			await db.Database.MigrateAsync();

			var before = await GetHistoryRowsAsync(db);

			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);

			Assert.That(await GetHistoryRowsAsync(db), Is.EqualTo(before));
		}
		finally
		{
			CleanUp(dbPath);
		}
	}
}