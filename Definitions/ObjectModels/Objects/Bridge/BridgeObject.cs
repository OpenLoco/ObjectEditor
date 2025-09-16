using Definitions.ObjectModels.Types;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Bridge;

public class BridgeObject : ILocoStruct
{
	public BridgeObjectFlags Flags { get; set; }
	public uint16_t ClearHeight { get; set; }
	public int16_t DeckDepth { get; set; }
	public uint8_t SpanLength { get; set; }
	public SupportPillarSpacing PillarSpacing { get; set; }
	public Speed16 MaxSpeed { get; set; }
	public MicroZ MaxHeight { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BaseCostFactor { get; set; }
	public int16_t HeightCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint16_t DesignedYear { get; set; }
	public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
	public uint8_t var_03 { get; set; } // unknown

	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];
	public List<ObjectModelHeader> CompatibleRoadObjects { get; set; } = [];

	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		if (-SellCostFactor > BaseCostFactor)
		{
			return false;
		}

		if (BaseCostFactor <= 0)
		{
			return false;
		}

		if (HeightCostFactor < 0)
		{
			return false;
		}

		if (DeckDepth is not 16 and not 32)
		{
			return false;
		}

		if (SpanLength is not 1 and not 2 and not 4)
		{
			return false;
		}

		//if (CompatibleTrackObjectCount > 7)
		//{
		//	return false;
		//}

		//if (CompatibleRoadObjectCount > 7)
		//{
		//	return false;
		//}

		return true;
	}
}
