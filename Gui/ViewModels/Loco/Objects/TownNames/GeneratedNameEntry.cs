using Definitions.ObjectModels.Objects.TownNames;
using System.Collections.Generic;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class GeneratedNameEntry
{
	public List<string> MorphemeComponents { get; set; } = [];
	public LocationFlags Flags { get; set; } = LocationFlags.None;

	public string Name
		=> string.Join(string.Empty, MorphemeComponents);
}
