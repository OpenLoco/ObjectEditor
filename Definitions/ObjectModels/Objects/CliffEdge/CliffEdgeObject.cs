namespace Definitions.ObjectModels.Objects.CliffEdge;
public class CliffEdgeObject : ILocoStruct, IImageTableNameProvider
{
	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
	{
		if (id is >= 0 and <= 63)
		{
			var direction = id / 16 % 2 == 0 ? "west" : "east";
			var side = id is >= 16 and <= 47 ? "right" : "left";
			var level = id % 16;
			value = $"south {direction} {side} {level}";
			return true;
		}

		if (id is >= 64 and <= 69)
		{
			value = ImageIdNameMap[id];
			return true;
		}

		value = null;
		return false;
	}

	public static readonly Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 64, "north west slope 1" },
		{ 65, "north west slope 2" },
		{ 66, "north west slope 3" },
		{ 67, "north east slope 1" },
		{ 68, "north east slope 2" },
		{ 69, "north east slope 3" },
	};
}
