using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(DatName), nameof(DatChecksum), IsDescending = [true, false], IsUnique = true)]
	public class TblLocoObject : DbCoreObject
	{
		#region OriginalDatData

		public required string DatName { get; set; }

		public required uint DatChecksum { get; set; }

		#endregion

		public ObjectSource SourceGame { get; set; }

		public ObjectType ObjectType { get; set; }

		public VehicleType? VehicleType { get; set; }

		public ObjectAvailability Availability { get; set; }

		public ICollection<TblLocoObjectPack> ObjectPacks { get; set; } // aka modpack
	}
}
