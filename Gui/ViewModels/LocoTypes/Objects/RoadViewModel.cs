using Definitions.ObjectModels.Objects.Road;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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

	public override RoadObject GetAsModel()
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
			CompatibleTracksAndRoads = Compatible.ToList().ConvertAll(x => x.GetAsModel()),
			RoadMods = Mods.ToList().ConvertAll(x => x.GetAsModel()),
			Tunnel = Tunnel.GetAsModel(),
			Bridges = Bridges.ToList().ConvertAll(x => x.GetAsModel()),
			Stations = Stations.ToList().ConvertAll(x => x.GetAsModel()),
		};
}
