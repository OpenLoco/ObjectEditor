using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VehicleObjectBodySprite
	{
		uint8_t numFlatRotationFrames;   // 0x00 4, 8, 16, 32, 64?
		uint8_t numSlopedRotationFrames; // 0x01 4, 8, 16, 32?
		uint8_t numAnimationFrames;      // 0x02
		uint8_t numCargoLoadFrames;      // 0x03
		uint8_t numCargoFrames;          // 0x04
		uint8_t numRollFrames;           // 0x05
		uint8_t bogeyPosition;           // 0x06
		BodySpriteFlags flags;           // 0x07
		uint8_t width;                   // 0x08 sprite width
		uint8_t heightNegative;          // 0x09 sprite height negative
		uint8_t heightPositive;          // 0x0A sprite height positive
		uint8_t flatYawAccuracy;         // 0x0B 0 - 4 accuracy of yaw on flat built from numFlatRotationFrames (0 = lowest accuracy 3bits, 4 = highest accuracy 7bits)
		uint8_t slopedYawAccuracy;       // 0x0C 0 - 3 accuracy of yaw on slopes built from numSlopedRotationFrames  (0 = lowest accuracy 3bits, 3 = highest accuracy 6bits)
		uint8_t numFramesPerRotation;    // 0x0D numAnimationFrames * numCargoFrames * numRollFrames + 1 (for braking lights)
		uint32_t flatImageId;            // 0x0E
		uint32_t unkImageId;             // 0x12
		uint32_t gentleImageId;          // 0x16
		uint32_t steepImageId;           // 0x1A
	};
}
