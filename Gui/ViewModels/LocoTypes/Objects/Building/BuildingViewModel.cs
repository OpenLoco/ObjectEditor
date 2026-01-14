using Dat.Loaders;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels.LocoTypes.Objects.Building;

public class BuildingViewModel : LocoObjectViewModel<BuildingObject>
{
	public BuildingViewModel(BuildingObject model) : base(model)
	{
		ProducedCargo = new(model.ProducedCargo);
		RequiredCargo = new(model.RequiredCargo);
		ProducedQuantity = new(model.ProducedQuantity);

		BuildingVariations = new(model.BuildingComponents.BuildingVariations.Select(x => x.ToBindingList()).ToBindingList());
		BuildingHeights = model.BuildingComponents.BuildingHeights.ToBindingList();
		BuildingAnimations = model.BuildingComponents.BuildingAnimations.ToBindingList();

		ElevatorSequence1 = model.ElevatorHeightSequences.Count > 0 ? new(model.ElevatorHeightSequences[0]) : null;
		ElevatorSequence2 = model.ElevatorHeightSequences.Count > 1 ? new(model.ElevatorHeightSequences[1]) : null;
		ElevatorSequence3 = model.ElevatorHeightSequences.Count > 2 ? new(model.ElevatorHeightSequences[2]) : null;
		ElevatorSequence4 = model.ElevatorHeightSequences.Count > 3 ? new(model.ElevatorHeightSequences[3]) : null;

		#region Building Components Synchronization

		// Subscribe to BuildingVariations changes (including nested lists)
		BuildingVariations.ListChanged += OnBuildingVariationsChanged;
		foreach (var variation in BuildingVariations)
		{
			variation.ListChanged += OnBuildingVariationItemChanged;
		}

		// Subscribe to BuildingHeights changes
		BuildingHeights.ListChanged += OnBuildingHeightsChanged;

		// Subscribe to BuildingAnimations changes
		BuildingAnimations.ListChanged += OnBuildingAnimationsChanged;

		#endregion
	}

	void OnBuildingVariationsChanged(object? sender, ListChangedEventArgs e)
	{
		// Only handle when the outer list itself changes, not when inner list items change
		if (e.ListChangedType == ListChangedType.ItemAdded && e.NewIndex >= 0 && e.NewIndex < BuildingVariations.Count)
		{
			BuildingVariations[e.NewIndex].ListChanged += OnBuildingVariationItemChanged;
			SynchronizeBuildingComponentsToModel();
			NotifyBuildingChanged();
		}
		else if (e.ListChangedType == ListChangedType.ItemDeleted)
		{
			SynchronizeBuildingComponentsToModel();
			NotifyBuildingChanged();
		}
		else if (e.ListChangedType == ListChangedType.Reset)
		{
			// Re-subscribe to all items after reset
			foreach (var variation in BuildingVariations)
			{
				variation.ListChanged += OnBuildingVariationItemChanged;
			}
			SynchronizeBuildingComponentsToModel();
			NotifyBuildingChanged();
		}
		// Ignore ItemChanged events as they're handled by OnBuildingVariationItemChanged
	}

	void OnBuildingVariationItemChanged(object? sender, ListChangedEventArgs e)
	{
		SynchronizeBuildingComponentsToModel();
		NotifyBuildingChanged();
	}

	void OnBuildingHeightsChanged(object? sender, ListChangedEventArgs e)
	{
		SynchronizeBuildingComponentsToModel();
		NotifyBuildingChanged();
	}

	void OnBuildingAnimationsChanged(object? sender, ListChangedEventArgs e)
	{
		SynchronizeBuildingComponentsToModel();
		NotifyBuildingChanged();
	}

	void SynchronizeBuildingComponentsToModel()
	{
		Model.BuildingComponents.BuildingVariations = [.. BuildingVariations.Select(x => x.ToList())];
		Model.BuildingComponents.BuildingHeights = [.. BuildingHeights];
		Model.BuildingComponents.BuildingAnimations = [.. BuildingAnimations];
	}

	void NotifyBuildingChanged()
	{
		MessageBus.Current.SendMessage(new BuildingComponents()
		{
			BuildingAnimations = [.. BuildingAnimations],
			BuildingHeights = [.. BuildingHeights],
			BuildingVariations = [.. BuildingVariations.Select(x => x.ToList())]
		});
	}

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

	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeader> ProducedCargo { get; set; }
	[Category("Production"), Length(0, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<ObjectModelHeader> RequiredCargo { get; set; }
	[Category("Production"), Length(1, BuildingObjectLoader.Constants.MaxProducedCargoType)] public BindingList<uint8_t> ProducedQuantity { get; set; }

	//[Category("Building")]
	//public BuildingComponents BuildingComponents
	//{
	//	get => Model.BuildingComponents;
	//	set => Model.BuildingComponents = value;
	//}

	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingVariationCount)]
	public BindingList<BindingList<uint8_t>> BuildingVariations { get; init; }

	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingHeightCount)]
	public BindingList<uint8_t> BuildingHeights { get; init; }

	[Category("Building"), Length(1, BuildingObjectLoader.Constants.BuildingAnimationCount)]
	public BindingList<BuildingPartAnimation> BuildingAnimations { get; init; }

	// note: these height sequences are massive. BLDCTY28 has 2 sequences, 512 in length and 1024 in length. Avalonia PropertyGrid takes 30+ seconds to render this. todo: don't use property grid in future
	//[Reactive, Category("Building"), Length(1, BuildingObject.MaxElevatorHeightSequences), Browsable(false)] public BindingList<BindingList<uint8_t>> ElevatorHeightSequences { get; set; } // NumElevatorSequences

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence1 { get; init; }

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence2 { get; init; }

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence3 { get; init; }

	[Category("Elevator"), Browsable(false)]
	public BindingList<uint8_t>? ElevatorSequence4 { get; init; }

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
