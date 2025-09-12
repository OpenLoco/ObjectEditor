using Definitions.ObjectModels.Objects.Tree;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class TreeViewModel : LocoObjectViewModel<TreeObject>
{
	[EnumProhibitValues<TreeObjectFlags>(TreeObjectFlags.None)] public TreeObjectFlags Flags { get; set; }
	public uint8_t NumRotations { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public uint8_t SeasonState { get; set; }
	public uint8_t Season { get; set; }
	public uint32_t Colours { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t ClearCostFactor { get; set; }
	[Category("Building")] public uint8_t Clearance { get; set; }
	[Category("Building")] public uint8_t Height { get; set; }
	[Category("Building")] public int16_t Rating { get; set; }
	[Category("Building")] public int16_t DemolishRatingReduction { get; set; }
	[Category("<unknown>")] public uint8_t var_04 { get; set; }
	[Category("<unknown>")] public uint8_t var_05 { get; set; }
	[Category("<unknown>")] public TreeFlagsUnk var_3C { get; set; }

	public TreeViewModel(TreeObject to)
	{
		Flags = to.Flags;
		NumRotations = to.NumRotations;
		NumGrowthStages = to.NumGrowthStages;
		SeasonState = to.SeasonState;
		Season = to.Season;
		Colours = to.Colours;
		CostIndex = to.CostIndex;
		BuildCostFactor = to.BuildCostFactor;
		ClearCostFactor = to.ClearCostFactor;
		Clearance = to.Clearance;
		Height = to.Height;
		Rating = to.Rating;
		DemolishRatingReduction = to.DemolishRatingReduction;
		var_04 = to.var_04;
		var_05 = to.var_05;
		var_3C = to.var_3C;
	}

	public override TreeObject GetAsModel()
		=> new()
		{
			Flags = Flags,
			NumRotations = NumRotations,
			NumGrowthStages = NumGrowthStages,
			SeasonState = SeasonState,
			Season = Season,
			Colours = Colours,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			ClearCostFactor = ClearCostFactor,
			Clearance = Clearance,
			Height = Height,
			Rating = Rating,
			DemolishRatingReduction = DemolishRatingReduction,
			var_04 = var_04,
			var_05 = var_05,
			var_3C = var_3C,
		};
}
