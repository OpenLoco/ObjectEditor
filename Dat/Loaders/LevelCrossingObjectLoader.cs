// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.LevelCrossing;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class LevelCrossingObjectLoader : IDatObjectLoader
{
	public static ObjectType ObjectType => ObjectType.LevelCrossing;
	public static DatObjectType DatObjectType => DatObjectType.LevelCrossing;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new LevelCrossingObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.AnimationSpeed = br.ReadByte();
			model.IdleClosedFrames = br.ReadByte();
			model.ClosingSequenceFrames = br.ReadByte();
			model.AnimationSpeedBitmask = br.ReadByte(); // something like IdleAnimationFrames or something
			br.SkipByte(); // pad_0B, not part of object definition
			model.DesignedYear = br.ReadUInt16();
			br.SkipImageId();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as LevelCrossingObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.AnimationSpeed);
			bw.Write(model.IdleClosedFrames);
			bw.Write(model.ClosingSequenceFrames);
			bw.Write(model.AnimationSpeedBitmask); // something like IdleAnimationFrames
			bw.WriteEmptyBytes(1); // pad_0B, not part of object definition
			bw.Write(model.DesignedYear);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}
}
