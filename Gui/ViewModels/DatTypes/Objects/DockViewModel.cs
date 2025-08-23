using Dat.Loaders;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class DockViewModel : LocoObjectViewModel<DockObject>
{
	[Reactive, EnumProhibitValues<DockObjectFlags>(DockObjectFlags.None)] public DockObjectFlags Flags { get; set; }
	[Reactive] public uint16_t DesignedYear { get; set; }
	[Reactive] public uint16_t ObsoleteYear { get; set; }
	[Reactive] public Pos2 BoatPosition { get; set; }
	[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
	[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
	[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
	[Reactive, Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingVariationCount)] public BindingList<BindingList<uint8_t>> BuildingVariations { get; set; } // NumBuildingVariations
	[Reactive, Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingHeightCount)] public BindingList<uint8_t> BuildingHeights { get; set; } // NumBuildingParts
	[Reactive, Category("Building"), Length(1, AirportObjectLoader.Constants.BuildingAnimationCount)] public BindingList<BuildingPartAnimation> BuildingAnimations { get; set; } // NumBuildingParts
	[Reactive, Category("<unknown>")] public uint8_t var_07 { get; set; }

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

	public override DockObject GetAsStruct()
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
			//BuildingVariations = [.. BuildingVariations],
		};
		return dockObject;
	}
}
