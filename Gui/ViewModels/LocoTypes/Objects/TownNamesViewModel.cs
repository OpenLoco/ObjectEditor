using Definitions.ObjectModels.Objects.TownNames;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class TownNamesViewModel : LocoObjectViewModel<TownNamesObject>
{
	[Length(6, 6), Editable(false)]
	public List<Category> Categories { get; set; }

	public TownNamesViewModel(TownNamesObject tno)
		: base(tno)
		=> Categories = [.. tno.Categories];

	public TownNamesObject CopyBackToModel()
		=> new()
		{
			Categories = [.. Categories],
		};
}
