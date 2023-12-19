using System.ComponentModel;
using System.Xml.Linq;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Headers
{
	[Flags]
	public enum G1ElementFlags : uint16_t
	{
		None = 0,
		HasTransparency = 1 << 0,   // Image data contains transparent sections (when not set data is plain bmp)
		unk1 = 1 << 1,              // Unknown function not used on any entry
		IsRLECompressed = 1 << 2,   // Image data is encoded using CS's form of run length encoding
		IsR8G8B8Palette = 1 << 3,   // Image data is a sequence of palette entries R8G8B8
		HasZoomSprites = 1 << 4,    // Use a different sprite for higher zoom levels
		NoZoomDraw = 1 << 5,        // Does not get drawn at higher zoom levels (only zoom 0)
		DuplicatePrevious = 1 << 6, // Duplicates the previous element but with adjusted x/y offsets
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x10)]
	public record G1Element32(
		[property: LocoStructOffset(0x00)] uint32_t Offset,
		[property: LocoStructOffset(0x04)] int16_t Width,
		[property: LocoStructOffset(0x06)] int16_t Height,
		[property: LocoStructOffset(0x08)] int16_t XOffset,
		[property: LocoStructOffset(0x0A)] int16_t YOffset,
		[property: LocoStructOffset(0x0C)] G1ElementFlags Flags,
		[property: LocoStructOffset(0x0E)] int16_t ZoomOffset
	) : ILocoStruct
	{
		public static int StructLength => 0x10;
		public byte[] ImageData;

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[StructLength + ImageData.Length];

			var offset = BitConverter.GetBytes(Offset);
			var width = BitConverter.GetBytes(Width);
			var height = BitConverter.GetBytes(Height);
			var xOffset = BitConverter.GetBytes(XOffset);
			var yOffset = BitConverter.GetBytes(YOffset);
			var flags = BitConverter.GetBytes((uint16_t)Flags);
			var zoomOffset = BitConverter.GetBytes(ZoomOffset);

			offset.CopyTo(span, 0);
			width.CopyTo(span, 0x04);
			height.CopyTo(span, 0x06);
			xOffset.CopyTo(span, 0x08);
			yOffset.CopyTo(span, 0x0A);
			flags.CopyTo(span, 0x0C);
			zoomOffset.CopyTo(span, 0x0E);

			ImageData.CopyTo(span, 0x10);

			return span;
		}
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x08)]
	public record G1Header(
		[property: LocoStructOffset(0x00)] uint32_t NumEntries,
		[property: LocoStructOffset(0x04)] uint32_t TotalSize
		) : ILocoStruct
	{
		public static int StructLength => 0x08;
		public byte[] ImageData;
	}
}
