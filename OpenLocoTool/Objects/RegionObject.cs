
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	//[LocoStringTable("Name")]
	public record RegionObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		//[property: LocoStructOffset(0x02)] uint32_t Image,
		[property: LocoStructOffset(0x06), LocoArrayLength(0x8 - 0x6)] uint8_t[] pad_06,
		[property: LocoStructOffset(0x08)] uint8_t var_08,
		[property: LocoStructOffset(0x09), LocoArrayLength(4)] uint8_t[] var_09,
		[property: LocoStructOffset(0x0D), LocoArrayLength(0x12 - 0xD)] uint8_t[] pad_0D
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Region;
		public static int StructSize => 0x12;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// unk
			remainingData = remainingData[(S5Header.StructLength * var_08)..];

			// find all objects that this object loads
			var ptr = 0;
			while (remainingData[ptr] != 0xFF)
			{
				ptr += S5Header.StructLength;
			}

			ptr++;
			remainingData = remainingData[ptr..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
