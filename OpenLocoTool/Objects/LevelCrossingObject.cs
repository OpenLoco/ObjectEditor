
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record LevelCrossingObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] int16_t CostFactor,     // 0x02
		[property: LocoStructProperty] int16_t SellCostFactor, // 0x04
		[property: LocoStructProperty] uint8_t CostIndex,      // 0x06
		[property: LocoStructProperty] uint8_t AnimationSpeed, // 0x07
		[property: LocoStructProperty] uint8_t ClosingFrames,  // 0x08
		[property: LocoStructProperty] uint8_t ClosedFrames,   // 0x09
		[property: LocoStructProperty, LocoArrayLength(0x0C - 0x0A)] uint8_t pad_0A,
		[property: LocoStructProperty] uint16_t DesignedYear, // 0x0C
		[property: LocoStructProperty] uint32_t Image         // 0x0E
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.levelCrossing;
		public int ObjectStructSize => 0x12;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
