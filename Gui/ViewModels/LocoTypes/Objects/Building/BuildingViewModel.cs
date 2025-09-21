using Dat.Loaders;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels.LocoTypes.Objects.Building;

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
	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public ObservableCollection<ObjectModelHeaderViewModel> ProducedCargo { get; set; }
	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public ObservableCollection<ObjectModelHeaderViewModel> RequiredCargo { get; set; }
	[Category("Production"), Length(1, BuildingObjectLoader.Constants.MaxProducedCargoType)] public ObservableCollection<uint8_t> ProducedQuantity { get; set; }
	[Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingVariationCount)] public ObservableCollection<ObservableCollection<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingHeightCount)] public ObservableCollection<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingAnimationCount)] public ObservableCollection<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts

	// note: these height sequences are massive. BLDCTY28 has 2 sequences, 512 in length and 1024 in length. Avalonia PropertyGrid takes 30+ seconds to render this. todo: don't use property grid in future
	//[Reactive, Category("Building"), Length(1, BuildingObject.MaxElevatorHeightSequences), Browsable(false)] public BindingList<BindingList<uint8_t>> ElevatorHeightSequences { get; set; } // NumElevatorSequences

	[Category("Elevator"), Browsable(false)] public uint8_t[]? ElevatorSequence1 { get; set; }
	[Category("Elevator"), Browsable(false)] public uint8_t[]? ElevatorSequence2 { get; set; }
	[Category("Elevator"), Browsable(false)] public uint8_t[]? ElevatorSequence3 { get; set; }
	[Category("Elevator"), Browsable(false)] public uint8_t[]? ElevatorSequence4 { get; set; }

	[Reactive, Category("<unknown>")] public uint8_t var_A6 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_A7 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_A8 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_A9 { get; set; }
	[Reactive, Category("<unknown>")] public uint8_t var_AC { get; set; }

	public BuildingViewModel(BuildingObject model)
		: base(model)
	{
		Flags = model.Flags;
		Colours = model.Colours;
		ScaffoldingColour = model.ScaffoldingColour;
		ScaffoldingSegmentType = model.ScaffoldingSegmentType;
		GeneratorFunction = model.GeneratorFunction;
		AverageNumberOnMap = model.AverageNumberOnMap;
		DemolishRatingReduction = model.DemolishRatingReduction;
		DesignedYear = model.DesignedYear;
		ObsoleteYear = model.ObsoleteYear;
		CostIndex = model.CostIndex;
		SellCostFactor = model.SellCostFactor;
		ProducedCargo = [.. model.ProducedCargo.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		RequiredCargo = [.. model.RequiredCargo.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
		ProducedQuantity = [.. model.ProducedQuantity];
		BuildingHeights = new(model.BuildingComponents.BuildingHeights);
		BuildingAnimations = new(model.BuildingComponents.BuildingAnimations);
		BuildingVariations = new(model.BuildingComponents.BuildingVariations.Select(x => new ObservableCollection<uint8_t>(x)));
		ElevatorSequence1 = model.ElevatorHeightSequences.Count > 0 ? model.ElevatorHeightSequences[0] : null;
		ElevatorSequence2 = model.ElevatorHeightSequences.Count > 1 ? model.ElevatorHeightSequences[1] : null;
		ElevatorSequence3 = model.ElevatorHeightSequences.Count > 2 ? model.ElevatorHeightSequences[2] : null;
		ElevatorSequence4 = model.ElevatorHeightSequences.Count > 3 ? model.ElevatorHeightSequences[3] : null;
		//ElevatorHeightSequences = new(bo.ElevatorHeightSequences.Select(x => new ObservableCollection<uint8_t>(x)).ToObservableCollection());
		var_A6 = model.var_A6;
		var_A7 = model.var_A7;
		var_A8 = model.var_A8;
		var_A9 = model.var_A9;
		var_AC = model.var_AC;
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	//public override BuildingObject CopyBackToModel()
	//	=> new()
	//	{
	//		BuildingComponents = new BuildingComponentsModel()
	//		{
	//			BuildingHeights = [.. BuildingHeights],
	//			BuildingAnimations = [.. BuildingAnimations],
	//			BuildingVariations = BuildingVariations.ToList().ConvertAll(x => x.ToList()),
	//		},
	//		Flags = Flags,
	//		Colours = Colours,
	//		ScaffoldingColour = ScaffoldingColour,
	//		ScaffoldingSegmentType = ScaffoldingSegmentType,
	//		GeneratorFunction = GeneratorFunction,
	//		AverageNumberOnMap = AverageNumberOnMap,
	//		DemolishRatingReduction = DemolishRatingReduction,
	//		DesignedYear = DesignedYear,
	//		ObsoleteYear = ObsoleteYear,
	//		CostIndex = CostIndex,
	//		SellCostFactor = SellCostFactor,
	//		var_AC = var_AC,
	//		ProducedQuantity = [.. ProducedQuantity],
	//		ProducedCargo = ProducedCargo.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		RequiredCargo = RequiredCargo.ToList().ConvertAll(x => x.CopyBackToModel()),
	//		ElevatorHeightSequences = GetElevatorSequences(),
	//		var_A6 = var_A6,
	//		var_A7 = var_A7,
	//		var_A8 = var_A8,
	//		var_A9 = var_A9,

	//	};

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
