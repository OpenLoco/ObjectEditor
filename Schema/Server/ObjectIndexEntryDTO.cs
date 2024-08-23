using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Schema.Server
{
	public class ObjectIndexEntryDTO
	{
		public string Filename { get; set; }
		public string ObjectName { get; set; }
		public ObjectType ObjectType { get; set; }
		public SourceGame SourceGame { get; set; }
		public uint Checksum { get; set; }
		public VehicleType? VehicleType { get; set; } = null;
	}
}
