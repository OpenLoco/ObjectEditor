using Definitions.ObjectModels.Objects.TownNames;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using ReactiveObject = ReactiveUI.ReactiveObject;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class GeneratedNameEntry
{
	public List<string> MorphemeComponents { get; set; } = [];
	public LocationFlags Flags { get; set; } = LocationFlags.None;

	public string FlagsDisplay
		=> Flags == LocationFlags.None ? "-" : Flags.ToString();

	public string Name
		=> string.Join("", MorphemeComponents);

	public string ComponentsDisplay
		=> string.Join(" | ", MorphemeComponents
			.Select((x, i) => $"{i}:\"{x}\""));
}

public class TownNamesPreviewViewModel : ReactiveObject, IViewModel
{
	readonly TownNamesViewModel _townNamesVm;

	public string DisplayName => "Name Preview";

	[Reactive, Browsable(false)]
	public ObservableCollection<GeneratedNameEntry> NoneFlagNames { get; set; } = [];

	[Reactive, Browsable(false)]
	public ObservableCollection<GeneratedNameEntry> LargeWaterNames { get; set; } = [];

	[Reactive, Browsable(false)]
	public ObservableCollection<GeneratedNameEntry> MountainousNames { get; set; } = [];

	[Reactive, Browsable(false)]
	public ObservableCollection<GeneratedNameEntry> SmallWaterNames { get; set; } = [];

	public ReactiveCommand<Unit, Unit> GenerateNamesCommand { get; }

	readonly Random _random = new();

	public TownNamesPreviewViewModel(TownNamesViewModel townNamesViewModel)
	{
		ArgumentNullException.ThrowIfNull(townNamesViewModel);
		_townNamesVm = townNamesViewModel;

		GenerateNamesCommand = ReactiveCommand.Create(GenerateNames);
	}

	void GenerateNames()
	{
		NoneFlagNames.Clear();
		SmallWaterNames.Clear();
		LargeWaterNames.Clear();
		MountainousNames.Clear();

		const int NumToGenerate = 20;
		GenerateForFlag(LocationFlags.None, NoneFlagNames, NumToGenerate);
		GenerateForFlag(LocationFlags.AdjacentToSmallWaterBody, SmallWaterNames, NumToGenerate);
		GenerateForFlag(LocationFlags.AdjacentToLargeWaterBody, LargeWaterNames, NumToGenerate);
		GenerateForFlag(LocationFlags.Mountainous, MountainousNames, NumToGenerate);
	}

	void GenerateForFlag(LocationFlags targetFlag, ObservableCollection<GeneratedNameEntry> targetCollection, int count)
	{
		for (var i = 0; i < count; i++)
		{
			targetCollection.Add(GenerateSingleName(targetFlag));
		}
	}

	GeneratedNameEntry GenerateSingleName(LocationFlags targetFlag)
	{
		List<string> morphemeComponents = [];

		foreach (var category in _townNamesVm.MorphemeCategories)
		{
			// if the morphemes contains location-hinted entries, use those, else use generics
			var relevantMorphemes = category.Morphemes.Where(m => m.LocationHint == targetFlag).ToList();
			if (relevantMorphemes.Count == 0)
			{
				relevantMorphemes = category.Morphemes.Where(m => m.LocationHint == LocationFlags.None).ToList();
			}

			if (relevantMorphemes.Count == 0)
			{
				morphemeComponents.Add(string.Empty);
				continue;
			}

			var index = _random.Next(relevantMorphemes.Count + category.Bias);

			if (index >= 0 && index < relevantMorphemes.Count)
			{
				var entry = relevantMorphemes[index];
				morphemeComponents.Add(entry.Text);
			}
			else
			{
				// entry skipped due to bias, add empty string to maintain component count
				morphemeComponents.Add(string.Empty);
			}
		}

		return new GeneratedNameEntry
		{
			MorphemeComponents = morphemeComponents,
			Flags = targetFlag,
		};
	}
}
