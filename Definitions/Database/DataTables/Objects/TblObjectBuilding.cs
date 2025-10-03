using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;

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

	//public uint8_t NumBuildingParts { get; set; }
	//public uint8_t NumBuildingVariations { get; set; }
	//List<uint8_t> BuildingHeights { get; set; }
	//List<BuildingPartAnimation> BuildingAnimations { get; set; }
	//List<List<uint8_t>> BuildingVariations { get; set; }
	//public uint8_t[] ProducedQuantity { get; set; }
	//List<S5Header> ProducedCargo { get; set; }
	//List<S5Header> RequiredCargo { get; set; }
	//List<uint8_t> var_A6 { get; set; }
	//List<uint8_t> var_A8 { get; set; }
	//public uint8_t var_AC { get; set; }
	//public uint8_t NumElevatorSequences { get; set; }
	//List<uint8_t[]> _ElevatorHeightSequences // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)

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
		};
}
