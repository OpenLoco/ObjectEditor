using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Types;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels;

public class RoadViewModel(RoadObject model)
	: BaseViewModel<RoadObject>(model)
{
	[EnumProhibitValues<RoadObjectFlags>(RoadObjectFlags.None)]
	public RoadObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	[EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)]
	public RoadTraitFlags RoadPieces
	{
		get => Model.RoadPieces;
		set => Model.RoadPieces = value;
	}

	public Speed16 MaxSpeed
	{
		get => Model.MaxCurveSpeed;
		set => Model.MaxCurveSpeed = value;
	}

	public uint8_t PaintStyle
	{
		get => Model.PaintStyle;
		set => Model.PaintStyle = value;
	}

	[Description("The vertical offset in pixels used to display a vehicle in a vehicle list (eg build menu, station train list, etc)")]
	public uint8_t VehicleDisplayListVerticalOffset
	{
		get => Model.VehicleDisplayListVerticalOffset;
		set => Model.VehicleDisplayListVerticalOffset = value;
	}

	public TownSize TargetTownSize
	{
		get => Model.TargetTownSize;
		set => Model.TargetTownSize = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set
		{
			Model.CostIndex = value;
			this.RaisePropertyChanged(nameof(BuildCostFactor));
			this.RaisePropertyChanged(nameof(SellCostFactor));
			this.RaisePropertyChanged(nameof(TunnelCostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(nameof(CostIndex))]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(nameof(CostIndex))]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(nameof(CostIndex))]
	public int16_t TunnelCostFactor
	{
		get => Model.TunnelCostFactor;
		set => Model.TunnelCostFactor = value;
	}

	[Category("Compatible Objects")]
	public ObjectModelHeader Tunnel
	{
		get => Model.Tunnel;
		set => Model.Tunnel = value;
	}

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Bridges { get; set; } = new(model.Bridges);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Stations { get; set; } = new(model.Stations);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> Mods { get; set; } = new(model.RoadMods);

	[Category("Compatible Objects")]
	public BindingList<ObjectModelHeader> TracksAndRoads { get; set; } = new(model.TracksAndRoads);
}
