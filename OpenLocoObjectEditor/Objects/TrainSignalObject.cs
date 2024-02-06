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
	[LocoStructType(ObjectType.TrainSignal)]
	[LocoStringTable("Name", "Description")]
	public record TrainSignalObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] TrainSignalObjectFlags Flags,
		[property: LocoStructOffset(0x04)] uint8_t AnimationSpeed,
		[property: LocoStructOffset(0x05)] uint8_t NumFrames,
		[property: LocoStructOffset(0x06)] int16_t CostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] uint8_t var_0B,
		[property: LocoStructOffset(0x0C), LocoString, Browsable(false)] string_id Description,
		[property: LocoStructOffset(0x0E), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x12)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x13), LocoArrayLength(TrainSignalObject.ModsLength), Browsable(false)] object_id[] ModHeaderIds,
		[property: LocoStructOffset(0x1A)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x1C)] uint16_t ObsoleteYear
	) : ILocoStruct, ILocoStructVariableData
	{
		public const int ModsLength = 7;

		public List<S5Header> Mods { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			Mods = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatible);
			return remainingData[(S5Header.StructLength * NumCompatible)..];
		}

		public ReadOnlySpan<byte> Save()
			=> Mods
			.SelectMany(mod => mod.Write().ToArray())
			.ToArray();
	}
}
