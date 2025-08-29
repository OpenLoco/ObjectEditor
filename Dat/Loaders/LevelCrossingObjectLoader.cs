using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.LevelCrossing;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class LevelCrossingObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x12;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new LevelCrossingObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.CostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.AnimationSpeed = br.ReadByte();
			model.ClosingFrames = br.ReadByte();
			model.ClosedFrames = br.ReadByte();
			model.var_0A = br.ReadByte(); // something like IdleAnimationFrames or something
			br.SkipByte(1); // pad_0B, unused
			model.DesignedYear = br.ReadUInt16();
			br.SkipImageId();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.LevelCrossing), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.LevelCrossing, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as LevelCrossingObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.CostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.AnimationSpeed);
			bw.Write(model.ClosingFrames);
			bw.Write(model.ClosedFrames);
			bw.Write(model.var_0A); // something like IdleAnimationFrames or something
			bw.Write((uint8_t)0); // pad_0B, unused
			bw.Write(model.DesignedYear);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}
}

[LocoStructSize(0x12)]
[LocoStructType(DatObjectType.LevelCrossing)]
internal record DatLevelCrossingObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] int16_t CostFactor,
	[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x06)] uint8_t CostIndex,
	[property: LocoStructOffset(0x07)] uint8_t AnimationSpeed,
	[property: LocoStructOffset(0x08)] uint8_t ClosingFrames,
	[property: LocoStructOffset(0x09)] uint8_t ClosedFrames,
	[property: LocoStructOffset(0x0A)] uint8_t var_0A, // something like IdleAnimationFrames or something
	[property: LocoStructOffset(0x0B), LocoPropertyMaybeUnused] uint8_t pad_0B,
	[property: LocoStructOffset(0x0C)] uint16_t DesignedYear,
	[property: LocoStructOffset(0x0E), Browsable(false)] image_id Image
	)
{ }
