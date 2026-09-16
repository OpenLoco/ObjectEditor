using Definitions.ObjectModels.Objects.Tree;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels;

public class TreeViewModel(TreeObject model)
	: BaseViewModel<TreeObject>(model)
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
		set
		{
			Model.CostIndex = value;
			this.RaisePropertyChanged(nameof(BuildCostFactor));
			this.RaisePropertyChanged(nameof(ClearCostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(12, nameof(CostIndex))]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(12, nameof(CostIndex))]
	public int16_t ClearCostFactor
	{
		get => Model.ClearCostFactor;
		set => Model.ClearCostFactor = value;
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

	[Category("Building")]
	public uint8_t MinHeight
	{
		get => Model.MinHeight;
		set => Model.MinHeight = value;
	}

	[Category("Building")]
	public uint8_t MaxHeight
	{
		get => Model.MaxHeight;
		set => Model.MaxHeight = value;
	}

	[Category("Building")]
	public TreeObjectVariantFlags VariantFlags
	{
		get => Model.VariantFlags;
		set => Model.VariantFlags = value;
	}
}
