using Definitions.ObjectModels.Objects.LevelCrossing;

namespace Gui.ViewModels;

public class LevelCrossingViewModel : LocoObjectViewModel<LevelCrossingObject>
{
	public int16_t CostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t ClosingFrames { get; set; }
	public uint8_t ClosedFrames { get; set; }
	public uint8_t var_0A { get; set; }
	public uint16_t DesignedYear { get; set; }

	public LevelCrossingViewModel(LevelCrossingObject obj)
	{
		CostFactor = obj.CostFactor;
		SellCostFactor = obj.SellCostFactor;
		CostIndex = obj.CostIndex;
		AnimationSpeed = obj.AnimationSpeed;
		ClosingFrames = obj.ClosingFrames;
		ClosedFrames = obj.ClosedFrames;
		var_0A = obj.var_0A;
		DesignedYear = obj.DesignedYear;
	}

	public override LevelCrossingObject GetAsModel()
		=> new()
		{
			CostFactor = CostFactor,
			SellCostFactor = SellCostFactor,
			CostIndex = CostIndex,
			AnimationSpeed = AnimationSpeed,
			ClosingFrames = ClosingFrames,
			ClosedFrames = ClosedFrames,
			var_0A = var_0A,
			DesignedYear = DesignedYear,
		};
}
