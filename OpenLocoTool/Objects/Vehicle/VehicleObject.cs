using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record VehicleObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] TransportMode Mode,
		[property: LocoStructProperty(0x03)] VehicleType Type,
		[property: LocoStructProperty(0x04)] uint8_t var_04,
		[property: LocoStructProperty(0x05)] uint8_t TrackType,
		[property: LocoStructProperty(0x06)] uint8_t NumMods,
		[property: LocoStructProperty(0x07)] uint8_t CostIndex,
		[property: LocoStructProperty(0x08)] int16_t CostFactor,
		[property: LocoStructProperty(0x0A)] uint8_t Reliability,
		[property: LocoStructProperty(0x0B)] uint8_t RunCostIndex,
		[property: LocoStructProperty(0x0C)] int16_t RunCostFactor,
		[property: LocoStructProperty(0x0E)] uint8_t ColourType,
		[property: LocoStructProperty(0x0F)] uint8_t NumCompat,
		[property: LocoStructProperty(0x10), LocoArrayLength(8)] uint16_t[] CompatibleVehicles, // array of compatible vehicle_types
		[property: LocoStructProperty(0x20), LocoArrayLength(4)] uint8_t[] RequiredTrackExtras,
		[property: LocoStructProperty(0x24), LocoArrayLength(4)] VehicleObjectUnk[] var_24,
		[property: LocoStructProperty(0x3C), LocoArrayLength(VehicleObject.MaxBodySprites)] BodySprite[] BodySprites,
		[property: LocoStructProperty(0xB4), LocoArrayLength(2)] BogieSprite[] BogieSprites,
		[property: LocoStructProperty(0xD8)] uint16_t Power,
		[property: LocoStructProperty(0xDA)] Speed16 Speed,
		[property: LocoStructProperty(0xDC)] Speed16 RackSpeed,
		[property: LocoStructProperty(0xDE)] uint16_t Weight,
		[property: LocoStructProperty(0xE0)] VehicleObjectFlags Flags,
		[property: LocoStructProperty(0xE2), LocoArrayLength(2)] uint8_t[] MaxCargo, // size is relative to the first cargoTypes
		[property: LocoStructProperty(0xE4), LocoArrayLength(2)] uint32_t[] CargoTypes,
		[property: LocoStructProperty(0xEC), LocoArrayLength(32)] uint8_t[] CargoTypeSpriteOffsets,
		[property: LocoStructProperty(0x10C)] uint8_t NumSimultaneousCargoTypes,
		[property: LocoStructProperty(0x10D), LocoArrayLength(2)] SimpleAnimation[] Animation,
		[property: LocoStructProperty(0x113)] uint8_t var_113,
		[property: LocoStructProperty(0x114)] uint16_t Designed,
		[property: LocoStructProperty(0x116)] uint16_t Obsolete,
		[property: LocoStructProperty(0x118)] uint8_t RackRailType,
		[property: LocoStructProperty(0x119)] DrivingSoundType DrivingSoundType,
		//union
		//{
		//	VehicleObjectFrictionSound friction,
		//	VehicleObjectEngine1Sound engine1,
		//	VehicleObjectEngine2Sound engine2,
		//}
		//sound,
		[property: LocoStructProperty(0x135), LocoArrayLength(0x15A - 0x135)] uint8_t[] pad_135,
		[property: LocoStructProperty(0x15A)] uint8_t NumStartSounds, // use mask when accessing kHasCrossingWhistle stuffed in (1 << 7)
		[property: LocoStructProperty(0x15B), LocoArrayLength(3)] SoundObjectId[] StartSounds // sound array length numStartSounds highest sound is the crossing whistle
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.vehicle;
		public static int StructLength => 0x15E;

		public const int MaxBodySprites = 4;
	}
}
