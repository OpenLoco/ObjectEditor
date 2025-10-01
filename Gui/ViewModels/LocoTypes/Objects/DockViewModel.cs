using Dat.Loaders;
using Definitions.ObjectModels.Objects.Common;
using Definitions.ObjectModels.Objects.Dock;
using Definitions.ObjectModels.Types;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Gui.ViewModels;

public class DockViewModel(DockObject model)
	: LocoObjectViewModel<DockObject>(model)
{
	[EnumProhibitValues<DockObjectFlags>(DockObjectFlags.None)]
	public DockObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public uint16_t DesignedYear
	{
		get => Model.DesignedYear;
		set => Model.DesignedYear = value;
	}

	public uint16_t ObsoleteYear
	{
		get => Model.ObsoleteYear;
		set => Model.ObsoleteYear = value;
	}

	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public Pos2 BoatPosition
	{
		get => Model.BoatPosition;
		set => Model.BoatPosition = value;
	}

	[Category("Cost")]
	public uint8_t CostIndex
	{
		get => Model.CostIndex;
		set => Model.CostIndex = value;
	}

	[Category("Cost")]
	public int16_t BuildCostFactor
	{
		get => Model.BuildCostFactor;
		set => Model.BuildCostFactor = value;
	}

	[Category("Cost")]
	public int16_t SellCostFactor
	{
		get => Model.SellCostFactor;
		set => Model.SellCostFactor = value;
	}

	[Category("Building")]
	[Length(1, DockObjectLoader.Constants.BuildingVariationCount)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<BindingList<uint8_t>> BuildingVariations { get; init; } = new(model.BuildingComponents.BuildingVariations.Select(x => x.ToBindingList()).ToBindingList());

	[Category("Building")]
	[Length(1, DockObjectLoader.Constants.BuildingHeightCount)]
	public BindingList<uint8_t> BuildingHeights { get; init; } = model.BuildingComponents.BuildingHeights.ToBindingList();

	[Category("Building")]
	[Length(1, DockObjectLoader.Constants.BuildingAnimationCount)]
	[ExpandableObjectDisplayMode(IsCategoryVisible = NullableBooleanType.No)]
	public BindingList<BuildingPartAnimation> BuildingAnimations { get; init; } = model.BuildingComponents.BuildingAnimations.ToBindingList();

	[Category("<unknown>")]
	public uint8_t var_07
	{
		get => Model.var_07;
		set => Model.var_07 = value;
	}
}
