using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Gui.ViewModels;

public class TrackStationViewModel(TrackStationObject model)
	: LocoObjectViewModel<TrackStationObject>(model)
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

	[EnumProhibitValues<TrackTraitFlags>(TrackTraitFlags.None)]
	public TrackTraitFlags TrackPieces
	{
		get => Model.TrackPieces;
		set => Model.TrackPieces = value;
	}

	[EnumProhibitValues<TrackStationObjectFlags>(TrackStationObjectFlags.None)]
	public TrackStationObjectFlags Flags
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

	[Category("<unknown>")]
	public uint8_t var_0B
	{
		get => Model.var_0B;
		set => Model.var_0B = value;
	}

	[Category("<unknown>")]
	public uint8_t var_0D
	{
		get => Model.var_0D;
		set => Model.var_0D = value;
	}

	[Category("<unknown>")]
	public uint8_t[][] var_6E { get; set; } = model.var_6E;

	[Category("Cargo")]
	public CargoOffset[][][] CargoOffsets { get; init; } = model.CargoOffsets;

	[Category("Compatible")]
	public BindingList<ObjectModelHeader> CompatibleTrackObjects { get; init; } = new(model.CompatibleTrackObjects);
}
