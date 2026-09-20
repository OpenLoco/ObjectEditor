using Definitions;
using Definitions.Database;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UniqueObjectId = System.UInt64;

namespace Tests;

/// <summary>
/// Verifies the insert/update behaviour of <see cref="DbSubObjectHelper.AddOrUpdate"/> so an existing
/// sub-object row is actually updated (keeping its primary key) instead of being silently left as-is.
/// </summary>
[TestFixture]
public class DbSubObjectHelperTests
{
	static async Task<(LocoDbContext Db, SqliteConnection Connection)> CreateDbAsync()
	{
		var connection = new SqliteConnection("DataSource=:memory:");
		await connection.OpenAsync();
		var options = new DbContextOptionsBuilder<LocoDbContext>().UseSqlite(connection).Options;
		var db = new LocoDbContext(options);
		_ = await db.Database.EnsureCreatedAsync();
		return (db, connection);
	}

	static TblObject CreateParent(UniqueObjectId id)
		=> new()
		{
			Id = id,
			Name = $"test-airport-{id}",
			ObjectType = ObjectType.Airport,
			ObjectSource = ObjectSource.Custom,
			Availability = ObjectAvailability.Available,
		};

	[Test]
	public async Task AddOrUpdate_UpdatesExistingSubObjectAndKeepsItsId()
	{
		var (db, connection) = await CreateDbAsync();
		await using (db)
		using (connection)
		{
			var parent = CreateParent(1);
			_ = await db.Objects.AddAsync(parent);
			_ = await db.SaveChangesAsync();

			var existing = new TblObjectAirport { Parent = parent, BuildCostFactor = 100 };
			_ = await db.ObjAirport.AddAsync(existing);
			_ = await db.SaveChangesAsync();

			parent.SubObjectId = existing.Id;
			_ = await db.SaveChangesAsync();
			var existingId = existing.Id;

			var incoming = new TblObjectAirport { Parent = parent, BuildCostFactor = 200 };
			var result = await DbSubObjectHelper.AddOrUpdate(db, parent, incoming);
			_ = await db.SaveChangesAsync();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.EqualTo($"Updated {parent.Id}-{existingId}"));
				Assert.That(parent.SubObjectId, Is.EqualTo(existingId));
				Assert.That(await db.ObjAirport.CountAsync(), Is.EqualTo(1));

				var reloaded = await db.ObjAirport.AsNoTracking().SingleAsync(x => x.Id == existingId);
				Assert.That(reloaded.BuildCostFactor, Is.EqualTo((short)200));
			}
		}
	}

	[Test]
	public async Task AddOrUpdate_InsertsSubObjectWhenNoneExists()
	{
		var (db, connection) = await CreateDbAsync();
		await using (db)
		using (connection)
		{
			var parent = CreateParent(1);
			_ = await db.Objects.AddAsync(parent);
			_ = await db.SaveChangesAsync();

			var incoming = new TblObjectAirport { Parent = parent, BuildCostFactor = 150 };
			var result = await DbSubObjectHelper.AddOrUpdate(db, parent, incoming);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Does.StartWith($"Added {parent.Id}-"));
				Assert.That(parent.SubObjectId, Is.Not.Zero);
				Assert.That(await db.ObjAirport.CountAsync(), Is.EqualTo(1));

				var reloaded = await db.ObjAirport.AsNoTracking().SingleAsync();
				Assert.That(reloaded.Id, Is.EqualTo(parent.SubObjectId));
				Assert.That(reloaded.BuildCostFactor, Is.EqualTo((short)150));
			}
		}
	}
}