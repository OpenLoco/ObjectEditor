using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels;

public class ImageTable : IHasGraphicsElements
{
	// public/old interface
	public List<GraphicsElement> GraphicsElements
	{
		get => [.. Groups.SelectMany(x => x.GraphicsElements)];
		set => Groups.Add(("<unk>)", value));
	}

	public List<(string Name, List<GraphicsElement> GraphicsElements)> Groups { get; set; } = [];
}
