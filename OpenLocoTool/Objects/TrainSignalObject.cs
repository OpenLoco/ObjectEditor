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
	public record TrainSignalObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] TrainSignalObjectFlags Flags,
		[property: LocoStructProperty(0x04)] uint8_t AnimationSpeed,
		[property: LocoStructProperty(0x05)] uint8_t NumFrames,
		[property: LocoStructProperty(0x06)] int16_t CostFactor,
		[property: LocoStructProperty(0x08)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x0A)] uint8_t CostIndex,
		[property: LocoStructProperty(0x0B)] uint8_t var_0B,
		[property: LocoStructProperty(0x0C)] uint16_t var_0C,
		[property: LocoStructProperty(0x0E)] uint32_t Image,
		[property: LocoStructProperty(0x12)] uint8_t NumCompatible,
		[property: LocoStructProperty(0x13), LocoArrayLength(TrainSignalObject.ModsLength)] uint8_t[] Mods,
		[property: LocoStructProperty(0x1A)] uint16_t DesignedYear,
		[property: LocoStructProperty(0x1C)] uint16_t ObsoleteYear
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.trainSignal;

		public const int ModsLength = 7;

		public static int StructLength => 0x1E;
	}
}
