using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RoadExtraViewModel(RoadExtraObject model)
	: LocoObjectViewModel<RoadExtraObject>(model)
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
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost")]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Cost"), ReadOnly(true), DisplayName("Effective Build Cost"), Description("The inflation-adjusted build cost for the year specified in settings")]
	public int EffectiveBuildCost
	{
		get
		{
			var year = GlobalSettings.CurrentSettings?.InflationYear ?? 1950;
			return Common.Economy.GetInflationAdjustedCost(Model.BuildCostFactor, Model.CostIndex, year);
		}
	}

	[Category("Cost"), ReadOnly(true), DisplayName("Effective Sell Cost"), Description("The inflation-adjusted sell cost for the year specified in settings")]
	public int EffectiveSellCost
	{
		get
		{
			var year = GlobalSettings.CurrentSettings?.InflationYear ?? 1950;
			return Common.Economy.GetInflationAdjustedCost(Model.SellCostFactor, Model.CostIndex, year);
		}
	}
}
