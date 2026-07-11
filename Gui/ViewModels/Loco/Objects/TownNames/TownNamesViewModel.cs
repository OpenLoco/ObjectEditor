using Common.Json;
using Definitions.ObjectModels.Objects.TownNames;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

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

	[Category("Import / Export")]
	public ReactiveCommand<Unit, Unit> ImportJsonCommand { get; }

	[Category("Import / Export")]
	public ReactiveCommand<Unit, Unit> ExportJsonCommand { get; }

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

		ImportJsonCommand = ReactiveCommand.CreateFromTask(ImportFromJsonAsync);
		ExportJsonCommand = ReactiveCommand.CreateFromTask(ExportToJsonAsync);
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

	async Task ImportFromJsonAsync()
	{
		try
		{
			var files = await PlatformSpecific.OpenFilePicker(PlatformSpecific.JsonFileTypes);
			var file = files?.FirstOrDefault();
			if (file?.Path == null)
			{
				return;
			}

			var json = await JsonFile.DeserializeFromFileAsync<TownNamesMorphemesJson>(file.Path.LocalPath);
			if (json == null)
			{
				return;
			}

			var categories = new[] { json.Category1, json.Category2, json.Category3, json.Category4, json.Category5, json.Category6 };
			for (var i = 0; i < 6; i++)
			{
				MorphemeCategories[i].Bias = categories[i].Bias;
				MorphemeCategories[i].Morphemes.Clear();
				foreach (var entry in categories[i].TownNames)
				{
					MorphemeCategories[i].Morphemes.Add(new StringTableEntryViewModel(entry.Text, entry.LocationHint));
				}
			}
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to import town names JSON: {ex.Message}");
		}
	}

	async Task ExportToJsonAsync()
	{
		try
		{
			var saveFile = await PlatformSpecific.SaveFilePicker(PlatformSpecific.JsonFileTypes);
			if (saveFile?.Path == null)
			{
				return;
			}

			var json = new TownNamesMorphemesJson(
				ToMorphemeCategory(MorphemeCategory1),
				ToMorphemeCategory(MorphemeCategory2),
				ToMorphemeCategory(MorphemeCategory3),
				ToMorphemeCategory(MorphemeCategory4),
				ToMorphemeCategory(MorphemeCategory5),
				ToMorphemeCategory(MorphemeCategory6));

			await JsonFile.SerializeToFileAsync(json, saveFile.Path.LocalPath);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to export town names JSON: {ex.Message}");
		}
	}

	static MorphemeCategory ToMorphemeCategory(MorphemeCategoryViewModel vm)
	{
		return new MorphemeCategory
		{
			Bias = vm.Bias,
			TownNames = [.. vm.Morphemes.Select(x => new StringTableEntry(x.Text, x.LocationHint))]
		};
	}
}
