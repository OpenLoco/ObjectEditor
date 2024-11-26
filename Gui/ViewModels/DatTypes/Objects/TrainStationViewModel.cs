using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class TrainStationViewModel : ReactiveObject, IObjectViewModel<ILocoStruct>
	{
		[Reactive] public uint8_t DrawStyle { get; set; }
		[Reactive] public uint8_t Height { get; set; }
		[Reactive] public TrackTraitFlags TrackPieces { get; set; }
		[Reactive] public uint16_t DesignedYear { get; set; }
		[Reactive] public uint16_t ObsoleteYear { get; set; }
		[Reactive] public int16_t BuildCostFactor { get; set; }
		[Reactive] public int16_t SellCostFactor { get; set; }
		[Reactive] public uint8_t CostIndex { get; set; }
		[Reactive] public TrainStationObjectFlags Flags { get; set; }
		[Reactive] public BindingList<uint32_t> ImageOffsets { get; set; }
		[Reactive] public BindingList<uint8_t> Mods { get; set; }
		[Reactive] public uint8_t var_0B { get; set; }
		[Reactive] public uint8_t var_0D { get; set; }
		[Reactive] public BindingList<S5Header> Compatible { get; set; }

		//public uint8_t[][][] CargoOffsetBytes { get; set; }

		//public uint8_t[][] ManualPower { get; set; }

		public TrainStationViewModel(TrainStationObject ts)
		{
			DrawStyle = ts.DrawStyle;
			Height = ts.Height;
			TrackPieces = ts.TrackPieces;
			DesignedYear = ts.DesignedYear;
			ObsoleteYear = ts.ObsoleteYear;
			BuildCostFactor = ts.BuildCostFactor;
			SellCostFactor = ts.SellCostFactor;
			CostIndex = ts.CostIndex;
			Flags = ts.Flags;
			ImageOffsets = new(ts.ImageOffsets);
			Mods = new(ts.Mods);
			var_0B = ts.var_0B;
			var_0D = ts.var_0D;
			Compatible = new(ts.Compatible);
			Mods = new(ts.Mods);
		}

		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
			=> GetAsStruct((locoStruct as TrainStationObject)!);

		// validation:
		// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
		public TrainStationObject GetAsStruct(TrainStationObject ts)
			=> ts with
			{
				DrawStyle = DrawStyle,
				Height = Height,
				TrackPieces = TrackPieces,
				DesignedYear = DesignedYear,
				ObsoleteYear = ObsoleteYear,
				BuildCostFactor = BuildCostFactor,
				SellCostFactor = SellCostFactor,
				CostIndex = CostIndex,
				Flags = Flags,
				var_0B = var_0B,
				var_0D = var_0D,
				Mods = [.. Mods],
				NumCompatible = (uint8_t)Compatible.Count(),
			};
	}
}
