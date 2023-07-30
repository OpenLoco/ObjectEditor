using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x15E)]
	struct VehicleObject
	{
		//static constexpr auto kObjectType = ObjectType::vehicle { get; set; }
		//static constexpr auto kMaxBodySprites = 4 { get; set; }

		public string_id name { get; set; }     // 0x00
		public TransportMode mode { get; set; } // 0x02
		public VehicleType type { get; set; }   // 0x03
		public uint8_t var_04 { get; set; }
		public uint8_t trackType { get; set; }              // 0x05
		public uint8_t numMods { get; set; }                // 0x06
		public uint8_t costIndex { get; set; }              // 0x07
		public int16_t costFactor { get; set; }             // 0x08
		public uint8_t reliability { get; set; }            // 0x0A
		public uint8_t runCostIndex { get; set; }           // 0x0B
		public int16_t runCostFactor { get; set; }          // 0x0C
		public uint8_t colourType { get; set; }             // 0x0E
		public uint8_t numCompat { get; set; }              // 0x0F
		public unsafe fixed uint16_t compatibleVehicles[8]; // 0x10 array of compatible vehicle_types

		public unsafe fixed uint8_t requiredTrackExtras[4]; // 0x20

		public unsafe fixed byte var_24[4 * 0x6]; //unsafe fixed VehicleObjectUnk var_24[4] { get; set; }

		public unsafe fixed byte bodySprites[4 * 0x1D]; // 0x3C

		public unsafe fixed byte bogieSprites[2 * 0x12]; //VehicleObjectBogieSprite bogieSprites[2] { get; set; }             // 0xB4

		public uint16_t power { get; set; }                                       // 0xD8
		public Speed16 speed { get; set; }                                        // 0xDA
		public Speed16 rackSpeed { get; set; }                                    // 0xDC
		public uint16_t weight { get; set; }                                      // 0xDE
		public VehicleObjectFlags flags { get; set; }                             // 0xE0
		public unsafe fixed uint8_t maxCargo[2];                                   // 0xE2 size is relative to the first cargoTypes

		public unsafe fixed uint32_t cargoTypes[2];                              // 0xE4

		public unsafe fixed uint8_t cargoTypeSpriteOffsets[32];                   // 0xEC

		public uint8_t numSimultaneousCargoTypes { get; set; }                    // 0x10C
		public unsafe fixed byte animation[2 * 0x3];  // VehicleObjectSimpleAnimation           // 0x10D

		public uint8_t var_113 { get; set; }
		public uint16_t designed { get; set; }                 // 0x114
		public uint16_t obsolete { get; set; }                 // 0x116
		public uint8_t rackRailType { get; set; }              // 0x118
		public DrivingSoundType drivingSoundType { get; set; } // 0x119
															   //	union
															   //       {
															   //           VehicleObjectFrictionSound friction { get; set; }
															   //	VehicleObjectEngine1Sound engine1 { get; set; }
															   //	VehicleObjectEngine2Sound engine2 { get; set; }
															   //}
															   //sound { get; set; }
		public VehicleObjectEngine2Sound sound { get; set; } // this is a union in the real obj
		public unsafe fixed uint8_t pad_135[0x15A - 0x135];

		public uint8_t numStartSounds { get; set; }         // 0x15A use mask when accessing kHasCrossingWhistle stuffed in (1 << 7)
		public unsafe fixed SoundObjectId_t startSounds[3];  // 0x15B sound array length numStartSounds highest sound is the crossing whistle

	}
}
