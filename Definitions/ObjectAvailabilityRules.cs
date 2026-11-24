using Definitions.ObjectModels.Types;

namespace Definitions;

/// <summary>
/// The rules that decide an entity's <see cref="ObjectAvailability"/> and whether it may be downloaded.
/// <para>
/// Only <see cref="ObjectSource.Custom"/> content is ever downloadable (and, by extension, uploadable):
/// vanilla Locomotion (<see cref="ObjectSource.LocomotionSteam"/> / <see cref="ObjectSource.LocomotionGoG"/>)
/// and <see cref="ObjectSource.OpenLoco"/> content is placed on the server by hand and is never served to
/// clients. For Custom content the availability additionally reflects whether the file is present on disk.
/// </para>
/// </summary>
public static class ObjectAvailabilityRules
{
	/// <summary>
	/// The availability of a file-backed row: <see cref="ObjectAvailability.Available"/> only when the
	/// content is Custom and its file is present on disk, otherwise
	/// <see cref="ObjectAvailability.Unavailable"/> (non-Custom content is never available).
	/// </summary>
	public static ObjectAvailability ForFile(ObjectSource source, bool fileExists)
		=> source == ObjectSource.Custom && fileExists
			? ObjectAvailability.Available
			: ObjectAvailability.Unavailable;

	/// <summary>
	/// Whether an entity of the given source and availability may be downloaded from the server. Only a
	/// <see cref="ObjectAvailability.Available"/> Custom entity is downloadable; vanilla and OpenLoco
	/// content is always refused.
	/// </summary>
	public static bool IsDownloadable(ObjectSource source, ObjectAvailability availability)
		=> source == ObjectSource.Custom && availability == ObjectAvailability.Available;
}
