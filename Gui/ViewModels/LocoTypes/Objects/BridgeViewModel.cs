using Definitions.ObjectModels.Objects.Bridge;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

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

	public BridgeViewModel(BridgeObject bo)
	{
		Flags = bo.Flags;
		SpanLength = bo.SpanLength;
		PillarPlacement = (SupportPillarSpacing)bo.PillarSpacing;
		MaxSpeed = bo.MaxSpeed;
		MaxHeight = bo.MaxHeight;
		CostIndex = bo.CostIndex;
		BaseCostFactor = bo.BaseCostFactor;
		HeightCostFactor = bo.HeightCostFactor;
		SellCostFactor = bo.SellCostFactor;
		DisabledTrackFlags = bo.DisabledTrackFlags;
		DesignedYear = bo.DesignedYear;
		CompatibleTrackObjects = new(bo.CompatibleTrackObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CompatibleRoadObjects = new(bo.CompatibleRoadObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		var_03 = bo.var_03;
		ClearHeight = bo.ClearHeight;
		DeckDepth = bo.DeckDepth;
	}

	public override BridgeObject GetAsModel()
		=> new()
		{
			Flags = Flags,
			SpanLength = SpanLength,
			PillarSpacing = PillarPlacement,
			MaxSpeed = MaxSpeed,
			MaxHeight = MaxHeight,
			CostIndex = CostIndex,
			BaseCostFactor = BaseCostFactor,
			HeightCostFactor = HeightCostFactor,
			SellCostFactor = SellCostFactor,
			DisabledTrackFlags = DisabledTrackFlags,
			DesignedYear = DesignedYear,
			CompatibleTrackObjects = CompatibleTrackObjects.ToList().ConvertAll(x => x.GetAsModel()),
			CompatibleRoadObjects = CompatibleRoadObjects.ToList().ConvertAll(x => x.GetAsModel()),
			var_03 = var_03,
			ClearHeight = ClearHeight,
			DeckDepth = DeckDepth,
		};
}
