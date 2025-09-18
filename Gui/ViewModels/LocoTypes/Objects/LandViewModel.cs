using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Types;

namespace Gui.ViewModels;

public class LandViewModel(LandObject model)
	: LocoObjectViewModel<LandObject>(model)
{
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	public uint8_t NumGrowthStages
	{
		get => Model.NumGrowthStages;
		set => Model.NumGrowthStages = value;
	}

	public uint8_t NumImageAngles
	{
		get => Model.NumImageAngles;
		set => Model.NumImageAngles = value;
	}

	public LandObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public int16_t CostFactor
	{
		get => Model.CostFactor;
		set => Model.CostFactor = value;
	}

	public uint32_t NumImagesPerGrowthStage
	{
		get => Model.NumImagesPerGrowthStage;
		set => Model.NumImagesPerGrowthStage = value;
	}

	public uint8_t DistributionPattern
	{
		get => Model.DistributionPattern;
		set => Model.DistributionPattern = value;
	}

	public uint8_t NumVariations
	{
		get => Model.NumVariations;
		set => Model.NumVariations = value;
	}

	public uint8_t VariationLikelihood
	{
		get => Model.VariationLikelihood;
		set => Model.VariationLikelihood = value;
	}

	public ObjectModelHeader CliffEdgeHeader
	{
		get => Model.CliffEdgeHeader;
		set => Model.CliffEdgeHeader = value;
	}

	public ObjectModelHeader? UnkObjectHeader
	{
		get => Model.UnkObjectHeader;
		set => Model.UnkObjectHeader = value;
	}
}
