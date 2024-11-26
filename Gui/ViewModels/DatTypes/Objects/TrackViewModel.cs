using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class TrackViewModel : ReactiveObject, IObjectViewModel<ILocoStruct>
	{
		[Reactive] public TrackObjectFlags Flags { get; set; }
		[Reactive] public TrackTraitFlags TrackPieces { get; set; }
		[Reactive] public TrackTraitFlags StationTrackPieces { get; set; }
		[Reactive] public Speed16 CurveSpeed { get; set; }
		[Reactive] public uint8_t DisplayOffset { get; set; }
		[Reactive] public S5Header Tunnel { get; set; }
		[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t TunnelCostFactor { get; set; }
		[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Compatible")] public BindingList<S5Header> Compatible { get; set; }
		[Reactive, Category("Mods")] public BindingList<S5Header> Mods { get; set; }
		[Reactive, Category("Signals")] public BindingList<S5Header> Signals { get; set; }
		[Reactive, Category("Bridges")] public BindingList<S5Header> Bridges { get; set; }
		[Reactive, Category("Stations")] public BindingList<S5Header> Stations { get; set; }
		[Reactive, Category("<unknown>")] public uint8_t var_06 { get; set; }
		[Reactive, Category("<unknown>")] public uint8_t var_35 { get; set; }

		public TrackViewModel(TrackObject to)
		{
			Flags = to.Flags;
			TrackPieces = to.TrackPieces;
			StationTrackPieces = to.StationTrackPieces;
			BuildCostFactor = to.BuildCostFactor;
			SellCostFactor = to.SellCostFactor;
			TunnelCostFactor = to.TunnelCostFactor;
			CostIndex = to.CostIndex;
			CurveSpeed = to.CurveSpeed;
			DisplayOffset = to.DisplayOffset;
			Tunnel = to.Tunnel;
			var_06 = to.var_06;
			var_35 = to.var_35;
			Compatible = new(to.Compatible);
			Mods = new(to.Mods);
			Signals = new(to.Signals);
			Bridges = new(to.Bridges);
			Stations = new(to.Stations);
		}

		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
			=> GetAsStruct((locoStruct as TrackObject)!);

		// validation:
		// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
		public TrackObject GetAsStruct(TrackObject to)
			=> to with
			{
				Flags = Flags,
				TrackPieces = TrackPieces,
				NumCompatible = (uint8_t)Compatible.Count(),
				NumMods = (uint8_t)Mods.Count(),
				NumBridges = (uint8_t)Bridges.Count(),
				NumSignals = (uint8_t)Signals.Count(),
				NumStations = (uint8_t)Stations.Count(),
				StationTrackPieces = StationTrackPieces,
				BuildCostFactor = BuildCostFactor,
				SellCostFactor = SellCostFactor,
				TunnelCostFactor = TunnelCostFactor,
				CostIndex = CostIndex,
				CurveSpeed = CurveSpeed,
				DisplayOffset = DisplayOffset,
				Compatible = [.. Compatible],
				Mods = [.. Mods],
				Signals = [.. Signals],
				Bridges = [.. Bridges],
				Stations = [.. Stations],
				Tunnel = Tunnel,
				var_06 = var_06,
				var_35 = var_35,
			};
	}
}
