using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;

namespace Dat.Objects;

public static class DatRoadExtraObject
{
	public static RoadExtraObject Load(ReadOnlySpan<byte> data)
	{
		if (data.Length != ObjectAttributes.StructSize(ObjectType.RoadExtra))
		{
			throw new InvalidDataException($"RoadExtraObject data too short: {data.Length} bytes, expected at least 0x12 bytes.");
		}

		var model = new RoadExtraObject
		{
			RoadPieces = (RoadTraitFlags)data[0x02],
			PaintStyle = data[0x04],
			CostIndex = data[0x05],
			BuildCostFactor = ByteReaderT.Read_int16t(data, 0x06),
			SellCostFactor = ByteReaderT.Read_int16t(data, 0x08),
		};
		return model;
	}

	public static MemoryStream Save(MemoryStream ms, RoadExtraObject obj)
	{
		using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
		{
			bw.Write((string_id)0);// Name offset, not part of object definition
			bw.Write((uint16_t)obj.RoadPieces);
			bw.Write(obj.PaintStyle);
			bw.Write(obj.CostIndex);
			bw.Write(obj.BuildCostFactor);
			bw.Write(obj.SellCostFactor);
			bw.Write((image_id)0); // Image offset, not part of object definition
			bw.Write((image_id)0); // BaseImageOffset, not part of object definition
		}
		return ms;
	}
}
