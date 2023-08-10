
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
	public record struct ImageAndHeight(
		[property: LocoStructProperty(0x00)] uint8_t ImageOffset,
		[property: LocoStructProperty(0x01)] uint8_t Height
	) : ILocoStruct
	{
		public static int ObjectStructSize => 0x02;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record SteamObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint16_t NumImages,
		[property: LocoStructProperty(0x04)] uint8_t NumStationaryTicks, // while stationary can be affected by wind
		[property: LocoStructProperty(0x05)] uint8_t SpriteWidth,
		[property: LocoStructProperty(0x06)] uint8_t SpriteHeightNegative,
		[property: LocoStructProperty(0x07)] uint8_t SpriteHeightPositive,
		[property: LocoStructProperty(0x08)] SteamObjectFlags Flags,
		[property: LocoStructProperty(0x0A)] uint32_t var_0A,
		[property: LocoStructProperty(0x0E)] uint32_t BaseImageId,
		[property: LocoStructProperty(0x12)] uint16_t TotalNumFramesType0,
		[property: LocoStructProperty(0x14)] uint16_t TotalNumFramesType1,
		//[property: LocoStructProperty(0x16)] const ImageAndHeight* FrameInfoType0,
		//[property: LocoStructProperty(0x1A)] const ImageAndHeight* FrameInfoType1,
		[property: LocoStructProperty(0x1E)] uint8_t NumSoundEffects,
		[property: LocoStructProperty(0x01F), LocoArrayLength(9)] SoundObjectId[] SoundEffects // size tbc
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.steam;
		public static int ObjectStructSize => 0x28;
	}
}
