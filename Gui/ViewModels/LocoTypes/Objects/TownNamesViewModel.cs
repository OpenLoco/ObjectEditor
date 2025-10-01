using Definitions.ObjectModels.Objects.TownNames;
using PropertyModels.ComponentModel;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class TownNamesViewModel(TownNamesObject model)
	: LocoObjectViewModel<TownNamesObject>(model)
{
	[Length(6, 6)]
	[Editable(false)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<Category> Categories { get; set; } = model.Categories.ToBindingList();

}
