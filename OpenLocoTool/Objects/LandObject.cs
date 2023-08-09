
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum LandObjectFlags : uint8_t
	{
		None = 0,
		unk0 = 1 << 0,
		unk1 = 1 << 1,
		IsDesert = 1 << 2,
		NoTrees = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record LandObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] uint8_t CostIndex, // 0x02
		[property: LocoStructProperty] uint8_t var_03,
		[property: LocoStructProperty] uint8_t var_04,
		[property: LocoStructProperty] LandObjectFlags Flags, // 0x05
		[property: LocoStructProperty] uint8_t var_06,
		[property: LocoStructProperty] uint8_t var_07,
		[property: LocoStructProperty] int8_t CostFactor, // 0x08
		[property: LocoStructProperty] uint8_t pad_09,
		[property: LocoStructProperty] uint32_t Image, // 0x0A
		[property: LocoStructProperty] uint32_t var_0E,
		[property: LocoStructProperty] uint32_t var_12,
		[property: LocoStructProperty] uint32_t var_16,
		[property: LocoStructProperty] uint8_t pad_1A,
		[property: LocoStructProperty] uint8_t NumVariations,       // 0x1B
		[property: LocoStructProperty] uint8_t VariationLikelihood, // 0x1C
		[property: LocoStructProperty] uint8_t pad_1D
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.land;
		public int ObjectStructSize => 0x1E;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
