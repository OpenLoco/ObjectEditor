using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObject : DbCoreObject
	{
		public ObjectSource ObjectSource { get; set; }

		public ObjectType ObjectType { get; set; }

		public VehicleType? VehicleType { get; set; }

		public ICollection<TblObjectPack> ObjectPacks { get; set; } // aka modpack

		public ICollection<TblDatObject> DatObjects { get; set; } // the DAT objects that created or reference this OpenLoco object. May be 0, may be multiple
	}
}
