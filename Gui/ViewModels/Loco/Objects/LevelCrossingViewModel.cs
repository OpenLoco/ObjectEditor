using Definitions.ObjectModels.Objects.LevelCrossing;
using System.ComponentModel;

namespace Gui.ViewModels;

public class LevelCrossingViewModel(LevelCrossingObject obj)
	: BaseViewModel<LevelCrossingObject>(obj)
{

	[Category("Cost")]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost")]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	[Category("Animation")]
	public uint8_t AnimationSpeed
	{
		get => Model.AnimationSpeed;
		set => Model.AnimationSpeed = value;
	}

	[Category("Animation")]
	public uint8_t IdleOpenFrames
	{
		get => Model.IdleClosedFrames;
		set => Model.IdleClosedFrames = value;
	}

	[Category("Animation")]
	public uint8_t AnimationFrames
	{
		get => Model.ClosingSequenceFrames;
		set => Model.ClosingSequenceFrames = value;
	}

	[Category("Animation")]
	public uint8_t IdleClosedFrames
	{
		get => Model.AnimationSpeedBitmask;
		set => Model.AnimationSpeedBitmask = value;
	}
}
