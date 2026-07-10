using Definitions.ObjectModels.Objects.TownNames;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class TownNamesViewModel : BaseViewModel<TownNamesObject>
{
	[Length(6, 6), Editable(false)]
	ObservableCollection<MorphemeCategoryViewModel> MorphemeCategories { get; set; } = [];

	[Category("Cat1")]
	public MorphemeCategoryViewModel MorphemeCategory1 { get => MorphemeCategories[0]; set => MorphemeCategories[0] = value; }
	[Category("Cat2")]
	public MorphemeCategoryViewModel MorphemeCategory2 { get => MorphemeCategories[1]; set => MorphemeCategories[1] = value; }
	[Category("Cat3")]
	public MorphemeCategoryViewModel MorphemeCategory3 { get => MorphemeCategories[2]; set => MorphemeCategories[2] = value; }
	[Category("Cat4")]
	public MorphemeCategoryViewModel MorphemeCategory4 { get => MorphemeCategories[3]; set => MorphemeCategories[3] = value; }
	[Category("Cat5")]
	public MorphemeCategoryViewModel MorphemeCategory5 { get => MorphemeCategories[4]; set => MorphemeCategories[4] = value; }
	[Category("Cat6")]
	public MorphemeCategoryViewModel MorphemeCategory6 { get => MorphemeCategories[5]; set => MorphemeCategories[5] = value; }

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

	public override void SynchronizeToModel()
	{
		Model.MorphemeCategories.Clear();

		foreach (var categoryViewModel in MorphemeCategories)
		{
			Model.MorphemeCategories.Add(new MorphemeCategory
			{
				Bias = categoryViewModel.Bias,
				TownNames = [.. categoryViewModel.Morphemes.Select(x => new StringTableEntry(x.Text, x.LocationHint))]
			});
		}
	}
}
