// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class RegionObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxCargoInfluenceObjects = 4;
	}

	public static class StructSizes
	{
		public const int CargoInfluenceTownFilterType = 0x01;
	}

	public static ObjectType ObjectType => ObjectType.Region;
	public static DatObjectType DatObjectType => DatObjectType.Region;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RegionObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			model.VehicleDrivingSide = (DatDrivingSide)br.ReadUInt16() == DatDrivingSide.Left ? DrivingSide.Left : DrivingSide.Right;
			var numCargoInfluenceObjects = br.ReadByte();
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				model.CargoInfluenceTownFilter.Add((CargoInfluenceTownFilterType)br.ReadByte()); // Cargo influence town filter
			}

			br.SkipBytes(Constants.MaxCargoInfluenceObjects * StructSizes.CargoInfluenceTownFilterType); // Cargo influence town filter
			br.SkipByte(); // 0x11 is a padding byte

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			model.CargoInfluenceObjects = [.. br.ReadS5HeaderList(numCargoInfluenceObjects)];
			model.DependentObjects = [.. br.ReadS5HeaderList()];

			// image table
			// N/A but Region has an empty image table for some reason
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (RegionObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write(model.VehicleDrivingSide == DrivingSide.Left ? (uint16_t)DatDrivingSide.Left : (uint16_t)DatDrivingSide.Right);
			bw.Write((uint8_t)model.CargoInfluenceObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxCargoInfluenceObjects);
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				bw.Write((uint8_t)model.CargoInfluenceTownFilter[i]); // Cargo influence town filter
			}

			bw.Write((byte)0); // 0x11 is a padding byte

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.CargoInfluenceObjects);
			bw.WriteS5HeaderList(model.DependentObjects);
			bw.WriteTerminator(); // end of dependent objects

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatDrivingSide : uint16_t
	{
		Left,
		Right
	}

	internal enum DatCargoInfluenceTownFilterType : uint8_t
	{
		AllTowns = 0,
		MaySnow = 1,
		InDesert = 2,
	}
}
