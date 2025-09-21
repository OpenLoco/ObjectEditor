using Dat.Loaders;
using Definitions.ObjectModels.Objects.TrackSignal;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gui.ViewModels;

public class TrackSignalViewModel : LocoObjectViewModel<TrackSignalObject>
{
	[EnumProhibitValues<TrackSignalObjectFlags>(TrackSignalObjectFlags.None)] public TrackSignalObjectFlags Flags { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t NumFrames { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Stats")] public uint16_t DesignedYear { get; set; }
	[Category("Stats")] public uint16_t ObsoleteYear { get; set; }
	[Length(0, TrackSignalObjectLoader.Constants.ModsLength)] public ObservableCollection<ObjectModelHeaderViewModel> CompatibleTrackObjects { get; set; }

	public TrackSignalViewModel(TrackSignalObject model)
		: base(model)
	{
		Flags = model.Flags;
		AnimationSpeed = model.AnimationSpeed;
		NumFrames = model.NumFrames;
		CostIndex = model.CostIndex;
		BuildCostFactor = model.BuildCostFactor;
		SellCostFactor = model.SellCostFactor;
		DesignedYear = model.DesignedYear;
		ObsoleteYear = model.ObsoleteYear;
		CompatibleTrackObjects = new(model.CompatibleTrackObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
	}

	public TrackSignalObject CopyBackToModel()
		=> new()
		{
			Flags = Flags,
			AnimationSpeed = AnimationSpeed,
			NumFrames = NumFrames,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			//CompatibleTrackObjects = [.. CompatibleTrackObjects.Select(x => x.CopyBackToModel())],
		};
}
