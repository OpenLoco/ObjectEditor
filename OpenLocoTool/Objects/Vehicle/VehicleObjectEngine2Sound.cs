using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x1B)]
	struct VehicleObjectEngine2Sound
	{
		public uint8_t soundObjectId { get; set; }         // 0x0
		public uint16_t defaultFrequency { get; set; }     // 0x1
		public uint8_t defaultVolume { get; set; }         // 0x2
		public uint16_t firstGearFrequency { get; set; }   // 0x4 All subsequent gears are based on this frequency
		public Speed16 firstGearSpeed { get; set; }        // 0x6
		public uint16_t secondGearFreqFactor { get; set; } // 0x8
		public Speed16 secondGearSpeed { get; set; }       // 0xA
		public uint16_t thirdGearFreqFactor { get; set; }  // 0xC
		public Speed16 thirdGearSpeed { get; set; }        // 0xE
		public uint16_t fourthGearFreqFactor { get; set; } // 0x10
		public uint8_t var_12 { get; set; }
		public uint8_t var_13 { get; set; }
		public uint16_t freqIncreaseStep { get; set; }  // 0x14
		public uint16_t freqDecreaseStep { get; set; }  // 0x16
		public uint8_t volumeIncreaseStep { get; set; } // 0x18
		public uint8_t volumeDecreaseStep { get; set; } // 0x19
		public uint8_t speedFreqFactor { get; set; }    // 0x1A
	};


}
