using Definitions.ObjectModels.Objects.Tree;

namespace Definitions.Database;

public class TblObjectTree : DbSubObject, IConvertibleToTable<TblObjectTree, TreeObject>
{
	public uint8_t InitialHeight { get; set; }
	public uint8_t Height { get; set; }
	public uint8_t MinHeight { get; set; }
	public uint8_t MaxHeight { get; set; }
	public uint8_t NumRotations { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public TreeObjectFlags Flags { get; set; }
	public uint16_t ShadowImageOffset { get; set; }
	public uint8_t SeasonState { get; set; }
	public uint8_t CurrentSeason { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t ClearCostFactor { get; set; }
	public uint32_t Colours { get; set; }
	public int16_t Rating { get; set; }
	public int16_t DemolishRatingReduction { get; set; }
	public TreeObjectVariantFlags VariantFlags { get; set; } // bitset of which of the six sprites[] variants have their own images

	public static TblObjectTree FromObject(TblObject tbl, TreeObject obj)
		=> new()
		{
			Parent = tbl,
			InitialHeight = obj.InitialHeight,
			Height = obj.Height,
			NumRotations = obj.NumRotations,
			NumGrowthStages = obj.NumGrowthStages,
			Flags = obj.Flags,
			ShadowImageOffset = obj.ShadowImageOffset,
			SeasonState = obj.SeasonState,
			CurrentSeason = obj.CurrentSeason,
			CostIndex = obj.CostIndex,
			BuildCostFactor = obj.BuildCostFactor,
			ClearCostFactor = obj.ClearCostFactor,
			Colours = obj.Colours,
			Rating = obj.Rating,
			DemolishRatingReduction = obj.DemolishRatingReduction,
		};
}
