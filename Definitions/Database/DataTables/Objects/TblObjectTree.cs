using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectTree : DbSubObject, IConvertibleToTable<TblObjectTree, TreeObject>
	{
		public uint8_t Clearance { get; set; }
		public uint8_t Height { get; set; }
		public uint8_t NumRotations { get; set; }
		public uint8_t NumGrowthStages { get; set; }
		public TreeObjectFlags Flags { get; set; }
		public uint16_t ShadowImageOffset { get; set; }
		public uint8_t SeasonState { get; set; }
		public uint8_t Season { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BuildCostFactor { get; set; }
		public int16_t ClearCostFactor { get; set; }
		public uint32_t Colours { get; set; }
		public int16_t Rating { get; set; }
		public int16_t DemolishRatingReduction { get; set; }

		//public uint8_t var_04 { get; set; }
		//public uint8_t var_05 { get; set; }
		//public UnkTreeFlags var_3C { get; set; } // something with images

		public static TblObjectTree FromObject(TblObject tbl, TreeObject obj)
			=> new()
			{
				Parent = tbl,
				Clearance = obj.Clearance,
				Height = obj.Height,
				NumRotations = obj.NumRotations,
				NumGrowthStages = obj.NumGrowthStages,
				Flags = obj.Flags,
				ShadowImageOffset = obj.ShadowImageOffset,
				SeasonState = obj.SeasonState,
				Season = obj.Season,
				CostIndex = obj.CostIndex,
				BuildCostFactor = obj.BuildCostFactor,
				ClearCostFactor = obj.ClearCostFactor,
				Colours = obj.Colours,
				Rating = obj.Rating,
				DemolishRatingReduction = obj.DemolishRatingReduction,
			};
	}
}
