using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x36)]

	struct TrackObject
	{
		//static constexpr auto kObjectType = ObjectType::track;

		public string_id name { get; set; }
		public TrackObjectPieceFlags trackPieces { get; set; } // 0x02
		public uint16_t stationTrackPieces { get; set; }       // 0x04
		public uint8_t var_06 { get; set; }
		public uint8_t numCompatible { get; set; }     // 0x07
		public uint8_t numMods { get; set; }           // 0x08
		public uint8_t numSignals { get; set; }        // 0x09
		public unsafe fixed uint8_t mods[4];           // 0x0A
		public uint16_t signals { get; set; }          // 0x0E bitset
		public uint16_t compatibleTracks { get; set; } // 0x10 bitset
		public uint16_t compatibleRoads { get; set; }  // 0x12 bitset
		public int16_t buildCostFactor { get; set; }   // 0x14
		public int16_t sellCostFactor { get; set; }    // 0x16
		public int16_t tunnelCostFactor { get; set; }  // 0x18
		public uint8_t costIndex { get; set; }         // 0x1A
		public uint8_t tunnel { get; set; }            // 0x1B
		public uint16_t curveSpeed { get; set; }       // 0x1C
		public uint32_t image { get; set; }            // 0x1E
		public TrackObjectFlags flags { get; set; }    // 0x22
		public uint8_t numBridges { get; set; }        // 0x24
		public unsafe fixed uint8_t bridges[7];        // 0x25
		public uint8_t numStations { get; set; }       // 0x2C
		public unsafe fixed uint8_t stations[7];       // 0x2D
		public uint8_t displayOffset { get; set; }     // 0x34
		public uint8_t pad_35 { get; set; }
	}
}