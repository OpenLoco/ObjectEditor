using Definitions.ObjectModels.Objects.TownNames;
using Gui.ViewModels.Loco.Objects.TownNames;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class TownNamesViewModelTests
{
	[Test]
	public void SynchronizeToModel_MapsNestedViewModelCollectionsBackToModel()
	{
		var model = new TownNamesObject();
		model.MorphemeCategories.Add(new MorphemeCategory
		{
			Bias = 1,
			TownNames = [new StringTableEntry("old", LocationFlags.None)]
		});

		var viewModel = new TownNamesViewModel(model);
		viewModel.MorphemeCategory1.Bias = 9;
		viewModel.MorphemeCategory1.Morphemes.Clear();
		viewModel.MorphemeCategory1.Morphemes.Add(new StringTableEntryViewModel("new", LocationFlags.AdjacentToLargeWaterBody));

		viewModel.SynchronizeToModel();

		Assert.Multiple(() =>
		{
			Assert.That(model.MorphemeCategories[0].Bias, Is.EqualTo((byte)9));
			Assert.That(model.MorphemeCategories[0].TownNames, Has.Count.EqualTo(1));
			Assert.That(model.MorphemeCategories[0].TownNames[0].Text, Is.EqualTo("new"));
			Assert.That(model.MorphemeCategories[0].TownNames[0].LocationHint, Is.EqualTo(LocationFlags.AdjacentToLargeWaterBody));
		});
	}
}
