using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

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

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static readonly Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "flat" },
		{ 1, "west corner up" },
		{ 2, "south corner up" },
		{ 3, "north east slope" },
		{ 4, "east corner up" },
		{ 5, "west and east corner up" },
		{ 6, "north west slope" },
		{ 7, "north corner down" },
		{ 8, "north corner up" },
		{ 9, "south east slope" },
		{ 10, "north and south corners up" },
		{ 11, "east corner down" },
		{ 12, "north west slope" },
		{ 13, "south corner down" },
		{ 14, "west corner down" },
		{ 15, "south slope" },
		{ 16, "north slope" },
		{ 17, "east slope" },
		{ 18, "west slope" }
	};
}
