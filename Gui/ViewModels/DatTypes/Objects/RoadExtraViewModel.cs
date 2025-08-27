using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;

namespace Gui.ViewModels;

public class RoadExtraViewModel : LocoObjectViewModel<RoadExtraObject>
{
	public RoadTraitFlags RoadPieces { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }

	public RoadExtraViewModel(RoadExtraObject obj)
	{
		RoadPieces = obj.RoadPieces;
		PaintStyle = obj.PaintStyle;
		CostIndex = obj.CostIndex;
		BuildCostFactor = obj.BuildCostFactor;
		SellCostFactor = obj.SellCostFactor;
	}

	public override RoadExtraObject GetAsModel()
		=> new()
		{
			RoadPieces = RoadPieces,
			PaintStyle = PaintStyle,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
		};
}
