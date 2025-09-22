using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class BridgeViewModel(BridgeObject model)
	: LocoObjectViewModel<BridgeObject>(model)
{
	[EnumProhibitValues<BridgeObjectFlags>(BridgeObjectFlags.None)]
	public BridgeObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public uint16_t ClearHeight
	{
		get => Model.ClearHeight;
		set => Model.ClearHeight = value;
	}

	public int16_t DeckDepth
	{
		get => Model.DeckDepth;
		set => Model.DeckDepth = value;
	}

	public uint8_t SpanLength
	{
		get => Model.SpanLength;
		set => Model.SpanLength = value;
	}

	public SupportPillarSpacing PillarSpacing
	{
		get => Model.PillarSpacing;
		set => Model.PillarSpacing = value;
	}

	public Speed16 MaxSpeed
	{
		get => Model.MaxSpeed;
		set => Model.MaxSpeed = value;
	}

	public MicroZ MaxHeight
	{
		get => Model.MaxHeight;
		set => Model.MaxHeight = value;
	}

	[EnumProhibitValues<BridgeDisabledTrackFlags>(BridgeDisabledTrackFlags.None)]
	public BridgeDisabledTrackFlags DisabledTrackFlags
	{
		get => Model.DisabledTrackFlags;
		set => Model.DisabledTrackFlags = value;
	}

	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public int16_t BaseCostFactor
	{
		get => Model.BaseCostFactor;
		set => Model.BaseCostFactor = value;
	}

	[Category("Cost")]
	public int16_t HeightCostFactor
	{
		get => Model.HeightCostFactor;
		set => Model.HeightCostFactor = value;
	}

	[Category("Cost")]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Compatible")] public BindingList<ObjectModelHeader> CompatibleTrackObjects { get; init; } = new(model.CompatibleTrackObjects);

	[Category("Compatible")] public BindingList<ObjectModelHeader> CompatibleRoadObjects { get; init; } = new(model.CompatibleRoadObjects);

	[Category("<unknown>")]
	public uint8_t var_03
	{
		get => Model.var_03;
		set => Model.var_03 = value;
	}
}
