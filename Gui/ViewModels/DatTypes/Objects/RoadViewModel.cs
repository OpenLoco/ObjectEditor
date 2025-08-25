using Definitions.ObjectModels.Objects.Road;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class RoadViewModel : LocoObjectViewModel<RoadObject>
{
	[Reactive, EnumProhibitValues<RoadObjectFlags>(RoadObjectFlags.None)] public RoadObjectFlags Flags { get; set; }
	[Reactive, EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)] public RoadTraitFlags RoadPieces { get; set; }
	[Reactive] public Speed16 MaxSpeed { get; set; }
	[Reactive] public uint8_t PaintStyle { get; set; }
	[Reactive] public uint8_t DisplayOffset { get; set; }
	[Reactive] public TownSize TargetTownSize { get; set; }
	[Reactive] public ObjectModelHeaderViewModel Tunnel { get; set; }
	[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Reactive, Category("Cost")] public int16_t TunnelCostFactor { get; set; }
	[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
	[Reactive, Category("Bridges")] public BindingList<ObjectModelHeaderViewModel> Bridges { get; set; }
	[Reactive, Category("Stations")] public BindingList<ObjectModelHeaderViewModel> Stations { get; set; }
	[Reactive, Category("Mods")] public BindingList<ObjectModelHeaderViewModel> Mods { get; set; }
	[Reactive, Category("Compatible")] public BindingList<ObjectModelHeaderViewModel> Compatible { get; set; }

	public RoadViewModel(RoadObject ro)
	{
		Flags = ro.Flags;
		RoadPieces = ro.RoadPieces;
		MaxSpeed = ro.MaxCurveSpeed;
		PaintStyle = ro.PaintStyle;
		DisplayOffset = ro.DisplayOffset;
		TargetTownSize = ro.TargetTownSize;
		BuildCostFactor = ro.BuildCostFactor;
		SellCostFactor = ro.SellCostFactor;
		TunnelCostFactor = ro.TunnelCostFactor;
		CostIndex = ro.CostIndex;
		Tunnel = new(ro.Tunnel);
		Compatible = new(ro.CompatibleTracksAndRoads.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		Mods = new(ro.RoadMods.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		Bridges = new(ro.Bridges.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		Stations = new(ro.Stations.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
	}

	public override RoadObject GetAsStruct()
		=> new()
		{
			Flags = Flags,
			RoadPieces = RoadPieces,
			MaxCurveSpeed = MaxSpeed,
			PaintStyle = PaintStyle,
			DisplayOffset = DisplayOffset,
			TargetTownSize = TargetTownSize,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			TunnelCostFactor = TunnelCostFactor,
			CostIndex = CostIndex,
			CompatibleTracksAndRoads = Compatible.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			RoadMods = Mods.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			Tunnel = Tunnel.GetAsUnderlyingType(),
			Bridges = Bridges.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			Stations = Stations.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
		};
}
