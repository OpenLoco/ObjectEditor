using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Identity;

/// <summary>
/// Renames the scenario-pack tables that were originally named after the SC5 file entity, so
/// databases created before the rename keep working (<c>EnsureCreated</c> does not alter existing
/// schemas). Join tables are renamed by replacing the entity-name token in their names.
/// </summary>
public static class ScenarioPackTableInitializer
{
	private const string OldTableName = "SC5FilePacks";
	private const string NewTableName = "ScenarioPacks";
	private const string OldEntityToken = "TblSC5FilePack";
	private const string NewEntityToken = "TblScenarioPack";

	public static async Task EnsureRenamedAsync(LocoDbContext db, ILogger logger)
	{
		var tables = await db.Database
			.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table'")
			.ToListAsync()
			.ConfigureAwait(false);

		var existing = new HashSet<string>(tables, StringComparer.OrdinalIgnoreCase);

		foreach (var table in tables)
		{
			var newName = GetRenamedTableName(table);
			if (newName == null || existing.Contains(newName))
			{
				continue;
			}

#pragma warning disable EF1002 // table names come from sqlite_master, not from user input
			await db.Database
				.ExecuteSqlRawAsync($"ALTER TABLE \"{table}\" RENAME TO \"{newName}\"")
				.ConfigureAwait(false);
#pragma warning restore EF1002

			logger.LogInformation("Renamed table {OldTable} to {NewTable}", table, newName);
		}
	}

	/// <summary>Returns the new name for a table that needs renaming, or null when it doesn't.</summary>
	public static string? GetRenamedTableName(string table)
	{
		if (table.Equals(OldTableName, StringComparison.OrdinalIgnoreCase))
		{
			return NewTableName;
		}

		return table.Contains(OldEntityToken, StringComparison.Ordinal)
			? table.Replace(OldEntityToken, NewEntityToken, StringComparison.Ordinal)
			: null;
	}
}
