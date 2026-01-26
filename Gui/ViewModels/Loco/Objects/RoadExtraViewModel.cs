using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RoadExtraViewModel(RoadExtraObject model)
	: BaseViewModel<RoadExtraObject>(model)
{
	[EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)]
	public RoadTraitFlags RoadPieces
	{
		get => Model.RoadPieces;
		set => Model.RoadPieces = value;
	}

	public uint8_t PaintStyle
	{
		get => Model.PaintStyle;
		set => Model.PaintStyle = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set
		{
			Model.CostIndex = value;
			this.RaisePropertyChanged(nameof(BuildCostFactor));
			this.RaisePropertyChanged(nameof(SellCostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(nameof(CostIndex))]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(nameof(CostIndex))]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}
}
