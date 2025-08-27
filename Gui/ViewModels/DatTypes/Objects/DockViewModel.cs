using Dat.Loaders;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class DockViewModel : LocoObjectViewModel<DockObject>
{
	[EnumProhibitValues<DockObjectFlags>(DockObjectFlags.None)] public DockObjectFlags Flags { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public Pos2 BoatPosition { get; set; }
	[Category("Cost")] public uint8_t CostIndex { get; set; }
	[Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Category("Building"), Length(1, DockObjectLoader.Constants.BuildingVariationCount)] public BindingList<BindingList<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Category("Building"), Length(1, DockObjectLoader.Constants.BuildingHeightCount)] public BindingList<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Category("Building"), Length(1, DockObjectLoader.Constants.BuildingAnimationCount)] public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts
	[Category("<unknown>")] public uint8_t var_07 { get; set; }

	public DockViewModel(DockObject @do)
	{
		Flags = @do.Flags;
		DesignedYear = @do.DesignedYear;
		ObsoleteYear = @do.ObsoleteYear;
		CostIndex = @do.CostIndex;
		BuildCostFactor = @do.BuildCostFactor;
		SellCostFactor = @do.SellCostFactor;
		BoatPosition = @do.BoatPosition;
		var_07 = @do.var_07;
		BuildingHeights = new(@do.BuildingHeights);
		BuildingAnimations = new(@do.BuildingAnimations);
		BuildingVariations = new(@do.BuildingVariations.Select(x => new BindingList<uint8_t>(x)).ToBindingList());
	}

	public override DockObject GetAsModel()
	{
		var dockObject = new DockObject()
		{
			Flags = Flags,
			DesignedYear = DesignedYear,
			ObsoleteYear = ObsoleteYear,
			CostIndex = CostIndex,
			BuildCostFactor = BuildCostFactor,
			SellCostFactor = SellCostFactor,
			BoatPosition = BoatPosition,
			var_07 = var_07,
			BuildingHeights = [.. BuildingHeights],
			BuildingAnimations = [.. BuildingAnimations],
			BuildingVariations = BuildingVariations.ToList().ConvertAll(x => x.ToList()),
		};
		return dockObject;
	}
}
