using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Types;

[Flags]
public enum GraphicsElementFlags : uint16_t
{
	None = 0,
	HasTransparency = 1 << 0,   // Image data contains transparent sections (when not set data is plain bmp)
	unk_01 = 1 << 1,            // Unknown function not used on any entry
	IsRLECompressed = 1 << 2,   // Image data is encoded using CS's form of run length encoding
	IsBgr24 = 1 << 3,           // Image data is in Bgr24 format. Is false (unset), image is in an Index8 palette format
	HasZoomSprites = 1 << 4,    // Use a different sprite for higher zoom levels
	NoZoomDraw = 1 << 5,        // Does not get drawn at higher zoom levels (only zoom 0)
	DuplicatePrevious = 1 << 6, // Duplicates the previous element but with adjusted x/y offsets
}

public class GraphicsElement // follows G1Element32, except XOffset and YOffset = are inverted - in loco they're negative but here they're positive
{
	public short Width { get; set; }
	public short Height { get; set; }
	public short XOffset { get; set; }
	public short YOffset { get; set; }
	public GraphicsElementFlags Flags { get; set; }
	public short ZoomOffset { get; set; }
	public byte[] ImageData { get; set; } = [];
	// string Name - taken from IImageNameProvider
}
