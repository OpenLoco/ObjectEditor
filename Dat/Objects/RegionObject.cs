using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects
{
	public enum CargoInfluenceTownFilterType : uint8_t
	{
		AllTowns = 0,
		MaySnow = 1,
		InDesert = 2,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.Region)]
	[LocoStringTable("Name")]
	public record RegionObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06), Browsable(false), LocoArrayLength(0x8 - 0x6), LocoPropertyMaybeUnused] uint8_t[] pad_06,
		[property: LocoStructOffset(0x08), Browsable(false)] uint8_t NumCargoInfluenceObjects,
		[property: LocoStructOffset(0x09), LocoArrayLength(RegionObject.MaxCargoInfluenceObjects), LocoStructVariableLoad, Browsable(false)] object_id[] _CargoInfluenceObjectIds,
		[property: LocoStructOffset(0x0D), LocoArrayLength(RegionObject.MaxCargoInfluenceObjects)] CargoInfluenceTownFilterType[] CargoInfluenceTownFilter,
		[property: LocoStructOffset(0x11), Browsable(false), LocoPropertyMaybeUnused] uint8_t pad_11
		) : ILocoStruct, ILocoStructVariableData
	{
		public const int MaxCargoInfluenceObjects = 4;
		public List<S5Header> CargoInfluenceObjects { get; set; } = [];
		public List<S5Header> DependentObjects { get; set; } = [];

		public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
		{
			// cargo influence objects
			CargoInfluenceObjects.Clear();
			CargoInfluenceObjects = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCargoInfluenceObjects);
			remainingData = remainingData[(S5Header.StructLength * NumCargoInfluenceObjects)..];

			// dependent objects
			DependentObjects.Clear();
			while (remainingData[0] != 0xFF)
			{
				DependentObjects.Add(S5Header.Read(remainingData[..S5Header.StructLength]));
				remainingData = remainingData[S5Header.StructLength..];
			}

			return remainingData[1..];
		}

		public ReadOnlySpan<byte> SaveVariable()
		{
			var variableBytesLength = (S5Header.StructLength * (CargoInfluenceObjects.Count + DependentObjects.Count)) + 1;
			var span = new byte[variableBytesLength].AsSpan();

			var ptr = 0;
			foreach (var reqObj in CargoInfluenceObjects.Concat(DependentObjects))
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
