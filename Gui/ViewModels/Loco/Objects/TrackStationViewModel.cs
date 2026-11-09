using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Types;
using Gui.Attributes;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels;

public class TrackStationViewModel(TrackStationObject model)
	: BaseViewModel<TrackStationObject>(model)
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
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set
		{
			Model.CostIndex = value;
			RaisePropertiesChanged(nameof(BuildCostFactor), nameof(SellCostFactor));
		}
	}

	[Category("Cost"), InflatableCurrency(8, nameof(CostIndex), nameof(DesignedYear))]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost"), InflatableCurrency(8, nameof(CostIndex), nameof(DesignedYear))]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Station")]
	public uint8_t PlatformType
	{
		get => Model.PlatformType;
		set => Model.PlatformType = value;
	}

	// pad_0D and DiagonalCargoOffsetBytes are part of the DAT layout but are unused by Locomotion,
	// so they are intentionally not surfaced for editing.

	[Category("Cargo")]
	public CargoOffset[][][] CargoOffsets { get; init; } = model.CargoOffsets;

	[Category("Compatible")]
	public ObjectReferenceList<TrackReferenceViewModel> CompatibleTrackObjects { get; init; } = new(model.CompatibleTrackObjects, h => new TrackReferenceViewModel(h), list => model.CompatibleTrackObjects = [.. list]);

	public override void SynchronizeToModel()
	{
		CompatibleTrackObjects.Synchronize();
	}
}
