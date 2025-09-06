namespace Definitions.ObjectModels.Objects.HillShape;

public class HillShapesObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t HillHeightMapCount { get; set; }
	public uint8_t MountainHeightMapCount { get; set; }
	public bool IsHeightMap { get; set; }

	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "hill shape 1" },
		{ 1, "hill shape 2" },
		{ 2, "mountain shape 1" },
		{ 3, "mountain shape 2" },
		{ 4, "preview image" },

	};
}
