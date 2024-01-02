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
	public class TrainSignalObject(
		TrainSignalObjectFlags flags,
		uint8_t animationSpeed,
		uint8_t numFrames,
		int16_t costFactor,
		int16_t sellCostFactor,
		uint8_t costIndex,
		uint8_t var_0B,
		uint8_t numCompatible,
		uint16_t designedYear,
		uint16_t obsoleteYear)
		: ILocoStruct, ILocoStructVariableData
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] public string_id Name { get; set; }
		[LocoStructOffset(0x02)] public TrainSignalObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x04)] public uint8_t AnimationSpeed { get; set; } = animationSpeed;
		[LocoStructOffset(0x05)] public uint8_t NumFrames { get; set; } = numFrames;
		[LocoStructOffset(0x06)] public int16_t CostFactor { get; set; } = costFactor;
		[LocoStructOffset(0x08)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x0A)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x0B)] public uint8_t var_0B { get; set; } = var_0B;
		//[LocoStructOffset(0x0C), LocoString, Browsable(false)] string_id Description { get; set; }
		//[LocoStructOffset(0x0E)] public image_id Image { get; set; }
		[LocoStructOffset(0x12)] public uint8_t NumCompatible { get; set; } = numCompatible;
		//[LocoStructOffset(0x13), LocoArrayLength(ModsLength)] object_index[] Mods { get; set; }
		[LocoStructOffset(0x1A)] public uint16_t DesignedYear { get; set; } = designedYear;
		[LocoStructOffset(0x1C)] public uint16_t ObsoleteYear { get; set; } = obsoleteYear;

		public const int ModsLength = 7;

		public List<S5Header> Mods { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			Mods.Clear();
			Mods = SawyerStreamReader.LoadVariableHeaders(remainingData, NumCompatible);
			return remainingData[(S5Header.StructLength * NumCompatible)..];
		}

		public ReadOnlySpan<byte> Save()
			=> Mods
			.SelectMany(mod => mod.Write().ToArray())
			.ToArray();
	}
}
