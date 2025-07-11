using Dat.Objects;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class AirportViewModel : LocoObjectViewModel<AirportObject>
{
	[Reactive] public uint16_t AllowedPlaneTypes { get; set; }
	[Reactive] public int8_t MinX { get; set; }
	[Reactive] public int8_t MinY { get; set; }
	[Reactive] public int8_t MaxX { get; set; }
	[Reactive] public int8_t MaxY { get; set; }
	[Reactive] public uint16_t DesignedYear { get; set; }
	[Reactive] public uint16_t ObsoleteYear { get; set; }
	[Reactive] public uint32_t LargeTiles { get; set; }
	[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
	[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Reactive, Category("Building"), Length(1, AirportObject.BuildingVariationCount)] public BindingList<BindingList<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Reactive, Category("Building"), Length(1, AirportObject.BuildingHeightCount)] public BindingList<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Reactive, Category("Building"), Length(1, AirportObject.BuildingAnimationCount)] public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts
	[Reactive, Category("Building")] public BindingList<AirportBuilding> BuildingPositions { get; set; }
	[Reactive, Category("Movement")] public BindingList<MovementNode> MovementNodes { get; set; } // NumMovementNodes
	[Reactive, Category("Movement")] public BindingList<MovementEdge> MovementEdges { get; set; } // NumMovementEdges
	[Reactive, Category("<unknown>")] public uint8_t var_07 { get; set; }
	[Reactive, Category("<unknown>"), Length(4, 4)] public BindingList<uint8_t> var_B6 { get; set; }

	public AirportViewModel(AirportObject ao)
	{
		CostIndex = ao.CostIndex;
		BuildCostFactor = ao.BuildCostFactor;
		SellCostFactor = ao.SellCostFactor;
		var_07 = ao.var_07;
		AllowedPlaneTypes = ao.AllowedPlaneTypes;
		BuildingHeights = new(ao.BuildingHeights);
		BuildingAnimations = new(ao.BuildingAnimations);
		BuildingVariations = new(ao.BuildingVariations.Select(x => new BindingList<uint8_t>(x)).ToBindingList());
		BuildingPositions = new(ao.BuildingPositions);
		LargeTiles = ao.LargeTiles;
		MinX = ao.MinX;
		MinY = ao.MinY;
		MaxX = ao.MaxX;
		MaxY = ao.MaxY;
		DesignedYear = ao.DesignedYear;
		ObsoleteYear = ao.ObsoleteYear;
		MovementNodes = new(ao.MovementNodes);
		MovementEdges = new(ao.MovementEdges);
		var_B6 = [.. ao.var_B6];
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public override AirportObject GetAsStruct(AirportObject ao)
		=> ao with
		{
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			CostIndex = CostIndex,
			var_07 = var_07,
			AllowedPlaneTypes = AllowedPlaneTypes,
			LargeTiles = LargeTiles,
			MinX = MinX,
			MinY = MinY,
			MaxX = MaxX,
			MaxY = MaxY,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			NumBuildingParts = (uint8_t)BuildingAnimations.Count,
			NumBuildingVariations = (uint8_t)BuildingVariations.Count,
			NumMovementEdges = (uint8_t)MovementEdges.Count,
			NumMovementNodes = (uint8_t)MovementNodes.Count,
			var_B6 = [.. var_B6],
		};
}
