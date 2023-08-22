
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record TownNamesUnk(
		[property: LocoStructOffset(0x00)] uint8_t Count,
		[property: LocoStructOffset(0x01)] uint8_t Fill,
		[property: LocoStructOffset(0x02)] uint16_t Offset
	) : ILocoStruct
	{
		public static int StructSize => 0x04;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1A)]
	public record TownNamesObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02), LocoArrayLength(6)] TownNamesUnk[] unks
		) : ILocoStruct, ILocoStructExtraLoading
	{
		public static ObjectType ObjectType => ObjectType.townNames;
		public static int StructSize => 0x1A;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// town names is interesting - loco has not RE'd the the whole object and there are no graphics, so we just
			// skip the rest of the data/object
			remainingData = remainingData[remainingData.Length..];

			return remainingData;
		}
	}
}
