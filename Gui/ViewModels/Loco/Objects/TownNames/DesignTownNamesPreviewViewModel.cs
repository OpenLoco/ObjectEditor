using Definitions.ObjectModels.Objects.TownNames;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class DesignTownNamesPreviewViewModel : TownNamesPreviewViewModel
{
	public DesignTownNamesPreviewViewModel()
		: base(new TownNamesViewModel(new TownNamesObject()))
	{
		GeneratedNames =
		[
			new() { Name = "Bridgeton", Flags = LocationFlags.None },
			new() { Name = "Riverdale", Flags = LocationFlags.None },
			new() { Name = "Oakhaven", Flags = LocationFlags.None },
			new() { Name = "Millbrook", Flags = LocationFlags.None },
			new() { Name = "Fairview", Flags = LocationFlags.None },
			new() { Name = "Westwood", Flags = LocationFlags.None },
			new() { Name = "Northgate", Flags = LocationFlags.None },
			new() { Name = "Southmere", Flags = LocationFlags.None },
			new() { Name = "Eastleigh", Flags = LocationFlags.None },
			new() { Name = "Greenfield", Flags = LocationFlags.None },
			new() { Name = "Portside", Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { Name = "Harborview", Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { Name = "Baymouth", Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { Name = "Seabrink", Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { Name = "Coastal", Flags = LocationFlags.AdjacentToLargeWaterBody },
			new() { Name = "Hillcrest", Flags = LocationFlags.NotMountainous },
			new() { Name = "Ridgefield", Flags = LocationFlags.NotMountainous },
			new() { Name = "Highland", Flags = LocationFlags.NotMountainous },
			new() { Name = "Mountainview", Flags = LocationFlags.NotMountainous },
			new() { Name = "Summit", Flags = LocationFlags.NotMountainous },
			new() { Name = "Brookside", Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { Name = "Creekview", Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { Name = "Lakewood", Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { Name = "Pondfield", Flags = LocationFlags.AdjacentToSmallWaterBody },
			new() { Name = "Streamvale", Flags = LocationFlags.AdjacentToSmallWaterBody },
		];
	}
}
