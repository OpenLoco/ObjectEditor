using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x04)]
	public record Category(
		[property: LocoStructOffset(0x00)] uint8_t Count,
		[property: LocoStructOffset(0x01)] uint8_t Bias,
		[property: LocoStructOffset(0x02)] uint16_t Offset
		) : ILocoStruct
	{
		public bool Validate() => true;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1A)]
	[LocoStructType(ObjectType.TownNames)]
	[LocoStringTable("Name")]
	public record TownNamesObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoArrayLength(6)] Category[] Categories
	) : ILocoStruct, ILocoStructVariableData
	{
		public const int MinNumNameCombinations = 80;

		byte[] tempUnkVariableData;

		public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
		{
			// town names is interesting - loco has not RE'd the whole object and there are no graphics, so we just
			// skip the rest of the data/object
			tempUnkVariableData = remainingData.ToArray();
			return remainingData[remainingData.Length..];
		}

		public ReadOnlySpan<byte> SaveVariable()
			=> tempUnkVariableData;

		public bool Validate() => true;
	}
}
