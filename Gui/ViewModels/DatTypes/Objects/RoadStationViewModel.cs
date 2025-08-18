using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class RoadStationViewModel : LocoObjectViewModel<RoadStationObject>
{
	[Reactive] public uint8_t PaintStyle { get; set; }
	[Reactive] public uint8_t Height { get; set; }
	[Reactive, EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)] public RoadTraitFlags RoadPieces { get; set; }
	[Reactive] public uint16_t DesignedYear { get; set; }
	[Reactive] public uint16_t ObsoleteYear { get; set; }
	[Reactive, EnumProhibitValues<RoadStationObjectFlags>(RoadStationObjectFlags.None)] public RoadStationObjectFlags Flags { get; set; }
	[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
	[Reactive, Category("Compatible")] public BindingList<ObjectModelHeaderViewModel> CompatibleRoadObjects { get; set; }

	public RoadStationViewModel(RoadStationObject ro)
	{
		PaintStyle = ro.PaintStyle;
		Height = ro.Height;
		RoadPieces = ro.RoadPieces;
		DesignedYear = ro.DesignedYear;
		ObsoleteYear = ro.ObsoleteYear;
		BuildCostFactor = ro.BuildCostFactor;
		SellCostFactor = ro.SellCostFactor;
		CostIndex = ro.CostIndex;
		Flags = ro.Flags;
		//CompatibleRoadObjects = new(ro.CompatibleRoadObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
	}

	public override RoadStationObject GetAsStruct()
		=> new()
		{
			PaintStyle = PaintStyle,
			Height = Height,
			RoadPieces = RoadPieces,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			CostIndex = CostIndex,
			Flags = Flags,
			//CompatibleRoadObjectCount = (uint8_t)CompatibleRoadObjects.Count,
			CompatibleRoadObjects = CompatibleRoadObjects.ToList().ConvertAll(x => x.GetAsUnderlyingType()),
		};
}
