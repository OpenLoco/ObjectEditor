using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Tree;

public class TreeObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t Clearance { get; set; }
	public uint8_t Height { get; set; }
	public uint8_t var_04 { get; set; }
	public uint8_t var_05 { get; set; }
	public uint8_t NumRotations { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public TreeObjectFlags Flags { get; set; }
	public uint16_t ShadowImageOffset { get; set; }
	public uint8_t SeasonState { get; set; }
	public uint8_t Season { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t ClearCostFactor { get; set; }
	public uint32_t Colours { get; set; }
	public int16_t Rating { get; set; }
	public int16_t DemolishRatingReduction { get; set; }
	public TreeFlagsUnk var_3C { get; set; } // something with images

	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		// 230/256 = ~90%
		if (-ClearCostFactor > BuildCostFactor * 230 / 256)
		{
			return false;
		}

		switch (NumRotations)
		{
			default:
				return false;
			case 1:
			case 2:
			case 4:
				break;
		}

		if (NumGrowthStages is < 1 or > 8)
		{
			return false;
		}

		if (Height < Clearance)
		{
			return false;
		}

		return var_05 >= var_04;
	}

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
		=> throw new NotImplementedException();
}
