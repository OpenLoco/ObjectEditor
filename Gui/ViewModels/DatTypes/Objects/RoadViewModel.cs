using Definitions.ObjectModels.Objects.Road;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

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
		//Tunnel = new(ro.Tunnel);
		//Compatible = new(ro.Compatible.ConvertAll(x => new S5HeaderViewModel(x)));
		//Mods = new(ro.Mods.ConvertAll(x => new S5HeaderViewModel(x)));
		//Bridges = new(ro.Bridges.ConvertAll(x => new S5HeaderViewModel(x)));
		//Stations = new(ro.Stations.ConvertAll(x => new S5HeaderViewModel(x)));
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
			//NumCompatible = (uint8_t)Compatible.Count,
			//Compatible = Compatible.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			//NumMods = (uint8_t)Mods.Count,
			//Mods = Mods.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			//Tunnel = Tunnel.GetAsUnderlyingType(),
			//NumBridges = (uint8_t)Bridges.Count,
			//Bridges = Bridges.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
			//NumStations = (uint8_t)Stations.Count,
			//Stations = Stations.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
		};
}
