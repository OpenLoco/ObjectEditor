using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;
public static class ImageTableLoader
{
	public static ImageTable CreateImageTable(List<GraphicsElement> imageList)
	{
		var imageTable = new ImageTable();
		CreateBasicGroups(imageList, imageTable);
		return imageTable;
	}

	private static void CreateBasicGroups(List<GraphicsElement> imageList, ImageTable imageTable)
	{
		var chunks = imageList.Chunk(4);
		imageTable.Groups.Add(("Base", chunks.First().ToList()));
		var floorCount = 0;
		foreach (var chunk in chunks.Skip(1))
		{
			imageTable.Groups.Add(($"Floor {floorCount++}", chunk.ToList()));
		}
	}
}
