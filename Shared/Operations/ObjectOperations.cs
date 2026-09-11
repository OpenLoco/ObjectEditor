using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;

namespace Shared.Operations;

public static class ObjectOperations
{
	public static int StripImages(LocoObject locoObject)
	{
		ArgumentNullException.ThrowIfNull(locoObject);

		var imageTable = locoObject.ImageTable;
		if (imageTable == null)
		{
			return 0;
		}

		var removed = imageTable.Groups.Sum(x => x.GraphicsElements.Count);

		foreach (var group in imageTable.Groups)
		{
			foreach (var image in group.GraphicsElements)
			{
				if (image != null
					&& !ReferenceEquals(image, ImageTableHelpers.ErrorImage)
					&& !ReferenceEquals(image, ImageTableHelpers.OnePixelTransparent))
				{
					image.Dispose();
				}
			}
		}

		imageTable.Groups.Clear();

		return removed;
	}

	public static int CropAllImages(LocoObject locoObject, PaletteMap paletteMap)
		=> ForEachImage(locoObject, x => x.Crop(paletteMap));

	public static int ZeroAllOffsets(LocoObject locoObject)
		=> ForEachImage(locoObject, x => x.ZeroOffsets());

	public static int CenterAllOffsets(LocoObject locoObject)
		=> ForEachImage(locoObject, x => x.CenterOffsets());

	public static int TranslateAllOffsets(LocoObject locoObject, short deltaX, short deltaY)
		=> ForEachImage(locoObject, x => x.TranslateOffsets(deltaX, deltaY));

	public static int ForEachImage(LocoObject locoObject, Action<GraphicsElement> action)
	{
		ArgumentNullException.ThrowIfNull(locoObject);
		ArgumentNullException.ThrowIfNull(action);

		var images = locoObject.ImageTable?.GraphicsElements;
		if (images == null)
		{
			return 0;
		}

		foreach (var image in images)
		{
			action(image);
		}

		return images.Count;
	}
}
