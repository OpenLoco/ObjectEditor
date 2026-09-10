using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics;

public record GraphicsElementJson(
	[property: JsonPropertyName("path")] string Path,
	[property: JsonPropertyName("x")] int16_t XOffset,
	[property: JsonPropertyName("y")] int16_t YOffset,
	[property: JsonPropertyName("zoomOffset")] int16_t? ZoomOffset,
	[property: JsonPropertyName("flags")] GraphicsElementFlags? Flags,
	[property: JsonPropertyName("name")] string? Name
	)
{
	public GraphicsElementJson()
		: this("", 0, 0, null, null, null)
	{ }

	public GraphicsElementJson(string path, int16_t xOffset, int16_t yOffset, string name)
		: this(path, xOffset, yOffset, null, null, name)
	{ }

	public GraphicsElementJson(string path, GraphicsImage image)
		: this(path, image.XOffset, image.YOffset, image.ZoomOffset, image.Flags, image.Name)
	{ }

	public static GraphicsElementJson Zero
		=> new(string.Empty, 0, 0, 0, GraphicsElementFlags.None, string.Empty);
}
