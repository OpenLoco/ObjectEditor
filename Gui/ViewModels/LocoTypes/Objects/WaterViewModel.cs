using Definitions.ObjectModels.Objects.Water;
using ReactiveUI.Fody.Helpers;

namespace Gui.ViewModels;

public class WaterViewModel : LocoObjectViewModel<WaterObject>
{
	[Reactive] public uint8_t CostIndex { get; set; }
	[Reactive] public uint8_t var_03 { get; set; }
	[Reactive] public int16_t CostFactor { get; set; }

	public WaterViewModel(WaterObject obj)
	{
		CostIndex = obj.CostIndex;
		var_03 = obj.var_03;
		CostFactor = obj.CostFactor;
	}

	public override WaterObject GetAsModel()
		=> new()
		{
			CostIndex = CostIndex,
			var_03 = var_03,
			CostFactor = CostFactor,
		};
}
