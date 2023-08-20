
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum SteamObjectFlags : uint16_t
	{
		None = 0,
		ApplyWind = 1 << 0,
		DisperseOnCollision = 1 << 1,
		unk2 = 1 << 2,
		unk3 = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]
	[LocoStringCount(0)]
	public record ImageAndHeight(
		[property: LocoStructOffset(0x00)] uint8_t ImageOffset,
		[property: LocoStructOffset(0x01)] uint8_t Height
	) : ILocoStruct
	{
		public static int StructLength => 0x02;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x28)]
	public record SteamObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint16_t NumImages,
		[property: LocoStructOffset(0x04)] uint8_t NumStationaryTicks, // while stationary can be affected by wind
		[property: LocoStructOffset(0x05)] uint8_t SpriteWidth,
		[property: LocoStructOffset(0x06)] uint8_t SpriteHeightNegative,
		[property: LocoStructOffset(0x07)] uint8_t SpriteHeightPositive,
		[property: LocoStructOffset(0x08)] SteamObjectFlags Flags,
		[property: LocoStructOffset(0x0A)] uint32_t var_0A,
		[property: LocoStructOffset(0x0E)] uint32_t BaseImageId,
		[property: LocoStructOffset(0x12)] uint16_t TotalNumFramesType0,
		[property: LocoStructOffset(0x14)] uint16_t TotalNumFramesType1,
		//[property: LocoStructProperty(0x16)] const ImageAndHeight* FrameInfoType0,
		//[property: LocoStructProperty(0x1A)] const ImageAndHeight* FrameInfoType1,
		[property: LocoStructOffset(0x1E)] uint8_t NumSoundEffects,
		[property: LocoStructOffset(0x01F), LocoArrayLength(9)] SoundObjectId[] SoundEffects // size tbc
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.steam;
		public static int StructLength => 0x28;
	}
}
