using OpenLoco.Dat.Objects;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System;
using System.ComponentModel;
using System.Linq;

namespace OpenLoco.Gui.ViewModels
{
	[Flags]
	public enum SupportPillarSpacing : uint8_t
	{
		Tile0A = 1 << 0,
		Tile0B = 1 << 1,
		Tile1A = 1 << 2,
		Tile1B = 1 << 3,
		Tile2A = 1 << 4,
		Tile2B = 1 << 5,
		Tile3A = 1 << 6,
		Tile4B = 1 << 7,
	}

	public class BridgeViewModel : LocoObjectViewModel<BridgeObject>
	{
		[Reactive, EnumProhibitValues<BridgeObjectFlags>(BridgeObjectFlags.None)] public BridgeObjectFlags Flags { get; set; }
		[Reactive] public uint16_t ClearHeight { get; set; }
		[Reactive] public int16_t DeckDepth { get; set; }
		[Reactive] public uint8_t SpanLength { get; set; }
		[Reactive] public SupportPillarSpacing PillarPlacement { get; set; }
		[Reactive] public Speed16 MaxSpeed { get; set; }
		[Reactive] public MicroZ MaxHeight { get; set; }
		[Reactive, EnumProhibitValues<BridgeDisabledTrackFlags>(BridgeDisabledTrackFlags.None)] public BridgeDisabledTrackFlags DisabledTrackFlags { get; set; }
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
			Flags = bo.Flags;
			SpanLength = bo.SpanLength;
			PillarPlacement = (SupportPillarSpacing)bo.PillarSpacing;
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
				Flags = Flags,
				SpanLength = SpanLength,
				PillarSpacing = (uint8_t)PillarPlacement,
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
