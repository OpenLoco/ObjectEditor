using Definitions.ObjectModels.Objects.TownNames;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class TownNamesViewModel : LocoObjectViewModel<TownNamesObject>
{
	[Reactive, Length(6, 6), Editable(false)]
	public BindingList<Category> Categories { get; set; }

	public TownNamesViewModel(TownNamesObject tno)
		=> Categories = new(tno.Categories);

	public override TownNamesObject GetAsStruct(TownNamesObject tno)
		=> tno;
}
