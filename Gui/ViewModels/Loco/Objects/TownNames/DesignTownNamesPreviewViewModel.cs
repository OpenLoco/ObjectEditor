using Definitions.ObjectModels.Objects.TownNames;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class DesignTownNamesPreviewViewModel : TownNamesPreviewViewModel
{
	public DesignTownNamesPreviewViewModel()
		: base(new TownNamesViewModel(
			new TownNamesObject()
			{
				MorphemeCategories =
				[
					new(),
					new(),
					new(),
					new(),
					new(),
					new()
				]
			}))
	{
		NoneFlagNames =
		[
			new() { MorphemeComponents = ["Bridge", "", "ton"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["River"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["Oak"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["Mill"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["Fair"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["West"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["North"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["South"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["East"], Flags = LocationFlags.None },
			new() { MorphemeComponents = ["Green"], Flags = LocationFlags.None },
		];
		SmallWaterNames =
		[
			new() { MorphemeComponents = ["Brook"], Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { MorphemeComponents = ["Creek"], Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { MorphemeComponents = ["Lake"], Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { MorphemeComponents = ["Pond"], Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { MorphemeComponents = ["Stream"], Flags = LocationFlags.AdjacentToSmallWaterBody },
		];
		LargeWaterNames =
		[
			new() { MorphemeComponents = ["Port"], Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { MorphemeComponents = ["Harbor"], Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { MorphemeComponents = ["Bay"], Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { MorphemeComponents = ["Seabrink"], Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { MorphemeComponents = ["Coastal"], Flags = LocationFlags.AdjacentToLargeWaterBody },
		];
		MountainousNames =
		[
			new() { MorphemeComponents = ["Hill"], Flags = LocationFlags.Mountainous },
			new() { MorphemeComponents = ["Ridge"], Flags = LocationFlags.Mountainous },
			new() { MorphemeComponents = ["High"], Flags = LocationFlags.Mountainous },
			new() { MorphemeComponents = ["Mountain"], Flags = LocationFlags.Mountainous },
			new() { MorphemeComponents = ["Shillhaven"], Flags = LocationFlags.Mountainous },
		];
	}
}
