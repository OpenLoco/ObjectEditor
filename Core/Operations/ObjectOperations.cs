using Core.Graphics;
using Core.Objects;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;

namespace Core.Operations;

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
			foreach (var element in group.GraphicsElements)
			{
				element.Image?.Dispose();
				element.Image = null;
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

		var elements = locoObject.ImageTable?.GraphicsElements;
		if (elements == null)
		{
			return 0;
		}

		foreach (var element in elements)
		{
			action(element);
		}

		return elements.Count;
	}
}
