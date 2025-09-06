namespace Definitions.ObjectModels.Objects.Scaffolding;
public class ScaffoldingObject : ILocoStruct
{
	public List<uint16_t> SegmentHeights { get; set; } = [];
	public List<uint16_t> RoofHeights { get; set; } = [];

	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "type0 1x1 SegmentBack" },
		{ 1, "type0 1x1 SegmentFront" },
		{ 2, "type0 1x1 RoofNE" },
		{ 3, "type0 1x1 RoofSE" },
		{ 4, "type0 1x1 RoofSW" },
		{ 5, "type0 1x1 RoofNW" },
		{ 6, "type0 2x2 SegmentBack" },
		{ 7, "type0 2x2 SegmentFront" },
		{ 8, "type0 2x2 RoofNE" },
		{ 9, "type0 2x2 RoofSE" },
		{ 10, "type0 2x2 RoofSW" },
		{ 11, "type0 2x2 RoofNW" },
		{ 12, "type1 1x1 SegmentBack" },
		{ 13, "type1 1x1 SegmentFront" },
		{ 14, "type1 1x1 RoofNE" },
		{ 15, "type1 1x1 RoofSE" },
		{ 16, "type1 1x1 RoofSW" },
		{ 17, "type1 1x1 RoofNW" },
		{ 18, "type1 2x2 SegmentBack" },
		{ 19, "type1 2x2 SegmentFront" },
		{ 20, "type1 2x2 RoofNE" },
		{ 21, "type1 2x2 RoofSE" },
		{ 22, "type1 2x2 RoofSW" },
		{ 23, "type1 2x2 RoofNW" },
		{ 24, "type2 1x1 SegmentBack" },
		{ 25, "type2 1x1 SegmentFront" },
		{ 26, "type2 1x1 RoofNE" },
		{ 27, "type2 1x1 RoofSE" },
		{ 28, "type2 1x1 RoofSW" },
		{ 29, "type2 1x1 RoofNW" },
		{ 30, "type2 2x2 SegmentBack" },
		{ 31, "type2 2x2 SegmentFront" },
		{ 32, "type2 2x2 RoofNE" },
		{ 33, "type2 2x2 RoofSE" },
		{ 34, "type2 2x2 RoofSW" },
		{ 35, "type2 2x2 RoofNW" },
	};
}
