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

	public uint8_t DisplayOffset
	{
		get => Model.DisplayOffset;
		set => Model.DisplayOffset = value;
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
	public BindingList<ObjectModelHeader> TracksAndRoads { get; set; } = new(model.TracksAndRoads);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> TrackExtras { get; set; } = new(model.TrackMods);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Signals { get; set; } = new(model.Signals);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Bridges { get; set; } = new(model.Bridges);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Stations { get; set; } = new(model.Stations);
}
