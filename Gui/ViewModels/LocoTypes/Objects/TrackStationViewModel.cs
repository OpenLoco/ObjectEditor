using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace Gui.ViewModels;

public class TrackStationViewModel : LocoObjectViewModel<TrackStationObject>
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	[EnumProhibitValues<TrackTraitFlags>(TrackTraitFlags.None)] public TrackTraitFlags TrackPieces { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	[EnumProhibitValues<TrackStationObjectFlags>(TrackStationObjectFlags.None)] public TrackStationObjectFlags Flags { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Compatible")] public List<ObjectModelHeaderViewModel> CompatibleTrackObjects { get; set; }
	[Category("<unknown>")] public uint8_t var_0B { get; set; }
	[Category("<unknown>")] public uint8_t var_0D { get; set; }

	[Browsable(false)]
	public uint8_t[][] var_6E { get; set; }

	public CargoOffset[][][] CargoOffsets { get; init; }

	public TrackStationViewModel(TrackStationObject tso)
	{
		PaintStyle = tso.PaintStyle;
		Height = tso.Height;
		TrackPieces = tso.TrackPieces;
		DesignedYear = tso.DesignedYear;
		ObsoleteYear = tso.ObsoleteYear;
		BuildCostFactor = tso.BuildCostFactor;
		SellCostFactor = tso.SellCostFactor;
		CostIndex = tso.CostIndex;
		Flags = tso.Flags;
		var_0B = tso.var_0B;
		var_0D = tso.var_0D;
		var_6E = tso.var_6E;
		//CargoOffsets = tso.CargoOffsets;
		CompatibleTrackObjects = [.. tso.CompatibleTrackObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
	}

	// validation:
	// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count
	public override TrackStationObject GetAsModel()
		=> new()
		{
			PaintStyle = PaintStyle,
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
			var_6E = var_6E,
			//CargoOffsets = CargoOffsets,
			CompatibleTrackObjects = CompatibleTrackObjects.ConvertAll(x => x.GetAsModel()),
		};
}
