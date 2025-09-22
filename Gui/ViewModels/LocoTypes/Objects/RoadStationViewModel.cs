using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RoadStationViewModel(RoadStationObject model)
	: LocoObjectViewModel<RoadStationObject>(model)
{

	public uint8_t PaintStyle
	{
		get => Model.PaintStyle;
		set => Model.PaintStyle = value;
	}

	public uint8_t Height
	{
		get => Model.Height;
		set => Model.Height = value;
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

	[EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)]
	public RoadTraitFlags RoadPieces
	{
		get => Model.RoadPieces;
		set => Model.RoadPieces = value;
	}

	[EnumProhibitValues<RoadStationObjectFlags>(RoadStationObjectFlags.None)]
	public RoadStationObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
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

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cargo")]
	public ObjectModelHeader? CargoType
	{
		get => Model.CargoType;
		set => Model.CargoType = value;
	}

	[Category("Cargo")]
	public CargoOffset[][][] CargoOffsets { get; init; } = model.CargoOffsets;

	[Category("Compatible")]
	public BindingList<ObjectModelHeader> CompatibleRoadObjects { get; init; } = new(model.CompatibleRoadObjects);
}
