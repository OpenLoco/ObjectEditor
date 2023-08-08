using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum TrainSignalObjectFlags : uint16_t
	{
		None = 0 << 0,
		IsLeft = 1 << 0,
		HasLights = 1 << 1,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record TrainSignalObject(
		[property: LocoStructProperty] string_id Name,    // 0x0,
		[property: LocoStructProperty] TrainSignalObjectFlags Flags, // 0x02
		[property: LocoStructProperty] uint8_t AnimationSpeed,    // 0x04
		[property: LocoStructProperty] uint8_t NumFrames,         // 0x05
		[property: LocoStructProperty] int16_t CostFactor,     // 0x06
		[property: LocoStructProperty] int16_t SellCostFactor, // 0x08
		[property: LocoStructProperty] uint8_t CostIndex,         // 0x0A
		[property: LocoStructProperty] uint8_t var_0B,            // 0x0B
		[property: LocoStructProperty] uint16_t var_0C,        // 0x0C
		[property: LocoStructProperty] uint32_t Image,         // 0x0E
		[property: LocoStructProperty] uint8_t NumCompatible,     // 0x12
		[property: LocoStructProperty, LocoArrayLength(TrainSignalObject.ModCount)] uint8_t[] Mods,            // 0x13 // 7 length
		[property: LocoStructProperty] uint16_t DesignedYear,  // 0x1A
		[property: LocoStructProperty] uint16_t ObsoleteYear   // 0x1C
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.signal;

		public const int ModCount = 7;

		public int ObjectStructSize => 0x1E;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			throw new NotImplementedException("");
		}

		public ReadOnlySpan<byte> Write()
		{
			throw new NotImplementedException("");
		}
	}
}
