namespace Definitions;

// Indicates the origin of an object record from the perspective of the editor UI.
// Local = on the user's disk and tracked by the local SQLite index.
// Online = retrieved from a remote ObjectService.
[Flags]
public enum FileLocation
{
	Local,
	Online,
}
