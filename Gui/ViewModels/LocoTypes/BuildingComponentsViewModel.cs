using Avalonia.Media.Imaging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using DynamicData.Binding;
using Gui.Models;
using Gui.ViewModels.Graphics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace Gui.ViewModels.LocoTypes;

public class BuildingLayerViewModel : ReactiveObject
{
	[Reactive] public Bitmap DisplayedImage { get; set; }

	public double Width => DisplayedImage.Size.Width;
	public double Height => DisplayedImage.Size.Height;

	public double X => XBase + XOffset;
	public double Y => YBase + YOffset;

	[Reactive] public double XBase { get; set; }
	[Reactive] public double YBase { get; set; }

	[Reactive] public double XOffset { get; set; }
	[Reactive] public double YOffset { get; set; }

	public BuildingLayerViewModel()
	{
		_ = this.WhenAnyValue(o => o.XBase)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(X)));
		_ = this.WhenAnyValue(o => o.XOffset)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(X)));

		_ = this.WhenAnyValue(o => o.YBase)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Y)));
		_ = this.WhenAnyValue(o => o.YOffset)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(Y)));
	}
}

public class BuildingStackViewModel : ReactiveObject
{
	public CardinalDirection Direction { get; init; }
	public ObservableCollection<BuildingLayerViewModel> Layers { get; set; } = [];
}

public class BuildingVariationViewModel : ReactiveObject
{
	public ObservableCollection<BuildingStackViewModel> Directions { get; set; } = [];
}

public class BuildingComponentsViewModel : ReactiveObject
{
	public BuildingComponentsModel BuildingComponents { get; }

	public ObservableCollection<BuildingVariationViewModel> BuildingVariations { get; set; } = [];

	[Reactive] public int OffsetSpacing { get; set; }

	[Reactive] public int MaxWidth { get; set; }
	[Reactive] public int MaxHeight { get; set; }

	public BuildingComponentsViewModel(BuildingComponentsModel buildingComponents, ImageTable imageTable, PaletteMap paletteMap)
	{
		BuildingComponents = buildingComponents;

		var layers = imageTable.Groups.ConvertAll(x => x.GraphicsElements); // each building part has 4 directions

		MaxWidth = layers.Max(x => x.Max(y => y.Width)) + 16;
		MaxHeight = (layers.Max(x => x.Max(y => y.Height)) * buildingComponents.BuildingHeights.Count) + buildingComponents.BuildingHeights.Sum(x => x) + (buildingComponents.BuildingVariations.Max(x => x.Count) * OffsetSpacing);

		foreach (var variation in buildingComponents.BuildingVariations)
		{
			var bv = new BuildingVariationViewModel();

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
					var layer = layers[variationItem];
					var bl = new BuildingLayerViewModel();

					if (!paletteMap.TryConvertG1ToRgba32Bitmap(layer[i], ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
					{
						throw new Exception("Failed to convert image");
					}

					bl.DisplayedImage = image.ToAvaloniaBitmap();
					bl.XBase = layer[i].XOffset + MaxWidth / 2;
					bl.YBase = layer[i].YOffset - cumulativeOffset + (MaxHeight * 0.80);
					cumulativeOffset += buildingComponents.BuildingHeights[variationItem];

					bl.XOffset = 0;
					bl.YOffset = 0;

					bs.Layers.Add(bl);
				}

				bv.Directions.Add(bs); // [i] = bs;
			}

			BuildingVariations.Add(bv);
		}

		// Update all nested layers' YOffset whenever OffsetSpacing changes
		_ = this.WhenAnyValue(x => x.OffsetSpacing)
			.Subscribe(ApplyOffsetToAllLayers);
	}

	private void ApplyOffsetToAllLayers(int offset)
	{
		foreach (var variation in BuildingVariations)
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
					cumulativeOffset -= OffsetSpacing;
				}
			}
		}
	}
}

