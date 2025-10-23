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
			model.VehiclesDriveOnThe = br.ReadByte() == 0 ? DrivingSide.Left : DrivingSide.Right;
			model.pad_07 = br.ReadByte();
			var numCargoInfluenceObjects = br.ReadByte();
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				model.CargoInfluenceTownFilter.Add((CargoInfluenceTownFilterType)br.ReadByte()); // Cargo influence town filter
			}

			br.SkipByte(Constants.MaxCargoInfluenceObjects * StructSizes.CargoInfluenceTownFilterType); // Cargo influence town filter
			model.pad_11 = br.ReadByte();

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
			bw.Write(model.VehiclesDriveOnThe == DrivingSide.Left ? (uint8_t)0 : (uint8_t)1);
			bw.Write(model.pad_07);
			bw.Write((uint8_t)model.CargoInfluenceObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxCargoInfluenceObjects);
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				bw.Write((uint8_t)model.CargoInfluenceTownFilter[i]); // Cargo influence town filter
			}

			bw.Write(model.pad_11); // pad

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

	internal enum DatCargoInfluenceTownFilterType : uint8_t
	{
		AllTowns = 0,
		MaySnow = 1,
		InDesert = 2,
	}
}
