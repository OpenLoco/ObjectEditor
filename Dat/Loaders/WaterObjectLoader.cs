using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Water;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;
public abstract class WaterObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x0E;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new WaterObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.CostIndex = br.ReadByte();
			model.var_03 = br.ReadByte();
			model.CostFactor = br.ReadInt16();
			br.SkipImageId(); // ImageId, not part of object definition
			br.SkipImageId(); // MapPixelImage, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Water), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Water, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (WaterObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.CostIndex);
			bw.Write(model.var_03);
			bw.Write(model.CostFactor);
			bw.WriteEmptyImageId(); // ImageId, not part of object definition
			bw.WriteEmptyImageId(); // MapPixelImage, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}

[LocoStructSize(0x0E)]
[LocoStructType(DatObjectType.Water)]
internal record DatWaterObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t CostIndex,
	[property: LocoStructOffset(0x03), LocoPropertyMaybeUnused] uint8_t var_03,
	[property: LocoStructOffset(0x04)] int16_t CostFactor,
	[property: LocoStructOffset(0x06), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x0A), Browsable(false)] image_id MapPixelImage
	)
{

}
