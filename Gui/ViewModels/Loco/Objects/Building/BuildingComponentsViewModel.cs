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
using System.Reactive.Linq;
using ReactiveObject = ReactiveUI.ReactiveObject;

namespace Gui.ViewModels.Loco.Objects.Building;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingComponentsViewModel : ReactiveObject, IViewModel
{
	public string ViewModelDisplayName
		=> "Building Components";

	[Reactive]
	[Trackable(0, 64)]
	public int VerticalLayerSpacing { get; set; }

	[Reactive] public int MaxWidth { get; set; }
	[Reactive] public int MaxHeight { get; set; }

	[Reactive]
	public BuildingComponents BuildingComponentsModel { get; set; }

	[Reactive, Browsable(false)]
	public ObservableCollection<uint8_t> BuildingHeights { get; set; } = [];

	[Reactive, Browsable(false)]
	public ObservableCollection<BuildingPartAnimation> BuildingAnimations { get; set; } = [];

	[Reactive]
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];

	[Reactive]
	public ObservableCollection<BuildingVariationViewModel> BuildingVariationViewModels { get; set; } = [];

	protected ImageTable ImageTable { get; set; }

	public BuildingComponentsViewModel()
	{
		_ = this.WhenAnyValue(x => x.BuildingVariations)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(BuildingVariationViewModels)));

		_ = this.WhenAnyValue(x => x.VerticalLayerSpacing)
			.Subscribe(ApplyOffsetToAllLayers);

		_ = MessageBus.Current.Listen<BuildingComponents>().Subscribe(UpdateBuildingComponents);
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
		_ = this.WhenAnyValue(x => x.BuildingVariationViewModels)
			.Where(x => x != null && ImageTable != null)
			.Subscribe(_ => RecomputeBuildingVariationViewModels(buildingComponents.BuildingVariations, buildingComponents.BuildingHeights));

		BuildingHeights = new ObservableCollection<uint8_t>(buildingComponents.BuildingHeights);
		BuildingAnimations = new ObservableCollection<BuildingPartAnimation>(buildingComponents.BuildingAnimations);
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
							DisplayedImage = layer[i].Image.ToAvaloniaBitmap(),
							XOffset = 0,
							YOffset = 0,
						};

						cumulativeOffset += buildingHeights[variationItem];
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
}

