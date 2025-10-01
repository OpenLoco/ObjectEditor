using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public class ImageTable : IHasGraphicsElements
{
	public PaletteMap PaletteMap
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			field = value;

			foreach (var g in Groups)
			{
				foreach (var ge in g.GraphicsElements)
				{
					if (!field.TryConvertG1ToRgba32Bitmap(ge, ColourSwatch.PrimaryRemap, ColourSwatch.SecondaryRemap, out var image))
					{
						throw new Exception("Failed to convert image");
					}
					ge.Image = image;
				}
			}
		}
	}

	// public/old interface
	public List<GraphicsElement> GraphicsElements
	{
		get => [.. Groups
			.SelectMany(x => x.GraphicsElements)
			.OrderBy(x => x.ImageTableIndex)];
		set
		{
			Groups.Clear();
			Groups.Add(("All", value));
		}
	}

	public List<(string Name, List<GraphicsElement> GraphicsElements)> Groups { get; set; } = [];
}
