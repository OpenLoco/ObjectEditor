using Definitions.ObjectModels.Objects.Water;
using Gui.Attributes;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels;

public class WaterViewModel(WaterObject model)
	: LocoObjectViewModel<WaterObject>(model)
{
	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set
		{
			Model.CostIndex = value;
			this.RaisePropertyChanged(nameof(CostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(nameof(CostIndex))]
	public int16_t CostFactor
	{
		get => Model.CostFactor;
		set => Model.CostFactor = value;
	}
	public uint8_t var_03
	{
		get => Model.var_03;
		set => Model.var_03 = value;
	}

}
