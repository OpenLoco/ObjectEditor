using Definitions.ObjectModels.Objects.Water;

namespace Gui.ViewModels;

public class WaterViewModel(WaterObject model)
	: LocoObjectViewModel<WaterObject>(model)
{
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	public uint8_t var_03
	{
		get => Model.var_03;
		set => Model.var_03 = value;
	}

	public int16_t CostFactor
	{
		get => Model.CostFactor;
		set => Model.CostFactor = value;
	}
}
