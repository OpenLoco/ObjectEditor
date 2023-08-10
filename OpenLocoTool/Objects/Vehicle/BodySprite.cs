using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record BodySprite(
		[property: LocoStructProperty(0x00)] uint8_t NumFlatRotationFrames,   // 4, 8, 16, 32, 64?
		[property: LocoStructProperty(0x01)] uint8_t NumSlopedRotationFrames, // 4, 8, 16, 32?
		[property: LocoStructProperty(0x02)] uint8_t NumAnimationFrames,
		[property: LocoStructProperty(0x03)] uint8_t NumCargoLoadFrames,
		[property: LocoStructProperty(0x04)] uint8_t NumCargoFrames,
		[property: LocoStructProperty(0x05)] uint8_t NumRollFrames,
		[property: LocoStructProperty(0x06)] uint8_t BogeyPosition,
		[property: LocoStructProperty(0x07)] BodySpriteFlags Flags,
		[property: LocoStructProperty(0x08)] uint8_t Width,                // sprite width
		[property: LocoStructProperty(0x09)] uint8_t HeightNegative,       // sprite height negative
		[property: LocoStructProperty(0x0A)] uint8_t HeightPositive,       // sprite height positive
		[property: LocoStructProperty(0x0B)] uint8_t FlatYawAccuracy,      // 0 - 4 accuracy of yaw on flat built from numFlatRotationFrames (0 = lowest accuracy 3bits, 4 = highest accuracy 7bits)
		[property: LocoStructProperty(0x0C)] uint8_t SlopedYawAccuracy,    // 0 - 3 accuracy of yaw on slopes built from numSlopedRotationFrames  (0 = lowest accuracy 3bits, 3 = highest accuracy 6bits)
		[property: LocoStructProperty(0x0D)] uint8_t NumFramesPerRotation, // numAnimationFrames * numCargoFrames * numRollFrames + 1 (for braking lights)
		[property: LocoStructProperty(0x0E)] uint32_t FlatImageId,
		[property: LocoStructProperty(0x12)] uint32_t unkImageId,
		[property: LocoStructProperty(0x16)] uint32_t GentleImageId,
		[property: LocoStructProperty(0x1A)] uint32_t SteepImageId
		) : ILocoStruct
	{
		public static int ObjectStructSize => 0x1E;
	}
}
