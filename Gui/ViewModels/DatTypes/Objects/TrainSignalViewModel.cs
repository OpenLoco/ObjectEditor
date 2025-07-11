using Dat.Objects;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels
{
	public class TrainSignalViewModel : LocoObjectViewModel<TrackSignalObject>
	{
		[Reactive, EnumProhibitValues<TrackSignalObjectFlags>(TrackSignalObjectFlags.None)] public TrackSignalObjectFlags Flags { get; set; }
		[Reactive] public uint8_t AnimationSpeed { get; set; }
		[Reactive] public uint8_t NumFrames { get; set; }
		[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
		[Reactive, Category("Stats")] public uint16_t DesignedYear { get; set; }
		[Reactive, Category("Stats")] public uint16_t ObsoleteYear { get; set; }
		[Reactive, Length(0, TrackSignalObject.ModsLength)] public BindingList<S5HeaderViewModel> CompatibleTrackObjects { get; set; }

		public TrainSignalViewModel(TrackSignalObject ro)
		{
			Flags = ro.Flags;
			AnimationSpeed = ro.AnimationSpeed;
			NumFrames = ro.NumFrames;
			CostIndex = ro.CostIndex;
			BuildCostFactor = ro.BuildCostFactor;
			SellCostFactor = ro.SellCostFactor;
			DesignedYear = ro.DesignedYear;
			ObsoleteYear = ro.ObsoleteYear;
			CompatibleTrackObjects = new(ro.CompatibleTrackObjects.ConvertAll(x => new S5HeaderViewModel(x)));
		}

		public override TrackSignalObject GetAsStruct(TrackSignalObject tso)
			=> tso with
			{
				Flags = Flags,
				AnimationSpeed = AnimationSpeed,
				NumFrames = NumFrames,
				CostIndex = CostIndex,
				BuildCostFactor = BuildCostFactor,
				SellCostFactor = SellCostFactor,
				DesignedYear = DesignedYear,
				ObsoleteYear = ObsoleteYear,
				CompatibleTrackObjects = CompatibleTrackObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				CompatibleTrackObjectCount = (uint8_t)CompatibleTrackObjects.Count,
			};
	}
}
