using Dat.Loaders;
using Definitions.ObjectModels.Objects.TrackSignal;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class TrackSignalViewModel(TrackSignalObject model)
	: LocoObjectViewModel<TrackSignalObject>(model)
{
	[EnumProhibitValues<TrackSignalObjectFlags>(TrackSignalObjectFlags.None)]
	public TrackSignalObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public uint8_t AnimationSpeed
	{
		get => Model.AnimationSpeed;
		set => Model.AnimationSpeed = value;
	}

	public uint8_t NumFrames
	{
		get => Model.NumFrames;
		set => Model.NumFrames = value;
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

	[Length(0, TrackSignalObjectLoader.Constants.ModsLength)]
	public BindingList<ObjectModelHeader> CompatibleTrackObjects { get; init; } = new(model.CompatibleTrackObjects);
}
