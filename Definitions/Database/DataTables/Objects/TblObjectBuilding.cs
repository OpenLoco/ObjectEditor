using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;
using System.Text.Json;

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
	public string TownAmenityCategory { get; set; } = string.Empty;
	public string BuildingComponents { get; set; } = "null";
	public string ProducedQuantity { get; set; } = "[]";
	public string ProducedCargoQuantity { get; set; } = "[]";
	public string ConsumedCargoQuantity { get; set; } = "[]";
	public string ElevatorHeightSequences { get; set; } = "[]";
	public string ProducedCargoType { get; set; } = "[]";
	public string ConsumedCargoType { get; set; } = "[]";

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
			TownAmenityCategory = obj.TownAmenityCategory.ToString(),
			BuildingComponents = JsonSerializer.Serialize(obj.BuildingComponents),
			ProducedQuantity = JsonSerializer.Serialize(obj.ProducedQuantity),
			ProducedCargoQuantity = JsonSerializer.Serialize(obj.ProducedCargoQuantity),
			ConsumedCargoQuantity = JsonSerializer.Serialize(obj.ConsumedCargoQuantity),
			ElevatorHeightSequences = JsonSerializer.Serialize(obj.ElevatorHeightSequences),
			ProducedCargoType = JsonSerializer.Serialize(obj.ProducedCargoType),
			ConsumedCargoType = JsonSerializer.Serialize(obj.ConsumedCargoType),
		};
}
