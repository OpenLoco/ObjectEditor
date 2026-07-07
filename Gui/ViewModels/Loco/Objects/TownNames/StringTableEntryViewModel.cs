using Definitions.ObjectModels.Objects.TownNames;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.TownNames;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class StringTableEntryViewModel
{
	public string Text { get; set; }

	[Description("Where this morpheme should be used")]
	[EnumProhibitValues<LocationFlags>(LocationFlags.None)]
	public LocationFlags LocationHint { get; set; }

	public StringTableEntryViewModel(string text, LocationFlags type)
	{
		Text = text;
		LocationHint = type;
	}
}
