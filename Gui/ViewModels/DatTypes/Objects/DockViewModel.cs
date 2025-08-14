using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;

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
	[Reactive, Category("Building")] public BindingList<uint16_t> BuildingPartAnimations { get; set; }
	[Reactive, Category("Building")] public BindingList<uint8_t> BuildingVariationParts { get; set; }
	[Reactive, Category("Building")] public BindingList<uint8_t> BuildingPartHeights { get; set; }
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
		BuildingPartAnimations = new(@do.BuildingPartAnimations);
		BuildingVariationParts = new(@do.BuildingVariationParts);
		BuildingPartHeights = new(@do.BuildingPartHeights);
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
			BuildingPartAnimations = [.. BuildingPartAnimations],
			BuildingVariationParts = [.. BuildingVariationParts],
			BuildingPartHeights = [.. BuildingPartHeights]
		};
		return dockObject;
	}
}
