using Definitions.ObjectModels.Objects.TownNames;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ReactiveObject = ReactiveUI.ReactiveObject;

namespace Gui.ViewModels.Loco.Objects.TownNames;

public class GeneratedNameEntry
{
	public string Name { get; set; } = string.Empty;
	public LocationFlags Flags { get; set; } = LocationFlags.None;
	public string FlagsDisplay => Flags == LocationFlags.None ? "—" : Flags.ToString();
}

public class TownNamesPreviewViewModel : ReactiveObject, IViewModel, IDisposable
{
	readonly TownNamesViewModel _townNamesVm;
	readonly CompositeDisposable _subscriptions = [];
	bool _disposed;

	public string DisplayName => "Name Preview";

	[Reactive, Browsable(false)]
	public ObservableCollection<GeneratedNameEntry> GeneratedNames { get; set; } = [];

	public ReactiveCommand<Unit, Unit> GenerateNamesCommand { get; }

	readonly Random _random = new();

	public TownNamesPreviewViewModel(TownNamesViewModel townNamesViewModel)
	{
		ArgumentNullException.ThrowIfNull(townNamesViewModel);
		_townNamesVm = townNamesViewModel;

		GenerateNamesCommand = ReactiveCommand.Create(GenerateNames);

		// Reactively regenerate when any morpheme data changes in the TownNamesViewModel
		this.WhenAnyValue(x => x._townNamesVm.MorphemeCategories)
			.Where(x => x != null)
			.Subscribe(_ => GenerateNames())
			.DisposeWith(_subscriptions);
	}

	void GenerateNames()
	{
		GeneratedNames.Clear();

		GenerateForFlag(LocationFlags.None, 10);
		GenerateForFlag(LocationFlags.AdjacentToLargeWaterBody, 5);
		GenerateForFlag(LocationFlags.NotMountainous, 5);
		GenerateForFlag(LocationFlags.AdjacentToSmallWaterBody, 5);
	}

	void GenerateForFlag(LocationFlags targetFlag, int count)
	{
		var attempts = 0;
		const int maxAttempts = 1000;

		while (GeneratedNames.Count(x => x.Flags == targetFlag) < count && attempts < maxAttempts)
		{
			attempts++;
			var entry = GenerateSingleName();
			if (entry.Flags == targetFlag)
			{
				GeneratedNames.Add(entry);
			}
		}
	}

	GeneratedNameEntry GenerateSingleName()
	{
		// Read the current live data from the TownNamesViewModel (reflects user edits)
		var categories = _townNamesVm.MorphemeCategories;

		var rand = (uint)_random.Next();

		var name = "";
		LocationFlags locationFlags = LocationFlags.None;

		foreach (var category in categories)
		{
			var morphemes = category.Morphemes;
			if (morphemes.Count == 0)
			{
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
				locationFlags |= entry.LocationHint;
			}

			for (var shifts = morphemes.Count + category.Bias; shifts > 0; shifts >>= 1)
			{
				rand = (rand >> 1) | (rand << 31);
			}
		}

		return new GeneratedNameEntry
		{
			Name = name,
			Flags = locationFlags,
		};
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		_subscriptions.Dispose();
		GC.SuppressFinalize(this);
	}
}
