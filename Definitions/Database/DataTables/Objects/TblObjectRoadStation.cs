using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using System.Text.Json;

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
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint8_t pad_2D { get; set; }
	public string CargoOffsets { get; set; } = "[]";
	public UniqueObjectId? CargoTypeId { get; set; }
	public TblObjectCargo? CargoType { get; set; }

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
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			pad_2D = obj.pad_2D,
			CargoOffsets = JsonSerializer.Serialize(obj.CargoOffsets),
		};
}
