using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Scaffolding;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class ScaffoldingObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int SegmentHeightCount = 3;
		public const int RoofHeightCount = 3;
	}

	public static class StructSizes
	{
		public const int Dat = 0x12;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new ScaffoldingObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image, not part of object definition

			for (var i = 0; i < Constants.SegmentHeightCount; i++)
			{
				model.SegmentHeights.Add(br.ReadUInt16());
			}

			for (var i = 0; i < Constants.RoofHeightCount; i++)
			{
				model.RoofHeights.Add(br.ReadUInt16());
			}

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Scaffolding), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Scaffolding, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as ScaffoldingObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyImageId();

			for (var i = 0; i < Constants.SegmentHeightCount; i++)
			{
				bw.Write(model.SegmentHeights[i]);
			}

			for (var i = 0; i < Constants.RoofHeightCount; i++)
			{
				bw.Write(model.RoofHeights[i]);
			}

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.GraphicsElements);
		}
	}
}

[LocoStructSize(0x12)]
[LocoStructType(DatObjectType.Scaffolding)]
internal record DatScaffoldingObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x06), LocoArrayLength(3)] uint16_t[] SegmentHeights,
	[property: LocoStructOffset(0x0C), LocoArrayLength(3)] uint16_t[] RoofHeights
	) : ILocoStruct, IImageTableNameProvider
{
	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "type01x1SegmentBack" },
		{ 1, "type01x1SegmentFront" },
		{ 2, "type01x1RoofNE" },
		{ 3, "type01x1RoofSE" },
		{ 4, "type01x1RoofSW" },
		{ 5, "type01x1RoofNW" },
		{ 6, "type02x2SegmentBack" },
		{ 7, "type02x2SegmentFront" },
		{ 8, "type02x2RoofNE" },
		{ 9, "type02x2RoofSE" },
		{ 10, "type02x2RoofSW" },
		{ 11, "type02x2RoofNW" },
		{ 12, "type11x1SegmentBack" },
		{ 13, "type11x1SegmentFront" },
		{ 14, "type11x1RoofNE" },
		{ 15, "type11x1RoofSE" },
		{ 16, "type11x1RoofSW" },
		{ 17, "type11x1RoofNW" },
		{ 18, "type12x2SegmentBack" },
		{ 19, "type12x2SegmentFront" },
		{ 20, "type12x2RoofNE" },
		{ 21, "type12x2RoofSE" },
		{ 22, "type12x2RoofSW" },
		{ 23, "type12x2RoofNW" },
		{ 24, "type21x1SegmentBack" },
		{ 25, "type21x1SegmentFront" },
		{ 26, "type21x1RoofNE" },
		{ 27, "type21x1RoofSE" },
		{ 28, "type21x1RoofSW" },
		{ 29, "type21x1RoofNW" },
		{ 30, "type22x2SegmentBack" },
		{ 31, "type22x2SegmentFront" },
		{ 32, "type22x2RoofNE" },
		{ 33, "type22x2RoofSE" },
		{ 34, "type22x2RoofSW" },
		{ 35, "type22x2RoofNW" },
	};
}
