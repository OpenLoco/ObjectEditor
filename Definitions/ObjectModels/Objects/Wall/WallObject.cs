namespace Definitions.ObjectModels.Objects.Wall;

public class WallObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t Height { get; set; }
	public uint8_t ToolId { get; set; } // unused in loco???
	public WallObjectFlags1 Flags1 { get; set; } = WallObjectFlags1.None;
	public WallObjectFlags2 Flags2 { get; set; } = WallObjectFlags2.None; // unused in loco???

	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "kFlatSE" },
		{ 1, "kFlatNE" },
		{ 2, "SlopedSE" },
		{ 3, "SlopedNE" },
		{ 4, "SlopedNW" },
		{ 5, "SlopedSW" },
		{ 6, "kGlassFlatSE" },
		{ 7, "kGlassFlatNE" },
		{ 8, "kGlassSlopedSE" },
		{ 9, "kGlassSlopedNE" },
		{ 10, "kGlassSlopedNW" },
		{ 11, "kGlassSlopedSW" },
	};
}
