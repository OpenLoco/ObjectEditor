using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Common;
using PropertyModels.ComponentModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using ReactiveObject = ReactiveUI.ReactiveObject;

namespace Gui.ViewModels.Loco.Objects.Building;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingComponentsViewModel : ReactiveObject, IViewModel, IDisposable
{
	readonly CompositeDisposable _subscriptions = [];
	IDisposable? _modelSubscription;
	bool _disposed;
	public string DisplayName
		=> "Building Components";

	[Reactive]
	[Trackable(0, 64)]
	public int VerticalLayerSpacing { get; set; }

	[Reactive] public int MaxWidth { get; set; }
	[Reactive] public int MaxHeight { get; set; }

	[Reactive]
	public BuildingComponents BuildingComponentsModel { get; set; } = new();

	[Reactive, Browsable(false)]
	public ObservableCollection<uint8_t> BuildingHeights { get; set; } = [];

	[Reactive, Browsable(false)]
	public ObservableCollection<BuildingPartAnimation> BuildingAnimations { get; set; } = [];

	[Reactive]
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];

	[Reactive]
	public ObservableCollection<BuildingVariationViewModel> BuildingVariationViewModels { get; set; } = [];

	protected ImageTable ImageTable { get; set; } = new();

	public BuildingComponentsViewModel()
	{
		this.WhenAnyValue(x => x.BuildingVariations)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(BuildingVariationViewModels)))
			.DisposeWith(_subscriptions);

		this.WhenAnyValue(x => x.VerticalLayerSpacing)
			.Subscribe(ApplyOffsetToAllLayers)
			.DisposeWith(_subscriptions);

		MessageBus.Current.Listen<BuildingComponents>()
			.Subscribe(UpdateBuildingComponents)
			.DisposeWith(_subscriptions);
	}

	public BuildingComponentsViewModel(BuildingComponents buildingComponents, ImageTable imageTable) : this()
	{
		ArgumentNullException.ThrowIfNull(buildingComponents);
		ArgumentNullException.ThrowIfNull(imageTable);

		ImageTable = imageTable;
		UpdateBuildingComponents(buildingComponents);
	}

	void UpdateBuildingComponents(BuildingComponents buildingComponents)
	{
		// Replace any previous per-model subscription so we don't accumulate handlers
		// every time the model is swapped.
		_modelSubscription?.Dispose();
		_modelSubscription = this.WhenAnyValue(x => x.BuildingVariationViewModels)
			.Where(x => x != null && ImageTable != null)
			.Subscribe(_ => RecomputeBuildingVariationViewModels(buildingComponents.BuildingVariations, buildingComponents.BuildingHeights));

		BuildingHeights = [with(buildingComponents.BuildingHeights)];
		BuildingAnimations = [with(buildingComponents.BuildingAnimations)];
		BuildingVariations = buildingComponents.BuildingVariations;
		BuildingComponentsModel = buildingComponents;

		RecomputeBuildingVariationViewModels(buildingComponents.BuildingVariations, buildingComponents.BuildingHeights);
	}

	protected void RecomputeBuildingVariationViewModels(List<List<uint8_t>> buildingVariations, List<byte> buildingHeights)
	{
		if (!buildingVariations.Any() || !buildingHeights.Any())
		{
			// todo: log error
			return;
		}

		var layers = ImageTable.Groups.ConvertAll(x => x.GraphicsElements);

		BuildingVariationViewModels.Clear();

		MaxWidth = layers.Max(x => x.Max(y => y.Width)) + 16;
		MaxHeight = (layers.Max(x => x.Max(y => y.Height)) * buildingHeights.Count) + buildingHeights.Sum(x => x) + buildingVariations.Max(x => x.Count) * (VerticalLayerSpacing * 2);

		var x = 0;
		foreach (var variation in buildingVariations)
		{
			var bv = new BuildingVariationViewModel()
			{
				VariationName = $"Variation {x++}",
			};

			const int numDirections = 4;
			for (var i = 0; i < numDirections; ++i)
			{
				var bs = new BuildingStackViewModel()
				{
					Direction = (CardinalDirection)i
				};

				var cumulativeOffset = 0;
				foreach (var variationItem in variation)
				{
					if (layers.Count > variationItem)
					{
						var layer = layers[variationItem];
						var bl = new BuildingLayerViewModel
						{
							XBase = layer[i].XOffset + (MaxWidth / 2),
							YBase = layer[i].YOffset - cumulativeOffset + MaxHeight * 0.80,
							DisplayedImage = layer[i].Image?.ToAvaloniaBitmap(),
							XOffset = 0,
							YOffset = 0,
						};

						// this shouldn't be necessary to check, but some objects are fully invalid, eg USFIFACT, so without this
						// check the editor will crash but we'd instead prefer to show the invalid object to allow fixing it
						if (variationItem < buildingHeights.Count)
						{
							cumulativeOffset += buildingHeights[variationItem];
						}

						bs.Layers.Add(bl);
					}
				}

				bv.Directions.Add(bs); // [i] = bs;
			}

			BuildingVariationViewModels.Add(bv);
		}
	}

	private void ApplyOffsetToAllLayers(int offset)
	{
		foreach (var variation in BuildingVariationViewModels)
		{
			if (variation?.Directions == null)
			{
				continue;
			}

			foreach (var dir in variation.Directions)
			{
				if (dir?.Layers == null)
				{
					continue;
				}

				var cumulativeOffset = 0;
				foreach (var layer in dir.Layers)
				{
					layer.YOffset = cumulativeOffset;
					cumulativeOffset -= VerticalLayerSpacing;
				}
			}
		}
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_disposed = true;
		_modelSubscription?.Dispose();
		_subscriptions.Dispose();
		GC.SuppressFinalize(this);
	}
}

