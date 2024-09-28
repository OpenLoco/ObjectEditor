using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	public enum BodySpriteSlopeType
	{
		Flat,
		Gentle,
		Sloped,
		Steep,
		unk1,
		unk2
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	public record BodySprite(
		[property: LocoStructOffset(0x00)] uint8_t NumFlatRotationFrames,   // 4, 8, 16, 32, 64?
		[property: LocoStructOffset(0x01)] uint8_t NumSlopedRotationFrames, // 4, 8, 16, 32?
		[property: LocoStructOffset(0x02)] uint8_t NumAnimationFrames,
		[property: LocoStructOffset(0x03)] uint8_t NumCargoLoadFrames,
		[property: LocoStructOffset(0x04)] uint8_t NumCargoFrames,
		[property: LocoStructOffset(0x05)] uint8_t NumRollFrames,
		[property: LocoStructOffset(0x06)] uint8_t HalfLength,// the longest distance from pivot of body to either end of car component (not strictly body half length see crocodile train car)
		[property: LocoStructOffset(0x07)] BodySpriteFlags Flags,
		[property: LocoStructOffset(0x08), LocoStructVariableLoad, Browsable(false)] uint8_t _Width,                // sprite width
		[property: LocoStructOffset(0x09), LocoStructVariableLoad, Browsable(false)] uint8_t _HeightNegative,       // sprite height negative
		[property: LocoStructOffset(0x0A), LocoStructVariableLoad, Browsable(false)] uint8_t _HeightPositive,       // sprite height positive
		[property: LocoStructOffset(0x0B), LocoStructVariableLoad, Browsable(false)] uint8_t _FlatYawAccuracy,      // 0 - 4 accuracy of yaw on flat built from numFlatRotationFrames (0 = lowest accuracy 3bits, 4 = highest accuracy 7bits)
		[property: LocoStructOffset(0x0C), LocoStructVariableLoad, Browsable(false)] uint8_t _SlopedYawAccuracy,    // 0 - 3 accuracy of yaw on slopes built from numSlopedRotationFrames  (0 = lowest accuracy 3bits, 3 = highest accuracy 6bits)
		[property: LocoStructOffset(0x0D), LocoStructVariableLoad, Browsable(false)] uint8_t _NumFramesPerRotation, // numAnimationFrames * numCargoFrames * numRollFrames + 1 (for braking lights)
		[property: LocoStructOffset(0x0E), LocoStructVariableLoad, Browsable(false)] image_id _FlatImageId,
		[property: LocoStructOffset(0x12), LocoStructVariableLoad, Browsable(false)] image_id _UnkImageId,
		[property: LocoStructOffset(0x16), LocoStructVariableLoad, Browsable(false)] image_id _GentleImageId,
		[property: LocoStructOffset(0x1A), LocoStructVariableLoad, Browsable(false)] image_id _SteepImageId
		) : ILocoStruct
	{
		// these properties are set on vehicle load and are not saved in the struct/object file itself
		public uint8_t Width { get; set; }
		public uint8_t HeightNegative { get; set; }
		public uint8_t HeightPositive { get; set; }
		public uint8_t FlatYawAccuracy { get; set; }
		public uint8_t SlopedYawAccuracy { get; set; }
		public uint8_t NumFramesPerRotation { get; set; }

		// unused in this tool, but we need to keep them so the object saves properly
		[Browsable(false)] public image_id FlatImageId { get; set; }
		[Browsable(false)] public image_id GentleImageId { get; set; }
		[Browsable(false)] public image_id SlopedImageId { get; set; }
		[Browsable(false)] public image_id SteepImageId { get; set; }
		[Browsable(false)] public image_id UnkImageId1 { get; set; }
		[Browsable(false)] public image_id UnkImageId2 { get; set; }

		//

		public Dictionary<BodySpriteSlopeType, List<int>> ImageIds = [];

		public int NumImages { get; set; }

		public bool Validate() => true;
	}
}
