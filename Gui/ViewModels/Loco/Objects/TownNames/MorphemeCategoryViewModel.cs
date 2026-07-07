using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.TownNames;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class MorphemeCategoryViewModel
{
	[Description("Controls chance to skip / bias selection. When bias is 0, all morphemes are equally likely to be selected. Higher values increase the likelihood of selecting morphemes with lower indices.")]
	public uint8_t Bias { get; set; }

	public ObservableCollection<StringTableEntryViewModel> Morphemes { get; set; } = [];
}
