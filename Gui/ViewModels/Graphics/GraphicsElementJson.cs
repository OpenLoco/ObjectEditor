using Definitions.ObjectModels.Graphics;
using System.Text.Json.Serialization;

namespace Gui.ViewModels.Graphics;

public record GraphicsElementJson(
	[property: JsonPropertyName("path")] string Path,
	[property: JsonPropertyName("x")] int16_t XOffset,
	[property: JsonPropertyName("y")] int16_t YOffset,
	[property: JsonPropertyName("zoomOffset")] int16_t? ZoomOffset,
	[property: JsonPropertyName("flags")] GraphicsElementFlags? Flags,
	[property: JsonPropertyName("name")] string? Name
	// todo: add name
	)
{
	public GraphicsElementJson()
		: this("", 0, 0, null, null, null)
	{ }

	public GraphicsElementJson(string path, int16_t xOffset, int16_t yOffset, string name)
		: this(path, xOffset, yOffset, null, null, name)
	{ }

	public GraphicsElementJson(string path, GraphicsElement g1Element)
		: this(path, g1Element.XOffset, g1Element.YOffset, g1Element.ZoomOffset, g1Element.Flags, g1Element.Name)
	{ }

	public static GraphicsElementJson Zero
		=> new(string.Empty, 0, 0, 0, GraphicsElementFlags.None, string.Empty);
}
