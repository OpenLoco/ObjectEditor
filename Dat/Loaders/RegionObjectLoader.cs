using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class RegionObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxCargoInfluenceObjects = 4;
	}

	public static class StructSizes
	{
		public const int Dat = 0x12;
		public const int CargoInfluenceTownFilterType = 0x01;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RegionObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			_ = br.SkipImageId(); // Image offset, not part of object definition
			_ = br.SkipBytes(0x08 - 0x06); // pad
			var numCargoInfluenceObjects = br.ReadByte();
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				model.CargoInfluenceTownFilter.Add((CargoInfluenceTownFilterType)br.ReadByte()); // Cargo influence town filter
			}
			_ = br.SkipBytes(Constants.MaxCargoInfluenceObjects * StructSizes.CargoInfluenceTownFilterType); // Cargo influence town filter
			_ = br.SkipByte(); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Region), null);

			// variable
			model.CargoInfluenceObjects = br.ReadS5HeaderList(numCargoInfluenceObjects);
			model.DependentObjects = br.ReadS5HeaderList();

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Region, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (RegionObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.WriteImageId(); // Image offset, not part of object definition
			bw.Write((uint16_t)0); // pad
			bw.Write((uint8_t)model.CargoInfluenceObjects.Count);
			bw.WriteObjectId(Constants.MaxCargoInfluenceObjects);
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				bw.Write((uint8_t)model.CargoInfluenceTownFilter[i]); // Cargo influence town filter
			}
			bw.Write((uint8_t)0); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.CargoInfluenceObjects);
			bw.WriteS5HeaderList(model.DependentObjects);
			bw.Write((uint8_t)0xFF); // end of dependent objects

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}

internal enum DatCargoInfluenceTownFilterType : uint8_t
{
	AllTowns = 0,
	MaySnow = 1,
	InDesert = 2,
}

[LocoStructSize(0x12)]
[LocoStructType(DatObjectType.Region)]
internal record DatRegionObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x06), Browsable(false), LocoArrayLength(0x8 - 0x6), LocoPropertyMaybeUnused] uint8_t[] pad_06,
	[property: LocoStructOffset(0x08), Browsable(false)] uint8_t NumCargoInfluenceObjects,
	[property: LocoStructOffset(0x09), LocoArrayLength(RegionObjectLoader.Constants.MaxCargoInfluenceObjects), LocoStructVariableLoad, Browsable(false)] object_id[] _CargoInfluenceObjectIds,
	[property: LocoStructOffset(0x0D), LocoArrayLength(RegionObjectLoader.Constants.MaxCargoInfluenceObjects)] DatCargoInfluenceTownFilterType[] CargoInfluenceTownFilter,
	[property: LocoStructOffset(0x11), Browsable(false), LocoPropertyMaybeUnused] uint8_t pad_11
	) : ILocoStructVariableData
{
	public List<S5Header> CargoInfluenceObjects { get; set; } = [];
	public List<S5Header> DependentObjects { get; set; } = [];

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// cargo influence objects
		CargoInfluenceObjects.Clear();
		CargoInfluenceObjects = SawyerStreamReader.ReadS5HeaderList(remainingData, NumCargoInfluenceObjects);
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
}
