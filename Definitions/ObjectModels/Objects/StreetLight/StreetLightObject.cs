namespace Definitions.ObjectModels.Objects.Streetlight;
public class StreetLightObject : ILocoStruct, IImageTableNameProvider
{
	public List<uint16_t> DesignedYears { get; set; } = [];

	public bool Validate()
		=> true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "style0NE" },
		{ 1, "style0SE" },
		{ 2, "style0SW" },
		{ 3, "style0NW" },
		{ 4, "style1NE" },
		{ 5, "style1SE" },
		{ 6, "style1SW" },
		{ 7, "style1NW" },
		{ 8, "style2NE" },
		{ 9, "style2SE" },
		{ 10, "style2SW" },
		{ 11, "style2NW" },
	};
}
