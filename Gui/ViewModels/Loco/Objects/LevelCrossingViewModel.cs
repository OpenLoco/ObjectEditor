using Definitions.ObjectModels.Objects.LevelCrossing;
using System.ComponentModel;

namespace Gui.ViewModels;

public class LevelCrossingViewModel(LevelCrossingObject obj)
	: BaseViewModel<LevelCrossingObject>(obj)
{
	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

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

	[Category("Animation")]
	[Description("The number of frames used in the animation of opening/closing the crossing.")]
	public uint8_t TransitionAnimationFrameCount
	{
		get => Model.TransitionAnimationFrameCount;
		set => Model.TransitionAnimationFrameCount = value;
	}

	[Category("Animation")]
	[Description("Used a bitmask on `getScenarioTicks()` in OpenLoco. This value is the 'number of ticks to skip before advancing the transition animation'. A value of 0 means the animation advances every tick. 1 means every every 2 ticks. 3 means ever 4 ticks. 7 means every 8 ticks. 15 means every 16 ticks. etc.")]
	public SelectableList<uint8_t> TransitionAnimationDelayBitmask
	{
		get => new([0, 1, 3, 7, 15, 31, 63, 127, 255]) { SelectedValue = Model.TransitionAnimationDelayBitmask };
		set => Model.TransitionAnimationDelayBitmask = value.SelectedValue;
	}

	[Category("Animation")]
	[Description("The number of frames used in the animation of a closed/blocked crossing. Must be one of [1, 2, 4, 8, 16, 32]")]
	public SelectableList<uint8_t> ClosedAnimationFrameCount
	{
		get => new([1, 2, 4, 8, 16, 32]) { SelectedValue = Model.ClosedAnimationFrameCount };
		set => Model.ClosedAnimationFrameCount = value.SelectedValue;
	}

	[Category("Animation")]
	[Description("The delay between animation frames for a closed/blocked crossing. Higher values mean slower animations.")]
	public uint8_t ClosedAnimationDelay
	{
		get => Model.ClosedAnimationDelay;
		set => Model.ClosedAnimationDelay = value;
	}
}
