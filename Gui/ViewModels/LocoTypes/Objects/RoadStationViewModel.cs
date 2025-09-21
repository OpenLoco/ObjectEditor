using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Objects.Shared;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

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
	public ObjectModelHeader? CargoType { get; set; }

	public CargoOffset[][][] CargoOffsets { get; init; }

	public RoadStationViewModel(RoadStationObject model)
		: base(model)
	{
		PaintStyle = model.PaintStyle;
		Height = model.Height;
		RoadPieces = model.RoadPieces;
		DesignedYear = model.DesignedYear;
		ObsoleteYear = model.ObsoleteYear;
		BuildCostFactor = model.BuildCostFactor;
		SellCostFactor = model.SellCostFactor;
		CostIndex = model.CostIndex;
		Flags = model.Flags;
		CargoType = model.CargoType;
		CargoOffsets = model.CargoOffsets;
		CompatibleRoadObjects = [.. model.CompatibleRoadObjects.ConvertAll(x => new ObjectModelHeaderViewModel(x))];
	}

	public RoadStationObject CopyBackToModel()
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
			//CompatibleRoadObjects = CompatibleRoadObjects.ToList().ConvertAll(x => x.CopyBackToModel()),
		};
}
