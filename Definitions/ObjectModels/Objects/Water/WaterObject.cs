using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Objects.Water;

public class WaterObject : ILocoStruct, IImageTableNameProvider
{
	[JsonInclude]
	public uint8_t CostIndex { get; set; }
	public uint8_t var_03 { get; set; }
	public int16_t CostFactor { get; set; }

	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		if (CostFactor <= 0)
		{
			return false;
		}

		return true;
	}

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "zoom1 wave overlay full" },
		{ 1, "zoom1 wave overlay west" },
		{ 2, "zoom1 wave overlay east" },
		{ 3, "zoom1 wave overlay north" },
		{ 4, "zoom1 wave overlay south" },
		{ 5, "zoom1 wave overlay full" },
		{ 6, "zoom1 wave half-tile west" },
		{ 7, "zoom1 wave half-tile east" },
		{ 8, "zoom1 wave half-tile north" },
		{ 9, "zoom1 wave half-tile south" },
		{ 10, "zoom2 wave overlay full" },
		{ 11, "zoom2 wave overlay west" },
		{ 12, "zoom2 wave overlay east" },
		{ 13, "zoom2 wave overlay north" },
		{ 14, "zoom2 wave overlay south" },
		{ 15, "zoom2 wave overlay full" },
		{ 16, "zoom2 wave half-tile west" },
		{ 17, "zoom2 wave half-tile east" },
		{ 18, "zoom2 wave half-tile north" },
		{ 19, "zoom2 wave half-tile south" },
		{ 20, "zoom3 wave overlay full" },
		{ 21, "zoom3 wave overlay west" },
		{ 22, "zoom3 wave overlay east" },
		{ 23, "zoom3 wave overlay north" },
		{ 24, "zoom3 wave overlay south" },
		{ 25, "zoom3 wave overlay full" },
		{ 26, "zoom3 wave half-tile west" },
		{ 27, "zoom3 wave half-tile east" },
		{ 28, "zoom3 wave half-tile north" },
		{ 29, "zoom3 wave half-tile south" },
		{ 30, "zoom4 wave overlay full" },
		{ 31, "zoom4 wave overlay west" },
		{ 32, "zoom4 wave overlay east" },
		{ 33, "zoom4 wave overlay north" },
		{ 34, "zoom4 wave overlay south" },
		{ 35, "zoom4 wave overlay full" },
		{ 36, "zoom4 wave half-tile west" },
		{ 37, "zoom4 wave half-tile east" },
		{ 38, "zoom4 wave half-tile north" },
		{ 39, "zoom4 wave half-tile south" },
		{ 40, "minimap palette" },
		{ 41, "water colour palette" },
		{ 42, "water icon animation 0" },
		{ 43, "water icon animation 1" },
		{ 44, "water icon animation 2" },
		{ 45, "water icon animation 3" },
		{ 46, "water icon animation 4" },
		{ 47, "water icon animation 5" },
		{ 48, "water icon animation 6" },
		{ 49, "water icon animation 7" },
		{ 50, "water icon animation 8" },
		{ 51, "water icon animation 9" },
		{ 52, "water icon animation 10" },
		{ 54, "water icon animation 11" },
		{ 55, "water icon animation 12" },
		{ 56, "water icon animation 13" },
		{ 57, "water icon animation 14" },
		{ 53, "water icon animation 15" },
		{ 58, "pick up water vehicle" },
		{ 59, "place down water vehicle" },
		{ 60, "water animation frame 0" },
		{ 61, "water animation frame 1" },
		{ 62, "water animation frame 2" },
		{ 63, "water animation frame 3" },
		{ 64, "water animation frame 4" },
		{ 65, "water animation frame 5" },
		{ 66, "water animation frame 6" },
		{ 67, "water animation frame 7" },
		{ 68, "water animation frame 8" },
		{ 69, "water animation frame 9" },
		{ 70, "water animation frame 10" },
		{ 71, "water animation frame 11" },
		{ 72, "water animation frame 12" },
		{ 73, "water animation frame 13" },
		{ 74, "water animation frame 14" },
		{ 75, "water animation frame 15" },
	};
}
