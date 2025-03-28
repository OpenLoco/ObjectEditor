using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.Region)]
	[LocoStringTable("Name")]
	public record RegionObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06), LocoArrayLength(0x8 - 0x6)] uint8_t[] var_06,
		[property: LocoStructOffset(0x08), Browsable(false)] uint8_t RequiredObjectCount,
		[property: LocoStructOffset(0x09), LocoArrayLength(RegionObject.MaxRequiredObjects), LocoStructVariableLoad, Browsable(false)] object_id[] _RequiredObjects,
		[property: LocoStructOffset(0x0D), LocoArrayLength(0x12 - 0xD)] uint8_t[] var_0D
		) : ILocoStruct, ILocoStructVariableData
	{
		public const int MaxRequiredObjects = 4;
		public List<S5Header> RequiredObjects { get; set; } = [];
		public List<S5Header> DependentObjects { get; set; } = [];

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// required objects
			RequiredObjects.Clear();
			RequiredObjects = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, RequiredObjectCount);
			remainingData = remainingData[(S5Header.StructLength * RequiredObjectCount)..];

			// dependent objects
			DependentObjects.Clear();
			while (remainingData[0] != 0xFF)
			{
				DependentObjects.Add(S5Header.Read(remainingData[..S5Header.StructLength]));
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData[1..];
		}

		public ReadOnlySpan<byte> Save()
		{
			var variableBytesLength = (S5Header.StructLength * (RequiredObjects.Count + DependentObjects.Count)) + 1;
			var span = new byte[variableBytesLength].AsSpan();

			var ptr = 0;
			foreach (var reqObj in RequiredObjects.Concat(DependentObjects))
			{
				var bytes = reqObj.Write();
				bytes.CopyTo(span[ptr..(ptr + S5Header.StructLength)]);
				ptr += S5Header.StructLength;
			}

			span[^1] = 0xFF;
			return span;
		}

		public bool Validate()
			=> true;
	}
}
