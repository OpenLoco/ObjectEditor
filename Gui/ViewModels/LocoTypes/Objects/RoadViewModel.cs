using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RoadViewModel(RoadObject model)
	: LocoObjectViewModel<RoadObject>(model)
{
	[EnumProhibitValues<RoadObjectFlags>(RoadObjectFlags.None)]
	public RoadObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	[EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)]
	public RoadTraitFlags RoadPieces
	{
		get => Model.RoadPieces;
		set => Model.RoadPieces = value;
	}

	public Speed16 MaxSpeed
	{
		get => Model.MaxCurveSpeed;
		set => Model.MaxCurveSpeed = value;
	}

	public uint8_t PaintStyle
	{
		get => Model.PaintStyle;
		set => Model.PaintStyle = value;
	}

	[Description("The vertical offset in pixels used to display a vehicle in a vehicle list (eg build menu, station train list, etc)")]
	public uint8_t VehicleDisplayListVerticalOffset
	{
		get => Model.VehicleDisplayListVerticalOffset;
		set => Model.VehicleDisplayListVerticalOffset = value;
	}

	public TownSize TargetTownSize
	{
		get => Model.TargetTownSize;
		set => Model.TargetTownSize = value;
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

	[Category("Cost")]
	public int16_t TunnelCostFactor
	{
		get => Model.TunnelCostFactor;
		set => Model.TunnelCostFactor = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
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

	[Category("Cost"), ReadOnly(true), DisplayName("Effective Tunnel Cost"), Description("The inflation-adjusted tunnel cost for the year specified in settings")]
	public int EffectiveTunnelCost
	{
		get
		{
			var year = GlobalSettings.CurrentSettings?.InflationYear ?? 1950;
			return Common.Economy.GetInflationAdjustedCost(Model.TunnelCostFactor, Model.CostIndex, year);
		}
	}

	[Category("Compatible Objects")]
	public ObjectModelHeader Tunnel
	{
		get => Model.Tunnel;
		set => Model.Tunnel = value;
	}

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Bridges { get; set; } = new(model.Bridges);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Stations { get; set; } = new(model.Stations);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Mods { get; set; } = new(model.RoadMods);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> TracksAndRoads { get; set; } = new(model.TracksAndRoads);
}
