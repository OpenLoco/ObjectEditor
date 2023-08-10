
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
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t CostIndex,
		[property: LocoStructProperty(0x03)] uint8_t var_03,
		[property: LocoStructProperty(0x04)] uint8_t var_04,
		[property: LocoStructProperty(0x05)] LandObjectFlags Flags,
		[property: LocoStructProperty(0x06)] uint8_t var_06,
		[property: LocoStructProperty(0x07)] uint8_t var_07,
		[property: LocoStructProperty(0x08)] int8_t CostFactor,
		[property: LocoStructProperty(0x09)] uint8_t pad_09,
		[property: LocoStructProperty(0x0A)] uint32_t Image,
		[property: LocoStructProperty(0x0E)] uint32_t var_0E,
		[property: LocoStructProperty(0x12)] uint32_t var_12,
		[property: LocoStructProperty(0x16)] uint32_t var_16,
		[property: LocoStructProperty(0x1A)] uint8_t pad_1A,
		[property: LocoStructProperty(0x1B)] uint8_t NumVariations,
		[property: LocoStructProperty(0x1C)] uint8_t VariationLikelihood,
		[property: LocoStructProperty(0x1D)] uint8_t pad_1D
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.land;
		public static int StructLength => 0x1E;
	}
}
