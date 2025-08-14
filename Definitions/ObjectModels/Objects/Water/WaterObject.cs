namespace Definitions.ObjectModels.Objects.Water;
public class WaterObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t CostIndex { get; set; }
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
		{ 42, "kToolbarTerraformWater" },
	};
}
