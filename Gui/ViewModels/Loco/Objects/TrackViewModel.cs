using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;
using TrackObject = Definitions.ObjectModels.Objects.Track.TrackObject;

namespace Gui.ViewModels;

public class TrackViewModel(TrackObject model)
	: BaseViewModel<TrackObject>(model)
{
	TunnelReferenceViewModel? tunnel = model.Tunnel is { } tunnelModel ? new(tunnelModel) : null;

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

	public Speed16 MaxCurveSpeed
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
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set
		{
			Model.CostIndex = value;
			RaisePropertiesChanged(nameof(BuildCostFactor), nameof(SellCostFactor), nameof(TunnelCostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(10, nameof(CostIndex))]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(10, nameof(CostIndex))]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(10, nameof(CostIndex))]
	public int16_t TunnelCostFactor
	{
		get => Model.TunnelCostFactor;
		set => Model.TunnelCostFactor = value;
	}

	[Category("<unknown>")]
	public uint8_t var_06
	{
		get => Model.var_06;
		set => Model.var_06 = value;
	}

	[Category("Compatible Objects")]
	public TunnelReferenceViewModel? Tunnel
	{
		get => tunnel;
		set
		{
			tunnel = value;
			if (value != null)
			{
				Model.Tunnel = value.Model;
			}
		}
	}

	[Category("Compatible Objects")]
	public ObjectReferenceList<TrackOrRoadReferenceViewModel> TracksAndRoads { get; init; } = new(model.TracksAndRoads, h => new TrackOrRoadReferenceViewModel(h), list => model.TracksAndRoads = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<TrackExtraReferenceViewModel> TrackExtras { get; init; } = new(model.TrackMods, h => new TrackExtraReferenceViewModel(h), list => model.TrackMods = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<TrackSignalReferenceViewModel> Signals { get; init; } = new(model.Signals, h => new TrackSignalReferenceViewModel(h), list => model.Signals = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<BridgeReferenceViewModel> Bridges { get; init; } = new(model.Bridges, h => new BridgeReferenceViewModel(h), list => model.Bridges = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<TrackStationReferenceViewModel> Stations { get; init; } = new(model.Stations, h => new TrackStationReferenceViewModel(h), list => model.Stations = [.. list]);

	public override void SynchronizeToModel()
	{
		TracksAndRoads.Synchronize();
		TrackExtras.Synchronize();
		Signals.Synchronize();
		Bridges.Synchronize();
		Stations.Synchronize();
	}
}
