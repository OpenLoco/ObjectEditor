using Dat.Loaders;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class BuildingViewModel : LocoObjectViewModel<BuildingObject>
{
	[EnumProhibitValues<BuildingObjectFlags>(BuildingObjectFlags.None)] public BuildingObjectFlags Flags { get; set; }
	public uint32_t Colours { get; set; } // bitset
	public Colour ScaffoldingColour { get; set; }
	public uint8_t ScaffoldingSegmentType { get; set; }
	public uint8_t GeneratorFunction { get; set; }
	public uint8_t AverageNumberOnMap { get; set; }
	[Category("Stats")] public int16_t DemolishRatingReduction { get; set; }
	[Category("Stats")] public uint16_t DesignedYear { get; set; }
	[Category("Stats")] public uint16_t ObsoleteYear { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public uint16_t SellCostFactor { get; set; }
	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeaderViewModel> ProducedCargo { get; set; }
	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeaderViewModel> RequiredCargo { get; set; }
	[Category("Production"), Length(1, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<uint8_t> ProducedQuantity { get; set; }
	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingVariationCount)] public BindingList<BindingList<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingHeightCount)] public BindingList<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingAnimationCount)] public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts

	// note: these height sequences are massive. BLDCTY28 has 2 sequences, 512 in length and 1024 in length. Avalonia PropertyGrid takes 30+ seconds to render this. todo: don't use property grid in future
	//[Reactive, Category("Building"), Length(1, BuildingObject.MaxElevatorHeightSequences), Browsable(false)] public BindingList<BindingList<uint8_t>> ElevatorHeightSequences { get; set; } // NumElevatorSequences
	public uint8_t[]? ElevatorSequence1 { get; set; }
	public uint8_t[]? ElevatorSequence2 { get; set; }
	public uint8_t[]? ElevatorSequence3 { get; set; }
	public uint8_t[]? ElevatorSequence4 { get; set; }

	[Reactive, Category("<unknown>")] public uint8_t var_A6 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_A7 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_A8 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_A9 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_AC { get; set; }

	public BuildingViewModel(BuildingObject bo)
	{
		Flags = bo.Flags;
		Colours = bo.Colours;
		ScaffoldingColour = bo.ScaffoldingColour;
		ScaffoldingSegmentType = bo.ScaffoldingSegmentType;
		GeneratorFunction = bo.GeneratorFunction;
		AverageNumberOnMap = bo.AverageNumberOnMap;
		DemolishRatingReduction = bo.DemolishRatingReduction;
		DesignedYear = bo.DesignedYear;
		ObsoleteYear = bo.ObsoleteYear;
		CostIndex = bo.CostIndex;
		SellCostFactor = bo.SellCostFactor;
		ProducedCargo = new(bo.ProducedCargo.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		RequiredCargo = new(bo.RequiredCargo.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		ProducedQuantity = [.. bo.ProducedQuantity];
		BuildingHeights = new(bo.BuildingHeights);
		BuildingAnimations = new(bo.BuildingAnimations);
		BuildingVariations = new(bo.BuildingVariations.Select(x => new BindingList<uint8_t>(x)).ToBindingList());
		ElevatorSequence1 = bo.ElevatorHeightSequences.Count > 0 ? bo.ElevatorHeightSequences[0] : null;
		ElevatorSequence2 = bo.ElevatorHeightSequences.Count > 1 ? bo.ElevatorHeightSequences[1] : null;
		ElevatorSequence3 = bo.ElevatorHeightSequences.Count > 2 ? bo.ElevatorHeightSequences[2] : null;
		ElevatorSequence4 = bo.ElevatorHeightSequences.Count > 3 ? bo.ElevatorHeightSequences[3] : null;
		//ElevatorHeightSequences = new(bo.ElevatorHeightSequences.Select(x => new BindingList<uint8_t>(x)).ToBindingList());
		var_A6 = bo.var_A6;
		var_A7 = bo.var_A7;
		var_A8 = bo.var_A8;
		var_A9 = bo.var_A9;
		var_AC = bo.var_AC;
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public override BuildingObject GetAsModel()
		=> new BuildingObject()
		{
			BuildingAnimations = [.. BuildingAnimations],
			BuildingHeights = [.. BuildingHeights],
			BuildingVariations = BuildingVariations.ToList().ConvertAll(x => x.ToList()),
			Flags = Flags,
			Colours = Colours,
			ScaffoldingColour = ScaffoldingColour,
			ScaffoldingSegmentType = ScaffoldingSegmentType,
			GeneratorFunction = GeneratorFunction,
			AverageNumberOnMap = AverageNumberOnMap,
			DemolishRatingReduction = DemolishRatingReduction,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			CostIndex = CostIndex,
			SellCostFactor = SellCostFactor,
			var_AC = var_AC,
			ProducedQuantity = [.. ProducedQuantity],
			ProducedCargo = ProducedCargo.ToList().ConvertAll(x => x.GetAsModel()),
			RequiredCargo = RequiredCargo.ToList().ConvertAll(x => x.GetAsModel()),
			ElevatorHeightSequences = GetElevatorSequences(),
			var_A6 = var_A6,
			var_A7 = var_A7,
			var_A8 = var_A8,
			var_A9 = var_A9,

		};

	List<uint8_t[]> GetElevatorSequences()
	{
		List<uint8_t[]> result = [];
		if (ElevatorSequence1 != null)
		{
			result.Add(ElevatorSequence1);
		}
		if (ElevatorSequence2 != null)
		{
			result.Add(ElevatorSequence2);
		}
		if (ElevatorSequence3 != null)
		{
			result.Add(ElevatorSequence3);
		}
		if (ElevatorSequence4 != null)
		{
			result.Add(ElevatorSequence4);
		}
		return result;
	}
}
