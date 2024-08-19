using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Dat.FileParsing
{
	public class ObjectIndex
	{
		public const int JsonVersion = 1; // change this every time this format changes
		public int Version => JsonVersion;

		public required IEnumerable<ObjectIndexEntry> Objects { get; set; }

		public required IEnumerable<ObjectIndexFailedEntry> ObjectsFailed { get; set; }
	}

	public abstract record ObjectIndexEntryBase(string Filename);

	public record ObjectIndexEntry(string Filename, string ObjectName, ObjectType ObjectType, SourceGame SourceGame, uint32_t Checksum, VehicleType? VehicleType = null)
		: ObjectIndexEntryBase(Filename);

	public record ObjectIndexFailedEntry(string Filename)
		: ObjectIndexEntryBase(Filename);
}
