using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics;

public record ImageTableGroup(string Name, List<GraphicsElement> GraphicsElements);

public class ImageTable : IHasGraphicsElements
{
	[JsonIgnore]
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

	public void InsertAt(int index, bool insertBefore)
		=> InsertAt(ImageTableHelpers.GetErrorGraphicsElement(index), insertBefore);

	public void InsertAt(GraphicsElement ge, bool insertBefore)
	{
		var index = ge.ImageTableIndex;

		// find the group this image should go into
		var group = Groups.SingleOrDefault(x => x.GraphicsElements.Any(y => y.ImageTableIndex == index));

		ArgumentNullException.ThrowIfNull(group, nameof(group));

		// find the position to insert into the group
		var insertPos = group.GraphicsElements.FindIndex(x => x.ImageTableIndex == index);
		insertPos += insertBefore ? 0 : 1;

		// update the ImageTableIndex of all images at or after this index
		foreach (var g in Groups)
		{
			foreach (var ge2 in g.GraphicsElements)
			{
				if (ge2.ImageTableIndex >= index)
				{
					++ge2.ImageTableIndex;
				}
			}
		}

		// actually insert it
		if (insertPos >= 0 && insertPos < group.GraphicsElements.Count)
		{
			group.GraphicsElements.Insert(insertPos, ge);
		}
		else
		{
			group.GraphicsElements.Add(ge);
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
			foreach (var ge in g.GraphicsElements)
			{
				if (ge.ImageTableIndex > index)
				{
					--ge.ImageTableIndex;
				}
			}
		}
	}

	// public/old interface
	public List<GraphicsElement> GraphicsElements
		=> [.. Groups
			.SelectMany(x => x.GraphicsElements)
			.OrderBy(x => x.ImageTableIndex)];

	public List<ImageTableGroup> Groups { get; set; } = [];
}
