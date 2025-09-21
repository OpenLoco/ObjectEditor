using Definitions.ObjectModels.Objects.Track;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
	[Category("Tracks and Roads")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleTracksAndRoads { get; set; }
	[Category("Mods")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleTrackExtras { get; set; }
	[Category("Signals")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleSignals { get; set; }
	[Category("Bridges")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleBridges { get; set; }
	[Category("Stations")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleStations { get; set; }
	[Category("<unknown>")] public uint8_t var_06 { get; set; }

	public TrackViewModel(TrackObject model)
		: base(model)
	{
		Flags = model.Flags;
		TrackPieces = model.TrackPieces;
		StationTrackPieces = model.StationTrackPieces;
		CurveSpeed = model.MaxCurveSpeed;
		DisplayOffset = model.DisplayOffset;
		BuildCostFactor = model.BuildCostFactor;
		SellCostFactor = model.SellCostFactor;
		TunnelCostFactor = model.TunnelCostFactor;
		CostIndex = model.CostIndex;
		CompatibleTracksAndRoads = new(model.CompatibleTracksAndRoads.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleTrackExtras = new(model.TrackMods.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleSignals = new(model.Signals.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleTunnel = new(model.Tunnel);
		CompatibleBridges = new(model.Bridges.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleStations = new(model.Stations.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		var_06 = model.var_06;
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public TrackObject CopyBackToModel()
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
			//CompatibleTracksAndRoads = CompatibleTracksAndRoads.ToList().ConvertAll(x => x.CopyBackToModel()),
			//TrackMods = CompatibleTrackExtras.ToList().ConvertAll(x => x.CopyBackToModel()),
			//Signals = CompatibleSignals.ToList().ConvertAll(x => x.CopyBackToModel()),
			//Tunnel = CompatibleTunnel.CopyBackToModel(),
			//Bridges = CompatibleBridges.ToList().ConvertAll(x => x.CopyBackToModel()),
			//Stations = CompatibleStations.ToList().ConvertAll(x => x.CopyBackToModel()),
			var_06 = var_06,
		};
}
