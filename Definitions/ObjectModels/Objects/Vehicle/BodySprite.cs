using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BodySprite : ILocoStruct
{
	public uint8_t NumFlatRotationFrames { get; set; }
	public uint8_t NumSlopedRotationFrames { get; set; }
	public uint8_t NumAnimationFrames { get; set; }
	public uint8_t NumCargoLoadFrames { get; set; }
	public uint8_t NumCargoFrames { get; set; }
	public uint8_t NumRollFrames { get; set; }
	public uint8_t HalfLength { get; set; }
	public BodySpriteFlags Flags { get; set; }
	[Browsable(false)] public uint8_t _Width { get; set; }
	[Browsable(false)] public uint8_t _HeightNegative { get; set; }
	[Browsable(false)] public uint8_t _HeightPositive { get; set; }
	[Browsable(false)] public uint8_t _FlatYawAccuracy { get; set; }
	[Browsable(false)] public uint8_t _SlopedYawAccuracy { get; set; }
	[Browsable(false)] public uint8_t _NumFramesPerRotation { get; set; }
	[Browsable(false)] public image_id _FlatImageId { get; set; }
	[Browsable(false)] public image_id _UnkImageId { get; set; }
	[Browsable(false)] public image_id _GentleImageId { get; set; }
	[Browsable(false)] public image_id _SteepImageId { get; set; }

	//public Dictionary<BodySpriteSlopeType, List<int>> ImageIds { get; set; } = [];
	//public int NumImages { get; set; }

	public bool Validate() => true;
}
