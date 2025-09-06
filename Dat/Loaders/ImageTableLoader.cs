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
		=> imageTable.Groups = imageList
			.Chunk(4)
			.Select((x, i) => ($"Part {i}", x.ToList()))
			.ToList();
}
