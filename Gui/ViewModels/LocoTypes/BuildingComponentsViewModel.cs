using Avalonia.Media.Imaging;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
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

public enum CardinalDirection : uint8_t
{
	South,
	West,
	North,
	East,
}

public class BuildingLayerViewModel : ReactiveObject
{
	[Reactive] public Bitmap DisplayedImage { get; set; }

	public double Width => DisplayedImage.Size.Width;
	public double Height => DisplayedImage.Size.Height;

	[Reactive] public double X { get; set; }
	[Reactive] public double Y { get; set; }
	[Reactive] public double XOffset { get; set; }
	[Reactive] public double YOffset { get; set; }
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

	List<GraphicsElement> GraphicsElements { get; }

	public ObservableCollection<ImageViewModel> LayeredImages { get; set; } = [];

	public ObservableCollection<BuildingVariationViewModel> BuildingVariations { get; set; } = [];

	public int OffsetSpacing { get; set; }

	public BuildingComponentsViewModel(BuildingComponentsModel buildingComponents, List<GraphicsElement> graphicsElements, PaletteMap paletteMap)
	{
		BuildingComponents = buildingComponents;
		GraphicsElements = graphicsElements;

		var layers = graphicsElements.Chunk(4).ToList(); // each building part has 4 directions

		var baseX = 128;
		var baseY = 192;

		foreach (var variation in buildingComponents.BuildingVariations)
		{
			var bv = new BuildingVariationViewModel();

			var numDirections = 4;
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
					bl.X = layer[i].XOffset + baseX;
					bl.Y = layer[i].YOffset - cumulativeOffset + baseY;
					cumulativeOffset += buildingComponents.BuildingHeights[variationItem];

					bl.XOffset = 0;
					bl.YOffset = 0;

					bs.Layers.Add(bl);
				}

				bv.Directions.Add(bs); // [i] = bs;

				//yOffset -= buildingComponents.BuildingHeights[variationItem];
			}

			BuildingVariations.Add(bv);
		}

		//_ = this.WhenAnyValue(x => x.OffsetSpacing)
		//		.Where(_ => LayeredImages.Count > 0)
		//		.Subscribe(spacing =>
		//		{
		//			var index = 0;
		//			foreach (var img in LayeredImages)
		//			{
		//				var diff = OffsetSpacing - prevOffset;
		//				img.YOffset -= diff * index;
		//				img.RaisePropertyChanged(nameof(img.YOffset));
		//				img.RaisePropertyChanged(nameof(img));
		//				index++;
		//			}

		//			prevOffset = OffsetSpacing;
		//			this.RaisePropertyChanged(nameof(LayeredImages));
		//		});
	}
}

