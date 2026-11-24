using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectRoadStation : DbSubObject, IConvertibleToTable<TblObjectRoadStation, RoadStationObject>
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	public RoadTraitFlags RoadPieces { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public RoadStationObjectFlags Flags { get; set; }
	public List<ObjectModelHeader> CompatibleRoadObjects { get; set; } = [];
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public ObjectModelHeader? CargoType { get; set; }
	public CargoOffset[][][] CargoOffsets { get; set; } = [];

	public static TblObjectRoadStation FromObject(TblObject tbl, RoadStationObject obj)
		=> new()
		{
			Parent = tbl,
			PaintStyle = obj.PaintStyle,
			Height = obj.Height,
			RoadPieces = obj.RoadPieces,
			BuildCostFactor = obj.BuildCostFactor,
			SellCostFactor = obj.SellCostFactor,
			CostIndex = obj.CostIndex,
			Flags = obj.Flags,
			CompatibleRoadObjects = obj.CompatibleRoadObjects,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			CargoType = obj.CargoType,
			CargoOffsets = obj.CargoOffsets,
		};
}
