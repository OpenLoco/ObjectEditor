using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblLocoObject : DbCoreObject
	{
		public ObjectSource ObjectSource { get; set; }

		public ObjectType ObjectType { get; set; }

		public VehicleType? VehicleType { get; set; }

		public ICollection<TblLocoObjectPack> ObjectPacks { get; set; } // aka modpack

		public ICollection<TblObjectLookupFromDat> LinkedDatObjects { get; set; }
	}
}
