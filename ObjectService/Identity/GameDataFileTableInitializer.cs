using Definitions.Database;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ObjectService.Identity;

/// <summary>
/// <para>
/// Creates the per-entity game-data file tables (<c>Music</c>, <c>SoundEffects</c>,
/// <c>Tutorials</c>, <c>Graphics</c>) and their many-to-many join tables if they are missing.
/// <c>EnsureCreated</c> only creates the schema for a brand new database, so existing databases
/// need these tables added explicitly.
/// </para>
/// <para>
/// The CREATE statements are taken from EF's own model-generated create script rather than being
/// hand-written, so the upgrade can never drift from the entity definitions.
/// </para>
/// </summary>
public static class GameDataFileTableInitializer
{
	private static readonly Type[] Entities =
	[
		typeof(TblMusic), typeof(TblSoundEffect), typeof(TblTutorial), typeof(TblGraphics),
	];

	/// <summary>
	/// The tables required by the game-data file entities, including the many-to-many join tables
	/// EF generates for their authors/tags.
	/// </summary>
	public static IReadOnlyCollection<string> GetRequiredTables(LocoDbContext db)
	{
		var tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		foreach (var clrType in Entities)
		{
			var entity = db.Model.FindEntityType(clrType);
			if (entity == null)
			{
				continue;
			}

			if (entity.GetTableName() is { Length: > 0 } table)
			{
				_ = tables.Add(table);
			}

			foreach (var skipNavigation in entity.GetSkipNavigations())
			{
				if (skipNavigation.JoinEntityType is { } joinEntity && joinEntity.GetTableName() is { Length: > 0 } joinTable)
				{
					_ = tables.Add(joinTable);
				}
			}
		}

		return tables;
	}

	public static async Task EnsureTablesAsync(LocoDbContext db, ILogger logger)
	{
		var requiredTables = GetRequiredTables(db);
		if (requiredTables.Count == 0)
		{
			return;
		}

		var statements = SplitStatements(db.Database.GenerateCreateScript()).ToList();
		var existingTables = await GetExistingTableNamesAsync(db).ConfigureAwait(false);

		foreach (var table in requiredTables.Where(t => !existingTables.Contains(t)))
		{
			var createStatement = statements.FirstOrDefault(s => IsCreateTableFor(s, table));
			if (createStatement == null)
			{
				logger.LogWarning("No CREATE TABLE statement was generated for {Table}", table);
				continue;
			}

			await db.Database.ExecuteSqlRawAsync(createStatement).ConfigureAwait(false);
			logger.LogInformation("Created missing table {Table}", table);

			foreach (var indexStatement in statements.Where(s => IsIndexOnTable(s, table)))
			{
				await db.Database.ExecuteSqlRawAsync(indexStatement).ConfigureAwait(false);
			}
		}
	}

	private static async Task<HashSet<string>> GetExistingTableNamesAsync(LocoDbContext db)
	{
		var names = await db.Database
			.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table'")
			.ToListAsync()
			.ConfigureAwait(false);

		return new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
	}

	/// <summary>Splits EF's create script into individual statements.</summary>
	private static IEnumerable<string> SplitStatements(string script)
		=> Regex
			.Split(script, @";\s*(?=\r?\n|$)")
			.Select(statement => statement.Trim())
			.Where(statement => statement.Length > 0);

	private static bool IsCreateTableFor(string statement, string table)
		=> Regex.IsMatch(statement, $@"^CREATE TABLE ""?{Regex.Escape(table)}""?\s*\(", RegexOptions.IgnoreCase);

	private static bool IsIndexOnTable(string statement, string table)
		=> Regex.IsMatch(statement, $@"^CREATE (?:UNIQUE )?INDEX .+? ON ""?{Regex.Escape(table)}""?\s*\(", RegexOptions.IgnoreCase);
}
