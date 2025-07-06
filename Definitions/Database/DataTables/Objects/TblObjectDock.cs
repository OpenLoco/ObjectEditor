using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectDock : DbSubObject, IConvertibleToTable<TblObjectDock, DockObject>
	{
		public int16_t BuildCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint8_t CostIndex { get; set; }
		public DockObjectFlags Flags { get; set; }
		public uint8_t NumBuildingPartAnimations { get; set; }
		public uint8_t NumBuildingVariationParts { get; set; }
		public uint16_t DesignedYear { get; set; }
		public uint16_t ObsoleteYear { get; set; }

		// these map to [Pos2 BoatPosition] in the Dock object
		public coord_t BoatPositionX { get; set; }
		public coord_t BoatPositionY { get; set; }

		// public uint8_t var_07 { get; set; } // probably padding
		// public List<uint8_t> BuildingPartHeights { get; set; }
		// public List<uint16_t> BuildingPartAnimations { get; set; }
		// public List<uint8_t> BuildingVariationParts { get; set; }

		public static TblObjectDock FromObject(TblObject tbl, DockObject obj)
			=> new()
			{
				Parent = tbl,
				BuildCostFactor = obj.BuildCostFactor,
				SellCostFactor = obj.SellCostFactor,
				CostIndex = obj.CostIndex,
				Flags = obj.Flags,
				NumBuildingPartAnimations = obj.NumBuildingPartAnimations,
				NumBuildingVariationParts = obj.NumBuildingVariationParts,
				DesignedYear = obj.DesignedYear,
				ObsoleteYear = obj.ObsoleteYear,
				BoatPositionX = obj.BoatPosition.X,
				BoatPositionY = obj.BoatPosition.Y,
			};
	}
}
