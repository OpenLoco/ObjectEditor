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
		unk1 = 1 << 2,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1E)]
	[LocoStringCount(2)]
	public record TrainSignalObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] TrainSignalObjectFlags Flags,
		[property: LocoStructOffset(0x04)] uint8_t AnimationSpeed,
		[property: LocoStructOffset(0x05)] uint8_t NumFrames,
		[property: LocoStructOffset(0x06)] int16_t CostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] uint8_t var_0B,
		[property: LocoStructOffset(0x0C)] string_id var_0C,
		[property: LocoStructOffset(0x0E)] uint32_t Image,
		[property: LocoStructOffset(0x12)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x13), LocoArrayLength(TrainSignalObject.ModsLength)] uint8_t[] Mods,
		[property: LocoStructOffset(0x1A)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x1C)] uint16_t ObsoleteYear
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.TrainSignal;

		public const int ModsLength = 7;

		public static int StructSize => 0x1E;
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];
			return remainingData;
		}
	}
}
