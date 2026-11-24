namespace Definitions.Database;

/// <summary>
/// Base for the file-backed entities: game objects and the files dropped into the GameData folders
/// (scenarios, music, sound effects, tutorials and graphics). Unlike an object/scenario pack, each of
/// these rows describes one file on disk, so it carries an <see cref="Availability"/>.
/// <para>
/// The database is the source of truth: a row is never deleted when its backing file disappears - it
/// is marked <see cref="ObjectAvailability.Unavailable"/> and kept, so curated metadata (tags,
/// authors, licence) and any pack/scenario references survive a removal. The reverse happens when the
/// file reappears.
/// </para>
/// </summary>
public abstract class DbFileObject : DbCoreObject
{
	/// <summary>
	/// Whether this row can be served to clients. Only <see cref="ObjectSource.Custom"/> content can ever
	/// be available: vanilla Locomotion (Steam/GoG) and OpenLoco content is placed on the server by hand
	/// and is never downloadable (or uploadable). For Custom content it reflects whether the file is
	/// present on disk. Defaults to <see cref="ObjectAvailability.Unavailable"/> (the enum's zero value);
	/// import and reconciliation set it to <see cref="ObjectAvailability.Available"/> when a Custom file
	/// is there. See <see cref="ObjectAvailabilityRules"/> for the single source of the rule.
	/// </summary>
	public ObjectAvailability Availability { get; set; }
}
