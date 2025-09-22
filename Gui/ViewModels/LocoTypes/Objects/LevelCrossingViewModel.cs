using Definitions.ObjectModels.Objects.LevelCrossing;

namespace Gui.ViewModels;

public class LevelCrossingViewModel(LevelCrossingObject obj)
	: LocoObjectViewModel<LevelCrossingObject>(obj)
{

	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	public uint8_t AnimationSpeed
	{
		get => Model.AnimationSpeed;
		set => Model.AnimationSpeed = value;
	}

	public uint8_t ClosingFrames
	{
		get => Model.ClosingFrames;
		set => Model.ClosingFrames = value;
	}

	public uint8_t ClosedFrames
	{
		get => Model.ClosedFrames;
		set => Model.ClosedFrames = value;
	}

	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	public uint8_t var_0A
	{
		get => Model.var_0A;
		set => Model.var_0A = value;
	}
}
