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
	public ObservableCollection<GeneratedNameEntry> GeneratedNames { get; set; } = [];

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
		LargeWaterNames.Clear();
		MountainousNames.Clear();
		SmallWaterNames.Clear();

		const int NumToGenerate = 20;
		GenerateForFlag(LocationFlags.None, NoneFlagNames, NumToGenerate);
		GenerateForFlag(LocationFlags.Mountainous, MountainousNames, NumToGenerate);
		GenerateForFlag(LocationFlags.AdjacentToSmallWaterBody, SmallWaterNames, NumToGenerate);
		GenerateForFlag(LocationFlags.AdjacentToLargeWaterBody, LargeWaterNames, NumToGenerate);
	}

	void GenerateForFlag(LocationFlags targetFlag, ObservableCollection<GeneratedNameEntry> targetCollection, int count)
	{
		while (targetCollection.Count < count)
		{
			var entry = GenerateSingleName();
			if (entry.Flags == targetFlag)
			{
				targetCollection.Add(entry);
			}
		}
	}

	GeneratedNameEntry GenerateSingleName()
	{
		var categories = _townNamesVm.MorphemeCategories;

		var rand = (uint)_random.Next();
		var name = string.Empty;
		var locationFlags = LocationFlags.None;

		List<string> morphemeComponents = [];

		foreach (var category in categories)
		{
			var morphemes = category.Morphemes;
			if (morphemes.Count == 0)
			{
				morphemeComponents.Add(string.Empty);
				continue;
			}

			var ax = (ushort)(rand & 0xFFFF);
			var dx = (ushort)(morphemes.Count + category.Bias);
			var product = (uint)ax * dx;
			var index = (short)((product >> 16) - category.Bias);

			if (index >= 0 && index < morphemes.Count)
			{
				var entry = morphemes[index];
				name += entry.Text;
				morphemeComponents.Add(entry.Text);
				locationFlags |= entry.LocationHint;
			}
			else
			{
				morphemeComponents.Add(string.Empty);
			}

			for (var shifts = morphemes.Count + category.Bias; shifts > 0; shifts >>= 1)
			{
				rand = (rand >> 1) | (rand << 31);
			}
		}

		return new GeneratedNameEntry
		{
			MorphemeComponents = morphemeComponents,
			Flags = locationFlags,
		};
	}
}
