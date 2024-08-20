using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[Flags]
	public enum TrainSignalObjectFlags : uint16_t
	{
		None = 0 << 0,
		IsLeft = 1 << 0,
		HasLights = 1 << 1,
		unk_02 = 1 << 2,
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
	) : ILocoStruct, ILocoStructVariableData, ILocoImageTableNames
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

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 80, "redLights" },
			{ 88, "redLights2" },
			{ 96, "greenLights" },
			{ 104, "greenLights2" },
		};

		public bool Validate()
		{
			// animationSpeed must be 1 less than a power of 2 (its a mask)
			switch (AnimationSpeed)
			{
				case 0:
				case 1:
				case 3:
				case 7:
				case 15:
					break;
				default:
					return false;
			}

			switch (NumFrames)
			{
				case 4:
				case 7:
				case 10:
					break;
				default:
					return false;
			}

			if (CostIndex > 32)
			{
				return false;
			}

			if (-SellCostFactor > CostFactor)
			{
				return false;
			}

			if (NumCompatible > 7)
			{
				return false;
			}

			return true;
		}
	}
}
