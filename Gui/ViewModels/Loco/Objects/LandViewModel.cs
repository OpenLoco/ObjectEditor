using Definitions.ObjectModels.Objects.Land;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class LandViewModel(LandObject model)
	: BaseViewModel<LandObject>(model)
{
	CliffEdgeReferenceViewModel cliffEdgeHeader = model.CliffEdgeHeader is { } cliffEdgeHeaderModel ? new(cliffEdgeHeaderModel) : new();
	LandReferenceViewModel? replacementLandHeader = model.ReplacementLandHeader is { } replacementLandHeaderModel ? new(replacementLandHeaderModel) : null;

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

	[EnumProhibitValues<LandObjectFlags>(LandObjectFlags.None)]
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

	public CliffEdgeReferenceViewModel CliffEdgeHeader
	{
		get => cliffEdgeHeader;
		set
		{
			cliffEdgeHeader = value;
			if (value != null)
			{
				Model.CliffEdgeHeader = value.Model;
			}
		}
	}

	public LandReferenceViewModel? ReplacementLandHeader
	{
		get => replacementLandHeader;
		set
		{
			replacementLandHeader = value;
			Model.ReplacementLandHeader = value?.Model;
		}
	}
}
