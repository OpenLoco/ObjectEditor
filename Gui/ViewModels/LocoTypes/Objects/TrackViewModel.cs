using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;
using TrackObject = Definitions.ObjectModels.Objects.Track.TrackObject;

namespace Gui.ViewModels;

public class TrackViewModel(TrackObject model)
	: LocoObjectViewModel<TrackObject>(model)
{
	[EnumProhibitValues<TrackObjectFlags>(TrackObjectFlags.None)]
	public TrackObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	[EnumProhibitValues<TrackTraitFlags>(TrackTraitFlags.None)]
	public TrackTraitFlags TrackPieces
	{
		get => Model.TrackPieces;
		set => Model.TrackPieces = value;
	}

	[EnumProhibitValues<TrackTraitFlags>(TrackTraitFlags.None)]
	public TrackTraitFlags StationTrackPieces
	{
		get => Model.StationTrackPieces;
		set => Model.StationTrackPieces = value;
	}

	public Speed16 CurveSpeed
	{
		get => Model.MaxCurveSpeed;
		set => Model.MaxCurveSpeed = value;
	}

	[Description("The vertical offset in pixels used to display a vehicle in a vehicle list (eg build menu, station train list, etc)")]
	public uint8_t VehicleDisplayListVerticalOffset
	{
		get => Model.VehicleDisplayListVerticalOffset;
		set => Model.VehicleDisplayListVerticalOffset = value;
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

	[Category("<unknown>")]
	public uint8_t var_06
	{
		get => Model.var_06;
		set => Model.var_06 = value;
	}

	[Category("Compatible Objects")]
	public ObjectModelHeader Tunnel
	{
		get => Model.Tunnel;
		set => Model.Tunnel = value;
	}

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> TracksAndRoads { get; init; } = new(model.TracksAndRoads);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> TrackExtras { get; init; } = new(model.TrackMods);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Signals { get; init; } = new(model.Signals);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Bridges { get; init; } = new(model.Bridges);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Stations { get; init; } = new(model.Stations);
}
