using Definitions.ObjectModels.Objects.Bridge;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels;

public class BridgeViewModel : LocoObjectViewModel<BridgeObject>
{
	[EnumProhibitValues<BridgeObjectFlags>(BridgeObjectFlags.None)] public BridgeObjectFlags Flags { get; set; }
	public uint16_t ClearHeight { get; set; }
	public int16_t DeckDepth { get; set; }
	public uint8_t SpanLength { get; set; }
	public SupportPillarSpacing PillarPlacement { get; set; }
	public Speed16 MaxSpeed { get; set; }
	public MicroZ MaxHeight { get; set; }
	[EnumProhibitValues<BridgeDisabledTrackFlags>(BridgeDisabledTrackFlags.None)] public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
	public uint16_t DesignedYear { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public int16_t BaseCostFactor { get; set; }
	[Category("Cost")] public int16_t HeightCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Compatible")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleTrackObjects { get; set; }
	[Category("Compatible")] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleRoadObjects { get; set; }
	[Category("<unknown>")] public uint8_t var_03 { get; set; }

	public BridgeViewModel(BridgeObject model)
		: base(model)
	{
		Flags = model.Flags;
		SpanLength = model.SpanLength;
		PillarPlacement = (SupportPillarSpacing)model.PillarSpacing;
		MaxSpeed = model.MaxSpeed;
		MaxHeight = model.MaxHeight;
		CostIndex = model.CostIndex;
		BaseCostFactor = model.BaseCostFactor;
		HeightCostFactor = model.HeightCostFactor;
		SellCostFactor = model.SellCostFactor;
		DisabledTrackFlags = model.DisabledTrackFlags;
		DesignedYear = model.DesignedYear;
		CompatibleTrackObjects = new(model.CompatibleTrackObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleRoadObjects = new(model.CompatibleRoadObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		var_03 = model.var_03;
		ClearHeight = model.ClearHeight;
		DeckDepth = model.DeckDepth;
	}

	//public override BridgeObject CopyBackToModel()
	//	=> new()
	//	{
	//		Flags = Flags,
	//		SpanLength = SpanLength,
	//		PillarSpacing = PillarPlacement,
	//		MaxSpeed = MaxSpeed,
	//		MaxHeight = MaxHeight,
	//		CostIndex = CostIndex,
	//		BaseCostFactor = BaseCostFactor,
	//		HeightCostFactor = HeightCostFactor,
	//		SellCostFactor = SellCostFactor,
	//		DisabledTrackFlags = DisabledTrackFlags,
	//		DesignedYear = DesignedYear,
	//		CompatibleTrackObjects = CompatibleTrackObjects.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		CompatibleRoadObjects = CompatibleRoadObjects.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		var_03 = var_03,
	//		ClearHeight = ClearHeight,
	//		DeckDepth = DeckDepth,
	//	};
}
