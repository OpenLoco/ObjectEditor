using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectBuilding : DbSubObject, IConvertibleToTable<TblObjectBuilding, BuildingObject>
{
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public BuildingObjectFlags Flags { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint16_t SellCostFactor { get; set; }
	public int16_t DemolishRatingReduction { get; set; }
	public uint8_t ScaffoldingSegmentType { get; set; }
	public Colour ScaffoldingColour { get; set; }
	public uint32_t Colours { get; set; }
	public uint8_t GeneratorFunction { get; set; }
	public uint8_t AverageNumberOnMap { get; set; }

	public BuildingComponents BuildingComponents { get; set; } = new();
	public List<uint8_t> ProducedQuantity { get; set; } = [];
	public List<ObjectModelHeader> ProducedCargoType { get; set; } = [];
	public List<ObjectModelHeader> ConsumedCargoType { get; set; } = [];
	public List<uint8_t> ProducedCargoQuantity { get; set; } = [];
	public List<uint8_t> ConsumedCargoQuantity { get; set; } = [];
	public TownAmenityCategory TownAmenityCategory { get; set; }
	public List<uint8_t[]> ElevatorHeightSequences { get; set; } = [];

	public static TblObjectBuilding FromObject(TblObject tbl, BuildingObject obj)
		=> new()
		{
			Parent = tbl,
			DesignedYear = obj.DesignedYear,
			ObsoleteYear = obj.ObsoleteYear,
			Flags = obj.Flags,
			CostIndex = obj.CostIndex,
			SellCostFactor = obj.SellCostFactor,
			DemolishRatingReduction = obj.DemolishRatingReduction,
			ScaffoldingSegmentType = obj.ScaffoldingSegmentType,
			ScaffoldingColour = obj.ScaffoldingColour,
			Colours = obj.Colours,
			GeneratorFunction = obj.GeneratorFunction,
			AverageNumberOnMap = obj.AverageNumberOnMap,
			BuildingComponents = obj.BuildingComponents,
			ProducedQuantity = obj.ProducedQuantity,
			ProducedCargoType = obj.ProducedCargoType,
			ConsumedCargoType = obj.ConsumedCargoType,
			ProducedCargoQuantity = obj.ProducedCargoQuantity,
			ConsumedCargoQuantity = obj.ConsumedCargoQuantity,
			TownAmenityCategory = obj.TownAmenityCategory,
			ElevatorHeightSequences = obj.ElevatorHeightSequences,
		};
}
