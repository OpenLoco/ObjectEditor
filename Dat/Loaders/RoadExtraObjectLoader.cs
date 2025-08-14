using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadExtra;
using Definitions.ObjectModels.Types;

namespace Dat.Objects;

public abstract class RoadExtraObjectLoader : IDatObjectLoader
{
	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			var model = new RoadExtraObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.BaseStream.Seek(0x02, SeekOrigin.Begin); // Skip name offset
			model.RoadPieces = ((DatRoadTraitFlags)br.ReadUInt16()).Convert();
			model.PaintStyle = br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();

			// move to string table
			var structSize = ObjectAttributes.StructSize(DatObjectType.RoadExtra);
			_ = br.BaseStream.Seek(structSize, SeekOrigin.Begin);

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.RoadExtra), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.RoadExtra, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream ms, LocoObject obj)
	{
		var model = obj.Object as RoadExtraObject;

		using (var bw = new LocoBinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			bw.Write((string_id)0);// Name offset, not part of object definition
			bw.Write((uint16_t)model.RoadPieces);
			bw.Write(model.PaintStyle);
			bw.Write(model.CostIndex);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write((image_id)0); // Image offset, not part of object definition
			bw.Write((image_id)0); // BaseImageOffset, not part of object definition

			// string table
			SawyerStreamWriter.WriteStringTableStream(ms, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(ms, obj.GraphicsElements);
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
