using Definitions.ObjectModels.Objects.Track;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using TrackObject = Definitions.ObjectModels.Objects.Track.TrackObject;

namespace Gui.ViewModels;

public class TrackViewModel : LocoObjectViewModel<TrackObject>
{
	[EnumProhibitValues<TrackObjectFlags>(TrackObjectFlags.None)] public TrackObjectFlags Flags { get; set; }
	[EnumProhibitValues<TrackTraitFlags>(TrackTraitFlags.None)] public TrackTraitFlags TrackPieces { get; set; }
	[EnumProhibitValues<TrackTraitFlags>(TrackTraitFlags.None)] public TrackTraitFlags StationTrackPieces { get; set; }
	public Speed16 CurveSpeed { get; set; }
	public uint8_t DisplayOffset { get; set; }
	public ObjectModelHeaderViewModel CompatibleTunnel { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Cost")] public int16_t TunnelCostFactor { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Tracks and Roads")] public BindingList<ObjectModelHeaderViewModel> CompatibleTracksAndRoads { get; set; }
	[Category("Mods")] public BindingList<ObjectModelHeaderViewModel> CompatibleTrackExtras { get; set; }
	[Category("Signals")] public BindingList<ObjectModelHeaderViewModel> CompatibleSignals { get; set; }
	[Category("Bridges")] public BindingList<ObjectModelHeaderViewModel> CompatibleBridges { get; set; }
	[Category("Stations")] public BindingList<ObjectModelHeaderViewModel> CompatibleStations { get; set; }
	[Category("<unknown>")] public uint8_t var_06 { get; set; }

	public TrackViewModel(TrackObject to)
	{
		Flags = to.Flags;
		TrackPieces = to.TrackPieces;
		StationTrackPieces = to.StationTrackPieces;
		CurveSpeed = to.MaxCurveSpeed;
		DisplayOffset = to.DisplayOffset;
		BuildCostFactor = to.BuildCostFactor;
		SellCostFactor = to.SellCostFactor;
		TunnelCostFactor = to.TunnelCostFactor;
		CostIndex = to.CostIndex;
		CompatibleTracksAndRoads = new(to.CompatibleTracksAndRoads.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleTrackExtras = new(to.TrackMods.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleSignals = new(to.Signals.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleTunnel = new(to.Tunnel);
		CompatibleBridges = new(to.Bridges.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleStations = new(to.Stations.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		var_06 = to.var_06;
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public override TrackObject GetAsModel()
		=> new()
		{
			Flags = Flags,
			TrackPieces = TrackPieces,
			StationTrackPieces = StationTrackPieces,
			MaxCurveSpeed = CurveSpeed,
			DisplayOffset = DisplayOffset,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			TunnelCostFactor = TunnelCostFactor,
			CostIndex = CostIndex,
			CompatibleTracksAndRoads = CompatibleTracksAndRoads.ToList().ConvertAll(x => x.GetAsModel()),
			TrackMods = CompatibleTrackExtras.ToList().ConvertAll(x => x.GetAsModel()),
			Signals = CompatibleSignals.ToList().ConvertAll(x => x.GetAsModel()),
			Tunnel = CompatibleTunnel.GetAsModel(),
			Bridges = CompatibleBridges.ToList().ConvertAll(x => x.GetAsModel()),
			Stations = CompatibleStations.ToList().ConvertAll(x => x.GetAsModel()),
			var_06 = var_06,
		};
}
