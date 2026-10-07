using Definitions.Database;
using Definitions.DTO.Mappers;
using Definitions.ObjectModels.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ObjectService.Identity;
using System.Reflection;

namespace ObjectService.Tests.Integration;

/// <summary>
/// Locks the object-data structure: every game object (all 34 types) keeps its generic information in a
/// single header row in <c>Objects</c> and its type-specific information in one sub-object table named
/// after its <see cref="ObjectType"/>, so the type alone identifies the table.
/// </summary>
[TestFixture]
public class DbSubObjectStructureTests
{
	/// <summary>
	/// The sub-object table for an object type is named <c>Obj&lt;suffix&gt;</c> and typed
	/// <c>TblObject&lt;suffix&gt;</c>, where the suffix comes from
	/// <see cref="DbSubObjectHelper.GetTableSuffix"/> - the same helper the server uses, so this test locks
	/// the production convention rather than a copy of it.
	/// </summary>
	static string ExpectedName(ObjectType objectType)
		=> DbSubObjectHelper.GetTableSuffix(objectType);

	/// <summary>The <c>Obj&lt;ObjectType&gt;</c> DbSets exposed on the context, i.e. one per object type.</summary>
	static List<(string DbSet, Type Entity)> SubObjectTables
		=> [.. typeof(LocoDbContext)
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
			.Select(p => (DbSet: p.Name, Entity: p.PropertyType.GetGenericArguments()[0]))
			.Where(x => typeof(DbSubObject).IsAssignableFrom(x.Entity))
			.OrderBy(x => x.DbSet)];

	[Test]
	public void EveryObjectTypeMapsToOneSubObjectTableAndBack()
	{
		var expected = Enum.GetValues<ObjectType>().Select(t => "Obj" + ExpectedName(t)).ToList();
		var actual = SubObjectTables.Select(x => x.DbSet).ToList();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(expected, Has.Count.EqualTo(34), "there are 34 game object types");
			Assert.That(actual, Is.EquivalentTo(expected), "every ObjectType must map to its own Obj<ObjectType> table");
			Assert.That(actual, Is.Unique, "each sub-object table belongs to exactly one ObjectType");

			// The table type has to agree too, since that is what the server resolves for validation.
			foreach (var objectType in Enum.GetValues<ObjectType>())
			{
				Assert.That(DbSubObjectHelper.GetSubObjectTableType(objectType), Is.EqualTo(SubObjectTables.Single(x => x.DbSet == "Obj" + ExpectedName(objectType)).Entity), $"ObjectType.{objectType}");
			}
		}
	}

	[Test]
	public void EverySubObjectTableIsBackedByADbSubObjectThatLinksToTheHeaderRow()
	{
		foreach (var (dbSet, entity) in SubObjectTables)
		{
			var suffix = dbSet["Obj".Length..];

			using (Assert.EnterMultipleScope())
			{
				Assert.That(typeof(DbSubObject).IsAssignableFrom(entity), Is.True, $"{dbSet} must derive from DbSubObject");
				Assert.That(entity.Name, Is.EqualTo("TblObject" + suffix), $"{dbSet} should be backed by TblObject{suffix}");
				Assert.That(entity.GetProperty(nameof(DbSubObject.Parent)), Is.Not.Null, $"{dbSet} must link back to its Objects header row via Parent");
			}
		}
	}

	/// <summary>
	/// Proves each object type resolves to a real table the row lookup can query - a type the switch in
	/// <see cref="DbSubObjectHelper.GetSubObjectRowAsync"/> has no arm for throws instead of quietly
	/// returning nothing.
	/// </summary>
	[Test]
	public async Task EveryObjectTypeHasARealTableThatTheRowLookupCanQuery()
	{
		var dbPath = Path.Combine(Path.GetTempPath(), $"structure-test-{Guid.NewGuid():N}", "loco.db");
		_ = Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

		try
		{
			await using var db = new LocoDbContext(new DbContextOptionsBuilder<LocoDbContext>()
				.UseSqlite($"Data Source={dbPath}")
				.Options);

			await MigrationInitializer.EnsureBaselineHistoryAsync(db, NullLogger.Instance);
			await db.Database.MigrateAsync();

			var tables = new HashSet<string>(
				await db.Database
					.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table'")
					.ToListAsync(),
				StringComparer.OrdinalIgnoreCase);

			using (Assert.EnterMultipleScope())
			{
				foreach (var objectType in Enum.GetValues<ObjectType>())
				{
					var table = "Obj" + ExpectedName(objectType);

					Assert.That(tables, Does.Contain(table), $"ObjectType.{objectType} should have a table named {table}");
					Assert.That(await DbSubObjectHelper.GetSubObjectRowAsync(db, objectType, 12345), Is.Null, $"the {table} row lookup must be wired for ObjectType.{objectType}");
				}
			}
		}
		finally
		{
			SqliteConnection.ClearAllPools();

			var dir = Path.GetDirectoryName(dbPath);
			if (dir is not null && Directory.Exists(dir))
			{
				Directory.Delete(dir, recursive: true);
			}
		}
	}

	/// <summary>
	/// A sub-object DTO is accepted only by the object type whose table it maps to, which is what stops a
	/// request from writing e.g. airport rows for a vehicle object.
	/// </summary>
	[Test]
	public void SubObjectsAreAcceptedOnlyByTheirOwnObjectType()
	{
		var dtoTypes = typeof(IDtoSubObject).Assembly
			.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && typeof(IDtoSubObject).IsAssignableFrom(t))
			.ToList();

		Assert.That(dtoTypes, Has.Count.EqualTo(34));

		foreach (var dtoType in dtoTypes)
		{
			var dto = (IDtoSubObject)Activator.CreateInstance(dtoType)!;
			var entityType = SubObjectDtoMapper.FindMapperOrNull(dtoType)!.ReturnType;
			var ownerType = Enum.GetValues<ObjectType>().Single(t => DbSubObjectHelper.GetSubObjectTableType(t) == entityType);

			foreach (var objectType in Enum.GetValues<ObjectType>())
			{
				Assert.That(DbSubObjectHelper.IsSubObjectOfType(dto, objectType), Is.EqualTo(objectType == ownerType), $"{dtoType.Name} vs ObjectType.{objectType}");
			}
		}
	}
}