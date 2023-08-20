using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x15E)]
	public record VehicleObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] TransportMode Mode,
		[property: LocoStructOffset(0x03)] VehicleType Type,
		[property: LocoStructOffset(0x04)] uint8_t var_04,
		[property: LocoStructOffset(0x05)] uint8_t TrackType,
		[property: LocoStructOffset(0x06)] uint8_t NumMods,
		[property: LocoStructOffset(0x07)] uint8_t CostIndex,
		[property: LocoStructOffset(0x08)] int16_t CostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t Reliability,
		[property: LocoStructOffset(0x0B)] uint8_t RunCostIndex,
		[property: LocoStructOffset(0x0C)] int16_t RunCostFactor,
		[property: LocoStructOffset(0x0E)] uint8_t ColourType,
		[property: LocoStructOffset(0x0F)] uint8_t NumCompat,
		[property: LocoStructOffset(0x10), LocoArrayLength(8)] uint16_t[] CompatibleVehicles, // array of compatible vehicle_types
		[property: LocoStructOffset(0x20), LocoArrayLength(4)] uint8_t[] RequiredTrackExtras,
		[property: LocoStructOffset(0x24), LocoArrayLength(4)] VehicleObjectUnk[] var_24,
		[property: LocoStructOffset(0x3C), LocoArrayLength(VehicleObject.MaxBodySprites)] BodySprite[] BodySprites,
		[property: LocoStructOffset(0xB4), LocoArrayLength(2)] BogieSprite[] BogieSprites,
		[property: LocoStructOffset(0xD8)] uint16_t Power,
		[property: LocoStructOffset(0xDA)] Speed16 Speed,
		[property: LocoStructOffset(0xDC)] Speed16 RackSpeed,
		[property: LocoStructOffset(0xDE)] uint16_t Weight,
		[property: LocoStructOffset(0xE0)] VehicleObjectFlags Flags,
		[property: LocoStructOffset(0xE2), LocoArrayLength(2)] uint8_t[] MaxCargo, // size is relative to the first cargoTypes
		[property: LocoStructOffset(0xE4), LocoArrayLength(2)] uint32_t[] CargoTypes,
		[property: LocoStructOffset(0xEC), LocoArrayLength(32)] uint8_t[] CargoTypeSpriteOffsets,
		[property: LocoStructOffset(0x10C)] uint8_t NumSimultaneousCargoTypes,
		[property: LocoStructOffset(0x10D), LocoArrayLength(2)] SimpleAnimation[] Animation,
		[property: LocoStructOffset(0x113)] uint8_t var_113,
		[property: LocoStructOffset(0x114)] uint16_t Designed,
		[property: LocoStructOffset(0x116)] uint16_t Obsolete,
		[property: LocoStructOffset(0x118)] uint8_t RackRailType,
		[property: LocoStructOffset(0x119)] DrivingSoundType DrivingSoundType,
		//union
		//{
		//	VehicleObjectFrictionSound friction,
		//	VehicleObjectEngine1Sound engine1,
		//	VehicleObjectEngine2Sound engine2,
		//}
		//sound,
		[property: LocoStructOffset(0x135), LocoArrayLength(0x15A - 0x135)] uint8_t[] pad_135,
		[property: LocoStructOffset(0x15A)] uint8_t NumStartSounds, // use mask when accessing kHasCrossingWhistle stuffed in (1 << 7)
		[property: LocoStructOffset(0x15B), LocoArrayLength(3)] SoundObjectId[] StartSounds // sound array length numStartSounds highest sound is the crossing whistle
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.vehicle;
		public static int StructLength => 0x15E;

		public const int MaxBodySprites = 4;
	}
}
