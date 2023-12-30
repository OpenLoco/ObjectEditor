
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.Region)]
	[LocoStringTable("Name")]
	public class RegionObject(
		uint8_t requiredObjectCount)
		: ILocoStruct, ILocoStructVariableData
	{

		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		//[property: LocoStructOffset(0x02)] image_id Image,
		//[property: LocoStructOffset(0x06), LocoArrayLength(0x8 - 0x6)] uint8_t[] pad_06,
		[property: LocoStructOffset(0x08)] public uint8_t RequiredObjectCount { get; set; } = requiredObjectCount;
		//[property: LocoStructOffset(0x09), LocoArrayLength(4)] public uint8_t[] requiredObjects { get; set; }
		//[property: LocoStructOffset(0x0D), LocoArrayLength(0x12 - 0xD)] uint8_t[] pad_0D

		public List<S5Header> RequiredObjects { get; set; } = new();
		public List<S5Header> DependentObjects { get; set; } = new();

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			RequiredObjects.Clear();
			DependentObjects.Clear();

			// unk
			for (var i = 0; i < RequiredObjectCount; ++i)
			{
				RequiredObjects.Add(S5Header.Read(remainingData[..S5Header.StructLength]));
				remainingData = remainingData[S5Header.StructLength..];
			}

			// dependent objects
			var ptr = 0;
			while (remainingData[ptr] != 0xFF)
			{
				DependentObjects.Add(S5Header.Read(remainingData[..S5Header.StructLength]));
				ptr += S5Header.StructLength;
			}

			ptr++;
			return remainingData[ptr..];
		}

		public ReadOnlySpan<byte> Save()
		{
			var variableBytesLength = S5Header.StructLength * (RequiredObjects.Count + DependentObjects.Count);
			var data = new byte[variableBytesLength];
			var span = data.AsSpan();

			var ptr = 0;
			foreach (var reqObj in RequiredObjects.Concat(DependentObjects))
			{
				var bytes = reqObj.Write();
				bytes.CopyTo(span[ptr..(ptr + S5Header.StructLength)]);
				ptr += S5Header.StructLength;
			}

			return data;
		}
	}
}
