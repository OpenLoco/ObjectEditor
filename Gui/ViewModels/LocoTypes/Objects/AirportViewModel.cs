using Dat.Loaders;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Common;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class AirportViewModel(AirportObject model)
	: LocoObjectViewModel<AirportObject>(model)
{
	public AirportObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public int8_t MinX
	{
		get => Model.MinX;
		set => Model.MinX = value;
	}

	public int8_t MinY
	{
		get => Model.MinY;
		set => Model.MinY = value;
	}

	public int8_t MaxX
	{
		get => Model.MaxX;
		set => Model.MaxX = value;
	}

	public int8_t MaxY
	{
		get => Model.MaxY;
		set => Model.MaxY = value;
	}

	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	public uint16_t ObsoleteYear
	{
		get => Model.ObsoleteYear;
		set => Model.ObsoleteYear = value;
	}

	public uint32_t LargeTiles
	{
		get => Model.LargeTiles;
		set => Model.LargeTiles = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost")]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Building")]
	[Length(1, AirportObjectLoader.Constants.BuildingVariationCount)]
	public BindingList<BindingList<uint8_t>> BuildingVariations { get; init; } = new(model.BuildingComponents.BuildingVariations.Select(x => x.ToBindingList()).ToBindingList());

	[Category("Building")]
	[Length(1, AirportObjectLoader.Constants.BuildingHeightCount)]
	public BindingList<uint8_t> BuildingHeights { get; set; } = model.BuildingComponents.BuildingHeights.ToBindingList();

	[Category("Building")]
	[Length(1, AirportObjectLoader.Constants.BuildingAnimationCount)]
	public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } = model.BuildingComponents.BuildingAnimations.ToBindingList();

	[Category("Building")]
	public BindingList<AirportBuilding> BuildingPositions { get; set; } = model.BuildingPositions.ToBindingList();

	[Category("Movement")]
	public BindingList<MovementNode> MovementNodes { get; set; } = model.MovementNodes.ToBindingList();

	[Category("Movement")]
	public BindingList<MovementEdge> MovementEdges { get; set; } = model.MovementEdges.ToBindingList();

	[Category("<unknown>")]
	public uint8_t var_07
	{
		get => Model.var_07;
		set => Model.var_07 = value;
	}

	[Category("<unknown>")]
	public uint32_t var_B6
	{
		get => Model.var_B6;
		set => Model.var_B6 = value;
	}
}
