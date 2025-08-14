using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class ScaffoldingObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[LocoStructSize(0x12)]
[LocoStructType(DatObjectType.Scaffolding)]
internal record ScaffoldingObject(
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
