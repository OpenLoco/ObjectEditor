using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using System.Collections.Generic;

namespace Gui.ViewModels.Loco.Objects.Building;

public class DesignBuildingComponentsViewModel : BuildingComponentsViewModel
{
	public static Image<Rgba32> CreateDummyImage(int width, int height)
	{
		var image = new Image<Rgba32>(width, height);

		// Fill the entire image with a background color (e.g., white)
		image.Mutate(ctx =>
		{
			_ = ctx.BackgroundColor(Color.White);

			// Draw a red rectangle border
			var border = 1;
			var red = Color.Red.ToPixel<Rgba32>();
			for (var y = border; y < height - border; y++)
			{
				for (var x = border; x < width - border; x++)
				{
					image[x, y] = red;
				}
			}
		});

		return image;
	}

	public DesignBuildingComponentsViewModel()
	{
		var width = (short)32;
		var height = (short)32;

		ImageTable = new ImageTable()
		{
			Groups = [
				new (
					"Layer 0",
					[
						new GraphicsElement() { Name = "Layer 0 - South", Width = width, Height = height, Image = CreateDummyImage(width, height) },
						new GraphicsElement() { Name = "Layer 0 - West ", Width = width, Height = height, Image = CreateDummyImage(width, height) },
						new GraphicsElement() { Name = "Layer 0 - North", Width = width, Height = height, Image = CreateDummyImage(width, height) },
						new GraphicsElement() { Name = "Layer 0 - East ", Width = width, Height = height, Image = CreateDummyImage(width, height) },
					]
				),
				new (
					"Layer 1",
					[
						new GraphicsElement() { Name = "Layer 1 - South", Width = width, Height = height, Image = CreateDummyImage(width, height) },
						new GraphicsElement() { Name = "Layer 1 - West ", Width = width, Height = height, Image = CreateDummyImage(width, height) },
						new GraphicsElement() { Name = "Layer 1 - North", Width = width, Height = height, Image = CreateDummyImage(width, height) },
						new GraphicsElement() { Name = "Layer 1 - East ", Width = width, Height = height, Image = CreateDummyImage(width, height) },
					]
				)
			]
		};

		BuildingHeights = [16, 16];
		BuildingAnimations =
		[
			new BuildingPartAnimation() { AnimationSpeed = 40, NumFrames = 20 },
			new BuildingPartAnimation() { AnimationSpeed = 30, NumFrames = 15 },
		];
		List<List<uint8_t>> buildingVariations =
		[
			[0, 1],
			[0, 1, 1],
		];
		List<uint8_t> buildingHeights = [16, 16];

		RecomputeBuildingVariationViewModels(buildingVariations, buildingHeights);
	}
}
