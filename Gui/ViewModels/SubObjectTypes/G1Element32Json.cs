using Dat.Types;
using System.Text.Json.Serialization;

namespace Gui.ViewModels;

public record G1Element32Json(
	[property: JsonPropertyName("path")] string Path,
	[property: JsonPropertyName("x")] int16_t XOffset,
	[property: JsonPropertyName("y")] int16_t YOffset,
	[property: JsonPropertyName("zoomOffset")] int16_t? ZoomOffset,
	[property: JsonPropertyName("flags")] G1ElementFlags? Flags
	)
{
	public G1Element32Json()
		: this("", 0, 0, null, null)
	{ }

	public G1Element32Json(string path, int16_t xOffset, int16_t yOffset)
		: this(path, xOffset, yOffset, null, null)
	{ }

	public G1Element32Json(string path, G1Element32 g1Element)
		: this(path, g1Element.XOffset, g1Element.YOffset, g1Element.ZoomOffset, g1Element.Flags)
	{ }

	public static G1Element32Json Zero
		=> new(string.Empty, 0, 0, 0, G1ElementFlags.None);
}
