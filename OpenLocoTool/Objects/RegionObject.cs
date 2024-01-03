
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
		//[property: LocoStructOffset(0x09), LocoArrayLength(4)] public object_index[] requiredObjects { get; set; }
		//[property: LocoStructOffset(0x0D), LocoArrayLength(0x12 - 0xD)] uint8_t[] pad_0D

		public List<S5Header> RequiredObjects { get; set; } = [];
		public List<S5Header> DependentObjects { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// unk
			RequiredObjects.Clear();
			RequiredObjects = SawyerStreamReader.LoadVariableHeaders(remainingData, RequiredObjectCount);
			remainingData = remainingData[(S5Header.StructLength * RequiredObjectCount)..];

			// dependent objects
			DependentObjects.Clear();
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
			var variableBytesLength = (S5Header.StructLength * (RequiredObjects.Count + DependentObjects.Count)) + 1;
			var data = new byte[variableBytesLength];
			var span = data.AsSpan();

			var ptr = 0;
			foreach (var reqObj in RequiredObjects.Concat(DependentObjects))
			{
				var bytes = reqObj.Write();
				bytes.CopyTo(span[ptr..(ptr + S5Header.StructLength)]);
				ptr += S5Header.StructLength;
			}
			span[^1] = 0xFF;

			return data;
		}
	}
}
