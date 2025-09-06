namespace Definitions.ObjectModels.Objects.Tunnel;

public class TunnelObject : ILocoStruct, IImageTableNameProvider
{
	public bool Validate() => true;

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static readonly Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "south west back" },
		{ 1, "south west front" },
		{ 2, "south east back" },
		{ 3, "south east front" },
	};
}
