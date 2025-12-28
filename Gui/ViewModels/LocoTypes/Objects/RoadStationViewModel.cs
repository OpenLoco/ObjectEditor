using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;
using Gui.Attributes;
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
