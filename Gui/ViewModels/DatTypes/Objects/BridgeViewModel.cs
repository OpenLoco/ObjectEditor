using OpenLoco.Dat.Objects;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class BridgeViewModel : LocoObjectViewModel<BridgeObject>
	{
		[Reactive] public uint8_t NoRoof { get; set; }
		[Reactive] public uint16_t ClearHeight { get; set; }
		[Reactive] public int16_t DeckDepth { get; set; }
		[Reactive] public uint8_t SpanLength { get; set; }
		[Reactive] public uint8_t PillarSpacing { get; set; }
		[Reactive] public Speed16 MaxSpeed { get; set; }
		[Reactive] public MicroZ MaxHeight { get; set; }
		[Reactive] public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
		[Reactive] public uint16_t DesignedYear { get; set; }
		[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Cost")] public int16_t BaseCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t HeightCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
		[Reactive] public BindingList<S5HeaderViewModel> TrackCompatibleMods { get; set; }
		[Reactive] public BindingList<S5HeaderViewModel> RoadCompatibleMods { get; set; }
		[Reactive, Category("<unknown>")] public uint8_t var_03 { get; set; }

		public BridgeViewModel(BridgeObject bo)
		{
			NoRoof = bo.NoRoof;
			SpanLength = bo.SpanLength;
			PillarSpacing = bo.PillarSpacing;
			MaxSpeed = bo.MaxSpeed;
			MaxHeight = bo.MaxHeight;
			CostIndex = bo.CostIndex;
			BaseCostFactor = bo.BaseCostFactor;
			HeightCostFactor = bo.HeightCostFactor;
			SellCostFactor = bo.SellCostFactor;
			DisabledTrackFlags = bo.DisabledTrackFlags;
			DesignedYear = bo.DesignedYear;
			TrackCompatibleMods = new(bo.TrackCompatibleMods.ConvertAll(x => new S5HeaderViewModel(x)));
			RoadCompatibleMods = new(bo.RoadCompatibleMods.ConvertAll(x => new S5HeaderViewModel(x)));
			var_03 = bo.var_03;
			ClearHeight = bo.ClearHeight;
			DeckDepth = bo.DeckDepth;
		}

		public override BridgeObject GetAsStruct(BridgeObject bro)
			=> bro with
			{
				NoRoof = NoRoof,
				SpanLength = SpanLength,
				PillarSpacing = PillarSpacing,
				MaxSpeed = MaxSpeed,
				MaxHeight = MaxHeight,
				CostIndex = CostIndex,
				BaseCostFactor = BaseCostFactor,
				HeightCostFactor = HeightCostFactor,
				SellCostFactor = SellCostFactor,
				DisabledTrackFlags = DisabledTrackFlags,
				DesignedYear = DesignedYear,
				TrackCompatibleMods = TrackCompatibleMods.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				RoadCompatibleMods = RoadCompatibleMods.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
				NumCompatibleTrackMods = (uint8_t)TrackCompatibleMods.Count,
				NumCompatibleRoadMods = (uint8_t)RoadCompatibleMods.Count,
				var_03 = var_03,
				ClearHeight = ClearHeight,
				DeckDepth = DeckDepth,
			};
	}
}
