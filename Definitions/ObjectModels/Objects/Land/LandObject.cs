using Definitions.ObjectModels.Objects.Cliff;
using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Land;

public class LandObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t CostIndex { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public uint8_t NumImageAngles { get; set; }
	public LandObjectFlags Flags { get; set; }
	public int16_t CostFactor { get; set; }
	public uint32_t NumImagesPerGrowthStage { get; set; }
	public uint8_t DistributionPattern { get; set; }
	public uint8_t NumVariations { get; set; }
	public uint8_t VariationLikelihood { get; set; }

	public ObjectModelHeader CliffEdgeHeader { get; set; }
	public ObjectModelHeader? UnkObjectHeader { get; set; }

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

		if (NumGrowthStages < 1)
		{
			return false;
		}

		if (NumGrowthStages > 8)
		{
			return false;
		}

		return NumImageAngles is 1 or 2 or 4;
	}

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static readonly Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "kFlatSE" },
		{ 1, "toolbar_terraform_land" },
	};
}
