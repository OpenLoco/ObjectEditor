using OpenLoco.Dat.Objects;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class TrackViewModel : LocoObjectViewModel<TrackObject>
	{
		[Reactive] public TrackObjectFlags Flags { get; set; }
		[Reactive] public TrackTraitFlags TrackPieces { get; set; }
		[Reactive] public TrackTraitFlags StationTrackPieces { get; set; }
		[Reactive] public Speed16 CurveSpeed { get; set; }
		[Reactive] public uint8_t DisplayOffset { get; set; }
		[Reactive] public S5HeaderViewModel Tunnel { get; set; }
		[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t TunnelCostFactor { get; set; }
		[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Compatible")] public BindingList<S5HeaderViewModel> Compatible { get; set; }
		[Reactive, Category("Mods")] public BindingList<S5HeaderViewModel> Mods { get; set; }
		[Reactive, Category("Signals")] public BindingList<S5HeaderViewModel> Signals { get; set; }
		[Reactive, Category("Bridges")] public BindingList<S5HeaderViewModel> Bridges { get; set; }
		[Reactive, Category("Stations")] public BindingList<S5HeaderViewModel> Stations { get; set; }
		[Reactive, Category("<unknown>")] public uint8_t var_06 { get; set; }
		[Reactive, Category("<unknown>")] public uint8_t var_35 { get; set; }

		public TrackViewModel(TrackObject to)
		{
			Flags = to.Flags;
			TrackPieces = to.TrackPieces;
			StationTrackPieces = to.StationTrackPieces;
			CurveSpeed = to.CurveSpeed;
			DisplayOffset = to.DisplayOffset;
			BuildCostFactor = to.BuildCostFactor;
			SellCostFactor = to.SellCostFactor;
			TunnelCostFactor = to.TunnelCostFactor;
			CostIndex = to.CostIndex;
			Compatible = new(to.Compatible.ConvertAll(x => new S5HeaderViewModel(x)));
			Mods = new(to.Mods.ConvertAll(x => new S5HeaderViewModel(x)));
			Signals = new(to.Signals.ConvertAll(x => new S5HeaderViewModel(x)));
			Tunnel = new(to.Tunnel);
			Bridges = new(to.Bridges.ConvertAll(x => new S5HeaderViewModel(x)));
			Stations = new(to.Stations.ConvertAll(x => new S5HeaderViewModel(x)));
			var_06 = to.var_06;
			var_35 = to.var_35;
		}

		// validation:
		// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
		public override TrackObject GetAsStruct(TrackObject to)
			=> to with
			{
				Flags = Flags,
				TrackPieces = TrackPieces,
				StationTrackPieces = StationTrackPieces,
				CurveSpeed = CurveSpeed,
				DisplayOffset = DisplayOffset,
				BuildCostFactor = BuildCostFactor,
				SellCostFactor = SellCostFactor,
				TunnelCostFactor = TunnelCostFactor,
				CostIndex = CostIndex,
				NumCompatible = (uint8_t)Compatible.Count,
				Compatible = Compatible.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				NumMods = (uint8_t)Mods.Count,
				Mods = Mods.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				NumSignals = (uint8_t)Signals.Count,
				Signals = Signals.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				Tunnel = Tunnel.GetAsUnderlyingType(),
				NumBridges = (uint8_t)Bridges.Count,
				Bridges = Bridges.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				NumStations = (uint8_t)Stations.Count,
				Stations = Stations.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				var_06 = var_06,
				var_35 = var_35,
			};
	}
}
