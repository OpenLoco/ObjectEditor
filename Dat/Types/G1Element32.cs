using OpenLoco.Dat.FileParsing;
using System.ComponentModel;

namespace OpenLoco.Dat.Types
{
	[Flags]
	public enum G1ElementFlags : uint16_t
	{
		None = 0,
		HasTransparency = 1 << 0,   // Image data contains transparent sections (when not set data is plain bmp)
		unk_01 = 1 << 1,            // Unknown function not used on any entry
		IsRLECompressed = 1 << 2,   // Image data is encoded using CS's form of run length encoding
		IsBgr24 = 1 << 3,           // Image data is in Bgr24 format. Is false (unset), image is in an Index8 palette format
		HasZoomSprites = 1 << 4,    // Use a different sprite for higher zoom levels
		NoZoomDraw = 1 << 5,        // Does not get drawn at higher zoom levels (only zoom 0)
		DuplicatePrevious = 1 << 6, // Duplicates the previous element but with adjusted x/y offsets
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x10)]
	public record G1Element32(
		[property: LocoStructOffset(0x00), Browsable(false)] uint32_t Offset,
		[property: LocoStructOffset(0x04)] int16_t Width,
		[property: LocoStructOffset(0x06)] int16_t Height,
		[property: LocoStructOffset(0x08)] int16_t XOffset,
		[property: LocoStructOffset(0x0A)] int16_t YOffset,
		[property: LocoStructOffset(0x0C)] G1ElementFlags Flags,
		[property: LocoStructOffset(0x0E)] int16_t ZoomOffset
	) : ILocoStruct
	{
		public static int StructLength => 0x10;
		public byte[] ImageData { get; set; } = [];

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[StructLength];

			var offset = BitConverter.GetBytes(Offset);
			var width = BitConverter.GetBytes(Width);
			var height = BitConverter.GetBytes(Height);
			var xOffset = BitConverter.GetBytes(XOffset);
			var yOffset = BitConverter.GetBytes(YOffset);
			var flags = BitConverter.GetBytes((uint16_t)Flags);
			var zoomOffset = BitConverter.GetBytes(ZoomOffset);

			offset.CopyTo(span, 0x00);
			width.CopyTo(span, 0x04);
			height.CopyTo(span, 0x06);
			xOffset.CopyTo(span, 0x08);
			yOffset.CopyTo(span, 0x0A);
			flags.CopyTo(span, 0x0C);
			zoomOffset.CopyTo(span, 0x0E);

			// image data is copied later

			return span;
		}

		public byte[] GetImageDataForSave()
			=> GetImageDataForSave(Flags, ImageData, Width, Height);

		public static byte[] GetImageDataForSave(G1ElementFlags flags, byte[] imageData, int width, int height)
			=> flags.HasFlag(G1ElementFlags.IsRLECompressed)
				? SawyerStreamWriter.EncodeRLEImageData(flags, imageData, width, height)
				: imageData;

		public bool Validate() => true;
	}
}
