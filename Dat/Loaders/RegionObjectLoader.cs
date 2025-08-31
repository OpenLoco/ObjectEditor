using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
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
		public const int Dat = 0x12;
		public const int CargoInfluenceTownFilterType = 0x01;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RegionObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			br.SkipByte(0x08 - 0x06); // pad
			var numCargoInfluenceObjects = br.ReadByte();
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				model.CargoInfluenceTownFilter.Add((CargoInfluenceTownFilterType)br.ReadByte()); // Cargo influence town filter
			}

			br.SkipByte(Constants.MaxCargoInfluenceObjects * StructSizes.CargoInfluenceTownFilterType); // Cargo influence town filter
			br.SkipByte(); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Region), null);

			// variable
			model.CargoInfluenceObjects = br.ReadS5HeaderList(numCargoInfluenceObjects);
			model.DependentObjects = br.ReadS5HeaderList();

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Region, model, stringTable, imageTable);
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
			bw.Write((uint16_t)0); // pad
			bw.Write((uint8_t)model.CargoInfluenceObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxCargoInfluenceObjects);
			for (var i = 0; i < Constants.MaxCargoInfluenceObjects; ++i)
			{
				bw.Write((uint8_t)model.CargoInfluenceTownFilter[i]); // Cargo influence town filter
			}

			bw.Write((uint8_t)0); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

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
