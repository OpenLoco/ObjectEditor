using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	public record BodySprite(
		[property: LocoStructOffset(0x00)] uint8_t NumFlatRotationFrames,   // 4, 8, 16, 32, 64?
		[property: LocoStructOffset(0x01)] uint8_t NumSlopedRotationFrames, // 4, 8, 16, 32?
		[property: LocoStructOffset(0x02)] uint8_t NumAnimationFrames,
		[property: LocoStructOffset(0x03)] uint8_t NumCargoLoadFrames,
		[property: LocoStructOffset(0x04)] uint8_t NumCargoFrames,
		[property: LocoStructOffset(0x05)] uint8_t NumRollFrames,
		[property: LocoStructOffset(0x06)] uint8_t BogeyPosition,
		[property: LocoStructOffset(0x07)] BodySpriteFlags Flags,
		[property: LocoStructOffset(0x08)] uint8_t Width,                // sprite width
		[property: LocoStructOffset(0x09)] uint8_t HeightNegative,       // sprite height negative
		[property: LocoStructOffset(0x0A)] uint8_t HeightPositive,       // sprite height positive
		[property: LocoStructOffset(0x0B)] uint8_t FlatYawAccuracy,      // 0 - 4 accuracy of yaw on flat built from numFlatRotationFrames (0 = lowest accuracy 3bits, 4 = highest accuracy 7bits)
		[property: LocoStructOffset(0x0C)] uint8_t SlopedYawAccuracy,    // 0 - 3 accuracy of yaw on slopes built from numSlopedRotationFrames  (0 = lowest accuracy 3bits, 3 = highest accuracy 6bits)
		[property: LocoStructOffset(0x0D)] uint8_t NumFramesPerRotation, // numAnimationFrames * numCargoFrames * numRollFrames + 1 (for braking lights)
		[property: LocoStructOffset(0x0E)] uint32_t FlatImageId,
		[property: LocoStructOffset(0x12)] uint32_t unkImageId,
		[property: LocoStructOffset(0x16)] uint32_t GentleImageId,
		[property: LocoStructOffset(0x1A)] uint32_t SteepImageId
		) : ILocoStruct
	{
		public static int StructLength => 0x1E;
	}
}
