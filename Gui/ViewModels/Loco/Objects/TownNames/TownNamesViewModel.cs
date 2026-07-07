using Definitions.ObjectModels.Objects.TownNames;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class TownNamesViewModel : BaseViewModel<TownNamesObject>
{
	[Length(6, 6)]
	public ObservableCollection<MorphemeCategoryViewModel> MorphemeCategories { get; set; } = [];

	public TownNamesViewModel(TownNamesObject model)
		: base(model)
	{
		foreach (var morphemeCategory in model.MorphemeCategories)
		{
			MorphemeCategories.Add(new MorphemeCategoryViewModel
			{
				Bias = morphemeCategory.Bias,
				Morphemes = [with(morphemeCategory.TownNames.Select(x => new StringTableEntryViewModel(x.Text, x.LocationHint)))]
			});
		}
	}
}
