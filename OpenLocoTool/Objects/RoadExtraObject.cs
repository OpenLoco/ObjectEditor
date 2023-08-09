
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record RoadExtraObject(
		[property: LocoStructProperty] string_id Name,          // 0x00
		[property: LocoStructProperty] uint16_t RoadPieces,     // 0x02
		[property: LocoStructProperty] uint8_t PaintStyle,      // 0x04
		[property: LocoStructProperty] uint8_t CostIndex,       // 0x05
		[property: LocoStructProperty] int16_t BuildCostFactor, // 0x06
		[property: LocoStructProperty] int16_t SellCostFactor,  // 0x08
		[property: LocoStructProperty] uint32_t Image,          // 0x0A
		[property: LocoStructProperty] uint32_t var_0E
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.roadExtra;
		public int ObjectStructSize => 0x12;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
