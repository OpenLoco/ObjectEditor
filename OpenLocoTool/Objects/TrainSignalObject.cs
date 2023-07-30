using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	[Flags]
	enum TrainSignalObjectFlags : uint16_t
	{
		None = 0 << 0,
		IsLeft = 1 << 0,
		HasLights = 1 << 1,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x1E)]
	struct TrainSignalObject
	{
		//static constexpr auto kObjectType = ObjectType::trackSignal;

		public string_id name { get; set; }
		public TrainSignalObjectFlags flags { get; set; } // 0x02
		public uint8_t animationSpeed { get; set; }       // 0x04
		public uint8_t numFrames { get; set; }            // 0x05
		public int16_t costFactor { get; set; }           // 0x06
		public int16_t sellCostFactor { get; set; }       // 0x08
		public uint8_t costIndex { get; set; }            // 0x0A
		public uint8_t var_0B { get; set; }
		public uint16_t var_0C { get; set; }
		public uint32_t image { get; set; }        // 0x0E
		public uint8_t numCompatible { get; set; } // 0x12
		public unsafe fixed uint8_t mods[7]; // 0x13
		public uint16_t designedYear { get; set; } // 0x1A
		public uint16_t obsoleteYear { get; set; } // 0x1C

		//bool validate() const;
		//void load(const LoadedObjectHandle& handle, stdx::span<const std::byte> data, ObjectManager::DependentObjects*);
		//      void unload();
		//void drawPreviewImage(Gfx::RenderTarget& rt, const int16_t x, const int16_t y) const;
		//constexpr bool hasFlags(TrainSignalObjectFlags flagsToTest) const
		//      {
		//          return (flags & flagsToTest) != TrainSignalObjectFlags::none;
		//      }
	};
}
