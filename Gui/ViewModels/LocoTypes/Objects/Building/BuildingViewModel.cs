using Dat.Loaders;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels.LocoTypes.Objects.Building;

public class BuildingViewModel(BuildingObject model)
	: LocoObjectViewModel<BuildingObject>(model)
{
	[EnumProhibitValues<BuildingObjectFlags>(BuildingObjectFlags.None)]
	public BuildingObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	[Description("Bitset")]
	public uint32_t Colours
	{
		get => Model.Colours;
		set => Model.Colours = value;
	}

	public Colour ScaffoldingColour
	{
		get => Model.ScaffoldingColour;
		set => Model.ScaffoldingColour = value;
	}

	public uint8_t ScaffoldingSegmentType
	{
		get => Model.ScaffoldingSegmentType;
		set => Model.ScaffoldingSegmentType = value;
	}

	public uint8_t GeneratorFunction
	{
		get => Model.GeneratorFunction;
		set => Model.GeneratorFunction = value;
	}

	public uint8_t AverageNumberOnMap
	{
		get => Model.AverageNumberOnMap;
		set => Model.AverageNumberOnMap = value;
	}

	[Category("Stats")]
	public int16_t DemolishRatingReduction
	{
		get => Model.DemolishRatingReduction;
		set => Model.DemolishRatingReduction = value;
	}

	[Category("Stats")]
	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	[Category("Stats")]
	public uint16_t ObsoleteYear
	{
		get => Model.ObsoleteYear;
		set => Model.ObsoleteYear = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public uint16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeader> ProducedCargo { get; set; } = new(model.ProducedCargo);
	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeader> RequiredCargo { get; set; } = new(model.RequiredCargo);
	[Category("Production"), Length(1, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<uint8_t> ProducedQuantity { get; set; } = new(model.ProducedQuantity);

	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingVariationCount)]
	public BindingList<BindingList<uint8_t>> BuildingVariations { get; init; } = new(model.BuildingComponents.BuildingVariations.Select(x => x.ToBindingList()).ToBindingList());

	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingHeightCount)]
	public BindingList<uint8_t> BuildingHeights { get; set; } = model.BuildingComponents.BuildingHeights.ToBindingList();

	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingAnimationCount)]
	public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } = model.BuildingComponents.BuildingAnimations.ToBindingList();

	// note: these height sequences are massive. BLDCTY28 has 2 sequences, 512 in length and 1024 in length. Avalonia PropertyGrid takes 30+ seconds to render this. todo: don't use property grid in future
	//[Reactive, Category("Building"), Length(1, BuildingObject.MaxElevatorHeightSequences), Browsable(false)] public BindingList<BindingList<uint8_t>> ElevatorHeightSequences { get; set; } // NumElevatorSequences

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence1 { get; init; } = model.ElevatorHeightSequences.Count > 0 ? new(model.ElevatorHeightSequences[0]) : null;

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence2 { get; init; } = model.ElevatorHeightSequences.Count > 1 ? new(model.ElevatorHeightSequences[1]) : null;

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence3 { get; init; } = model.ElevatorHeightSequences.Count > 2 ? new(model.ElevatorHeightSequences[2]) : null;

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence4 { get; init; } = model.ElevatorHeightSequences.Count > 3 ? new(model.ElevatorHeightSequences[3]) : null;

	[Category("<unknown>")]
	public uint8_t var_A6
	{
		get => Model.var_A6;
		set => Model.var_A6 = value;
	}

	[Category("<unknown>")]
	public uint8_t var_A7
	{
		get => Model.var_A7;
		set => Model.var_A7 = value;
	}

	[Category("<unknown>")]
	public uint8_t var_A8
	{
		get => Model.var_A8;
		set => Model.var_A8 = value;
	}

	[Category("<unknown>")]
	public uint8_t var_A9
	{
		get => Model.var_A9;
		set => Model.var_A9 = value;
	}

	[Category("<unknown>")]
	public uint8_t var_AC
	{
		get => Model.var_AC;
		set => Model.var_AC = value;
	}
}
