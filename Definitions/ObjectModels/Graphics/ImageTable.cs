namespace Definitions.ObjectModels.Graphics;

public record ImageTableGroup(string Name, List<GraphicsImage> GraphicsElements);

public class ImageTable : IHasGraphicsElements
{
	public void InsertAt(int index, bool insertBefore)
		=> InsertAt(ImageTableHelpers.GetErrorGraphicsImage(index), insertBefore);

	public void InsertAt(GraphicsImage newImage, bool insertBefore)
	{
		var index = newImage.ImageTableIndex;

		// find the group this image should go into
		var group = Groups.SingleOrDefault(x => x.GraphicsElements.Any(y => y.ImageTableIndex == index));

		ArgumentNullException.ThrowIfNull(group, nameof(group));

		// find the position to insert into the group
		var insertPos = group.GraphicsElements.FindIndex(x => x.ImageTableIndex == index);
		insertPos += insertBefore ? 0 : 1;

		// update the ImageTableIndex of all images at or after this index
		foreach (var g in Groups)
		{
			foreach (var image in g.GraphicsElements)
			{
				if (image.ImageTableIndex >= index)
				{
					++image.ImageTableIndex;
				}
			}
		}

		// actually insert it
		if (insertPos >= 0 && insertPos < group.GraphicsElements.Count)
		{
			group.GraphicsElements.Insert(insertPos, newImage);
		}
		else
		{
			group.GraphicsElements.Add(newImage);
		}
	}

	public void DeleteAt(int index)
	{
		foreach (var g in Groups)
		{
			var toRemove = g.GraphicsElements.FirstOrDefault(x => x.ImageTableIndex == index);
			if (toRemove != null)
			{
				_ = g.GraphicsElements.Remove(toRemove);
				break;
			}
		}

		// reindex all images after this one
		foreach (var g in Groups)
		{
			foreach (var image in g.GraphicsElements)
			{
				if (image.ImageTableIndex > index)
				{
					--image.ImageTableIndex;
				}
			}
		}
	}

	// public/old interface
	public List<GraphicsImage> GraphicsElements
		=> [.. Groups
			.SelectMany(x => x.GraphicsElements)
			.OrderBy(x => x.ImageTableIndex)];

	public List<ImageTableGroup> Groups { get; set; } = [];
}
