using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.Generic;
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
	[Category("Compatible")] public List<ObjectModelHeaderViewModel> CompatibleRoadObjects { get; set; }
	[Category("Cargo")] public ObjectModelHeader? CargoType { get; set; }

	[Category("Cargo")] public CargoOffset[][][] CargoOffsets { get; init; }

	public RoadStationViewModel(RoadStationObject rso)
	{
		PaintStyle = rso.PaintStyle;
		Height = rso.Height;
		RoadPieces = rso.RoadPieces;
		DesignedYear = rso.DesignedYear;
		ObsoleteYear = rso.ObsoleteYear;
		BuildCostFactor = rso.BuildCostFactor;
		SellCostFactor = rso.SellCostFactor;
		CostIndex = rso.CostIndex;
		Flags = rso.Flags;
		CargoType = rso.CargoType;
		CargoOffsets = rso.CargoOffsets;
		CompatibleRoadObjects = [.. rso.CompatibleRoadObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
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
			CargoType = CargoType,
			CargoOffsets = CargoOffsets,
			CompatibleRoadObjects = CompatibleRoadObjects.ToList().ConvertAll(x => x.GetAsModel()),
		};
}
