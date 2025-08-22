using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class RoadExtraObjectLoader : IDatObjectLoader
{
	public static class StructSizes
	{
		public const int DatStructSize = 0x12;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RoadExtraObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			model.RoadPieces = ((DatRoadTraitFlags)br.ReadUInt16()).Convert();
			model.PaintStyle = br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, StructSizes.DatStructSize, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.RoadExtra), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.RoadExtra, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var model = obj.Object as RoadExtraObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.Write((uint16_t)model.RoadPieces);
			bw.Write(model.PaintStyle);
			bw.Write(model.CostIndex);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.WriteImageId(); // Image offset, not part of object definition
			bw.WriteImageId(); // BaseImageOffset, not part of object definition

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}

static class RoadTraitFlagsConverter
{
	public static DatRoadTraitFlags Convert(this RoadTraitFlags roadTraitFlags)
		=> (DatRoadTraitFlags)roadTraitFlags;

	public static RoadTraitFlags Convert(this DatRoadTraitFlags datRoadTraitFlags)
		=> (RoadTraitFlags)datRoadTraitFlags;
}
