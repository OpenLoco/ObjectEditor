using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class RoadStationViewModel : LocoObjectViewModel<RoadStationObject>
{
	public uint8_t PaintStyle { get; set; }
	public uint8_t Height { get; set; }
	[EnumProhibitValues<RoadTraitFlags>(RoadTraitFlags.None)] public RoadTraitFlags RoadPieces { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	[EnumProhibitValues<RoadStationObjectFlags>(RoadStationObjectFlags.None)] public RoadStationObjectFlags Flags { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Compatible")] public BindingList<ObjectModelHeaderViewModel> CompatibleRoadObjects { get; set; }
	public ObjectModelHeader? CargoType { get; set; }

	[Browsable(false)]
	public uint8_t[][][] CargoOffsetBytes { get; set; }

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
		CompatibleRoadObjects = new(ro.CompatibleRoadObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x)));
		CargoType = ro.CargoType;
		CargoOffsetBytes = ro.CargoOffsetBytes;
	}

	public override RoadStationObject GetAsModel()
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
			CompatibleRoadObjects = CompatibleRoadObjects.ToList().ConvertAll(x => x.GetAsModel()),
			CargoType = CargoType,
			CargoOffsetBytes = CargoOffsetBytes
		};
}
