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
	TunnelReferenceViewModel? tunnel = model.Tunnel is { } tunnelModel ? new(tunnelModel) : null;

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

	public Speed16 MaxCurveSpeed
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
			RaisePropertiesChanged(nameof(BuildCostFactor), nameof(SellCostFactor), nameof(TunnelCostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(10, nameof(CostIndex))]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(10, nameof(CostIndex))]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(10, nameof(CostIndex))]
	public int16_t TunnelCostFactor
	{
		get => Model.TunnelCostFactor;
		set => Model.TunnelCostFactor = value;
	}

	[Category("Compatible Objects")]
	public TunnelReferenceViewModel? Tunnel
	{
		get => tunnel;
		set
		{
			tunnel = value;
			if (value != null)
			{
				Model.Tunnel = value.Model;
			}
		}
	}

	[Category("Compatible Objects")]
	public ObjectReferenceList<BridgeReferenceViewModel> Bridges { get; set; } = new(model.Bridges, h => new BridgeReferenceViewModel(h), list => model.Bridges = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<RoadStationReferenceViewModel> Stations { get; set; } = new(model.Stations, h => new RoadStationReferenceViewModel(h), list => model.Stations = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<RoadExtraReferenceViewModel> Mods { get; set; } = new(model.RoadMods, h => new RoadExtraReferenceViewModel(h), list => model.RoadMods = [.. list]);

	[Category("Compatible Objects")]
	public ObjectReferenceList<TrackOrRoadReferenceViewModel> TracksAndRoads { get; set; } = new(model.TracksAndRoads, h => new TrackOrRoadReferenceViewModel(h), list => model.TracksAndRoads = [.. list]);

	public override void SynchronizeToModel()
	{
		Bridges.Synchronize();
		Stations.Synchronize();
		Mods.Synchronize();
		TracksAndRoads.Synchronize();
	}
}
