using Definitions.ObjectModels.Objects.Road;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RoadViewModel : LocoObjectViewModel<RoadObject>
{
	[EnumProhibitValues<RoadObjectFlags>(RoadObjectFlags.None)] public RoadObjectFlags Flags { get; set; }
	[EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)] public RoadTraitFlags RoadPieces { get; set; }
	public Speed16 MaxSpeed { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t DisplayOffset { get; set; }
	public TownSize TargetTownSize { get; set; }
	public ObjectModelHeaderViewModel Tunnel { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Cost")] public int16_t TunnelCostFactor { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Bridges")] public ObservableCollection<ObjectModelHeaderViewModel> Bridges { get; set; }
	[Category("Stations")] public ObservableCollection<ObjectModelHeaderViewModel> Stations { get; set; }
	[Category("Mods")] public ObservableCollection<ObjectModelHeaderViewModel> Mods { get; set; }
	[Category("Compatible")] public ObservableCollection<ObjectModelHeaderViewModel> Compatible { get; set; }

	public RoadViewModel(RoadObject model)
		: base(model)
	{
		Flags = model.Flags;
		RoadPieces = model.RoadPieces;
		MaxSpeed = model.MaxCurveSpeed;
		PaintStyle = model.PaintStyle;
		DisplayOffset = model.DisplayOffset;
		TargetTownSize = model.TargetTownSize;
		BuildCostFactor = model.BuildCostFactor;
		SellCostFactor = model.SellCostFactor;
		TunnelCostFactor = model.TunnelCostFactor;
		CostIndex = model.CostIndex;
		Tunnel = new(model.Tunnel);
		Compatible = new(model.CompatibleTracksAndRoads.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		Mods = new(model.RoadMods.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		Bridges = new(model.Bridges.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		Stations = new(model.Stations.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
	}

	//public override RoadObject CopyBackToModel()
	//	=> new()
	//	{
	//		Flags = Flags,
	//		RoadPieces = RoadPieces,
	//		MaxCurveSpeed = MaxSpeed,
	//		PaintStyle = PaintStyle,
	//		DisplayOffset = DisplayOffset,
	//		TargetTownSize = TargetTownSize,
	//		BuildCostFactor = BuildCostFactor,
	//		SellCostFactor = SellCostFactor,
	//		TunnelCostFactor = TunnelCostFactor,
	//		CostIndex = CostIndex,
	//		CompatibleTracksAndRoads = Compatible.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		RoadMods = Mods.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		Tunnel = Tunnel.CopyBackToModel(),
	//		Bridges = Bridges.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		Stations = Stations.ToList().ConvertAll(x => x.CopyBackToModel()),
	//	};
}
