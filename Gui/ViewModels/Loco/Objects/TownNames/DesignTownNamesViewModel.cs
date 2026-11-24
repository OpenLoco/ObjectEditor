using Definitions.ObjectModels.Objects.TownNames;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class DesignTownNamesViewModel : TownNamesViewModel
{
	public DesignTownNamesViewModel()
		: base(new TownNamesObject()
		{
			MorphemeCategories =
			[
				new() { Bias = 5, TownNames = [new StringTableEntry("Bridge", LocationFlags.None), new StringTableEntry("River", LocationFlags.None)] },
				new() { Bias = 3, TownNames = [new StringTableEntry("ton", LocationFlags.None), new StringTableEntry("ville", LocationFlags.None)] },
				new() { Bias = 0, TownNames = [new StringTableEntry("Oak", LocationFlags.None), new StringTableEntry("Elm", LocationFlags.None)] },
				new() { Bias = 2, TownNames = [new StringTableEntry("Mill", LocationFlags.AdjacentToSmallWaterBody), new StringTableEntry("Brook", LocationFlags.AdjacentToLargeWaterBody)] },
				new() { Bias = 1, TownNames = [new StringTableEntry("Fair", LocationFlags.Mountainous), new StringTableEntry("High", LocationFlags.Mountainous)] },
				new() { Bias = 4, TownNames = [new StringTableEntry("West", LocationFlags.None), new StringTableEntry("North", LocationFlags.None)] },
			]
		})
	{
	}
}