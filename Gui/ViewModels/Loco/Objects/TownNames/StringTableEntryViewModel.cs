using Definitions.ObjectModels.Objects.TownNames;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.TownNames;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class StringTableEntryViewModel(string text, LocationFlags type)
{
	public string Text { get; set; } = text;

	[Description("Where this morpheme should be used")]
	[EnumProhibitValues<LocationFlags>(LocationFlags.None)]
	public LocationFlags LocationHint { get; set; } = type;

	public StringTableEntryViewModel()
		: this(string.Empty, LocationFlags.None) { }
}
