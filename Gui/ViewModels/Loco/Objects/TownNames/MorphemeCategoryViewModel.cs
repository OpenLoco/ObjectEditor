using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Gui.ViewModels.Loco.Objects.TownNames;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class MorphemeCategoryViewModel(byte bias, ObservableCollection<StringTableEntryViewModel> morphemes)
{
	[Description("Controls chance to skip / bias selection. When bias is 0, change to select from N morphemes is 1/N. With bias B, change is 1/(N+B). Chance of skipping entirely is added and is also 1/(N+B). In other words, adding a bias adds B chances to skip entirely.")]
	public uint8_t Bias { get; set; } = bias;

	public ObservableCollection<StringTableEntryViewModel> Morphemes { get; set; } = morphemes;

	public MorphemeCategoryViewModel()
		: this(0, []) { }
}
