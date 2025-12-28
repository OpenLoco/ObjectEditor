using Definitions.ObjectModels.Objects.Tree;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class TreeViewModel(TreeObject model)
	: LocoObjectViewModel<TreeObject>(model)
{
	[EnumProhibitValues<TreeObjectFlags>(TreeObjectFlags.None)]
	public TreeObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public uint8_t NumRotations
	{
		get => Model.NumRotations;
		set => Model.NumRotations = value;
	}

	public uint8_t NumGrowthStages
	{
		get => Model.NumGrowthStages;
		set => Model.NumGrowthStages = value;
	}

	public uint8_t SeasonState
	{
		get => Model.SeasonState;
		set => Model.SeasonState = value;
	}

	public uint8_t Season
	{
		get => Model.CurrentSeason;
		set => Model.CurrentSeason = value;
	}

	public uint32_t Colours
	{
		get => Model.Colours;
		set => Model.Colours = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}



	[Category("Building")]
	public uint8_t InitialHeight
	{
		get => Model.InitialHeight;
		set => Model.InitialHeight = value;
	}

	[Category("Building")]
	public uint8_t Height
	{
		get => Model.Height;
		set => Model.Height = value;
	}

	[Category("Building")]
	public int16_t Rating
	{
		get => Model.Rating;
		set => Model.Rating = value;
	}

	[Category("Building")]
	public int16_t DemolishRatingReduction
	{
		get => Model.DemolishRatingReduction;
		set => Model.DemolishRatingReduction = value;
	}

	[Category("<unknown>")]
	public uint8_t var_04
	{
		get => Model.var_04;
		set => Model.var_04 = value;
	}

	[Category("<unknown>")]
	public uint8_t var_05
	{
		get => Model.var_05;
		set => Model.var_05 = value;
	}

	[Category("<unknown>")]
	public TreeObjectSeasonalVariantFlags SeasonalVariants
	{
		get => Model.SeasonalVariants;
		set => Model.SeasonalVariants = value;
	}
}
