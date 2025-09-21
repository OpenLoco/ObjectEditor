using Dat.Loaders;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Common;
using PropertyModels.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class AirportViewModel : LocoObjectViewModel<AirportObject>
{
	public uint16_t AllowedPlaneTypes { get; set; }
	public int8_t MinX { get; set; }
	public int8_t MinY { get; set; }
	public int8_t MaxX { get; set; }
	public int8_t MaxY { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public uint32_t LargeTiles { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingVariationCount)] public ObservableCollection<ObservableCollection<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingHeightCount)] public ObservableCollection<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingAnimationCount)] public ObservableCollection<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts
	[Category("Building")] public ObservableCollection<AirportBuilding> BuildingPositions { get; set; }
	[Category("Movement")] public ObservableCollection<MovementNode> MovementNodes { get; set; } // NumMovementNodes
	[Category("Movement")] public ObservableCollection<MovementEdge> MovementEdges { get; set; } // NumMovementEdges
	[Category("<unknown>")] public uint8_t var_07 { get; set; }
	[Category("<unknown>"), Length(4, 4)] public ObservableCollection<uint8_t> var_B6 { get; set; }

	public AirportViewModel(AirportObject model)
		: base(model)
	{
		AllowedPlaneTypes = model.AllowedPlaneTypes;
		MinX = model.MinX;
		MinY = model.MinY;
		MaxX = model.MaxX;
		MaxY = model.MaxY;
		DesignedYear = model.DesignedYear;
		ObsoleteYear = model.ObsoleteYear;
		LargeTiles = model.LargeTiles;
		CostIndex = model.CostIndex;
		BuildCostFactor = model.BuildCostFactor;
		SellCostFactor = model.SellCostFactor;
		BuildingHeights = new(model.BuildingComponents.BuildingHeights);
		BuildingAnimations = new(model.BuildingComponents.BuildingAnimations);
		BuildingVariations = new(model.BuildingComponents.BuildingVariations.Select(x => new ObservableCollection<uint8_t>(x)));
		BuildingPositions = new(model.BuildingPositions);
		MovementNodes = new(model.MovementNodes);
		MovementEdges = new(model.MovementEdges);
		var_07 = model.var_07;
		var_B6 = [.. model.var_B6];
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public AirportObject CopyBackToModel()
		=> new()
		{
			AllowedPlaneTypes = AllowedPlaneTypes,
			MinX = MinX,
			MinY = MinY,
			MaxX = MaxX,
			MaxY = MaxY,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			LargeTiles = LargeTiles,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			BuildingComponents = new BuildingComponentsModel()
			{
				BuildingHeights = [.. BuildingHeights],
				BuildingAnimations = [.. BuildingAnimations],
				BuildingVariations = [.. BuildingVariations.Select(x => x.ToList())],
			},
			BuildingPositions = [.. BuildingPositions],
			MovementNodes = [.. MovementNodes],
			MovementEdges = [.. MovementEdges],
			var_07 = var_07,
			var_B6 = [.. var_B6],
		};
}
