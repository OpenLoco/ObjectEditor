using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackExtra;

namespace Gui.ViewModels;

public class TrackExtraViewModel : LocoObjectViewModel<TrackExtraObject>
{
	public TrackTraitFlags TrackPieces { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }

	public TrackExtraViewModel(TrackExtraObject obj)
	{
		TrackPieces = obj.TrackPieces;
		PaintStyle = obj.PaintStyle;
		CostIndex = obj.CostIndex;
		BuildCostFactor = obj.BuildCostFactor;
		SellCostFactor = obj.SellCostFactor;
	}

	public override TrackExtraObject GetAsModel()
		=> new()
		{
			TrackPieces = TrackPieces,
			PaintStyle = PaintStyle,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
		};
}
