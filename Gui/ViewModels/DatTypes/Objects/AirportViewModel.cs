using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OpenLoco.Gui.ViewModels
{
	public class AirportViewModel : ReactiveObject, IObjectViewModel<ILocoStruct>
	{
		[Reactive] public uint16_t AllowedPlaneTypes { get; set; }
		[Reactive] public int8_t MinX { get; set; }
		[Reactive] public int8_t MinY { get; set; }
		[Reactive] public int8_t MaxX { get; set; }
		[Reactive] public int8_t MaxY { get; set; }
		[Reactive] public uint16_t DesignedYear { get; set; }
		[Reactive] public uint16_t ObsoleteYear { get; set; }
		[Reactive] public uint32_t LargeTiles { get; set; }
		[Reactive, Category("Cost")] public uint8_t CostIndex { get; set; }
		[Reactive, Category("Cost")] public int16_t BuildCostFactor { get; set; }
		[Reactive, Category("Cost")] public int16_t SellCostFactor { get; set; }
		[Reactive, Category("Building")] public BindingList<uint8_t> BuildingVariationHeights { get; set; }
		[Reactive, Category("Building")] public BindingList<BuildingPartAnimation> BuildingVariationAnimations { get; set; }
		[Reactive, Category("Building"), Length(AirportObject.VariationPartCount, AirportObject.VariationPartCount)] public BindingList<uint8_t[]> BuildingVariationParts { get; set; }
		[Reactive, Category("Building")] public BindingList<AirportBuilding> BuildingPositions { get; set; }
		[Reactive, Category("Movement")] public BindingList<MovementNode> MovementNodes { get; set; }
		[Reactive, Category("Movement")] public BindingList<MovementEdge> MovementEdges { get; set; }
		[Reactive, Category("<unknown>")] public uint8_t var_07 { get; set; }
		[Reactive, Category("<unknown>"), Length(4, 4)] public BindingList<uint8_t> pad_B6 { get; set; }

		public AirportViewModel(AirportObject ao)
		{
			CostIndex = ao.CostIndex;
			BuildCostFactor = ao.BuildCostFactor;
			SellCostFactor = ao.SellCostFactor;
			var_07 = ao.var_07;
			AllowedPlaneTypes = ao.AllowedPlaneTypes;
			BuildingVariationHeights = new(ao.BuildingVariationHeights);
			BuildingVariationAnimations = new(ao.BuildingVariationAnimations);
			BuildingVariationParts = new(ao.BuildingVariationParts);
			BuildingPositions = new(ao.BuildingPositions);
			LargeTiles = ao.LargeTiles;
			MinX = ao.MinX;
			MinY = ao.MinY;
			MaxX = ao.MaxX;
			MaxY = ao.MaxY;
			DesignedYear = ao.DesignedYear;
			ObsoleteYear = ao.ObsoleteYear;
			MovementNodes = new(ao.MovementNodes);
			MovementEdges = new(ao.MovementEdges);
			pad_B6 = new(ao.pad_B6);
		}

		public ILocoStruct GetAsUnderlyingType(ILocoStruct locoStruct)
			=> GetAsStruct((locoStruct as AirportObject)!);

		public AirportObject GetAsStruct(AirportObject ao)
		{
			// validation:
			// BuildingVariationHeights.Count MUST equal BuildingVariationAnimations.Count

			return ao with
			{
				BuildCostFactor = ao.BuildCostFactor,
				SellCostFactor = ao.SellCostFactor,
				CostIndex = ao.CostIndex,
				var_07 = ao.var_07,
				AllowedPlaneTypes = ao.AllowedPlaneTypes,
				LargeTiles = ao.LargeTiles,
				MinX = ao.MinX,
				MinY = ao.MinY,
				MaxX = ao.MaxX,
				MaxY = ao.MaxY,
				DesignedYear = ao.DesignedYear,
				ObsoleteYear = ao.ObsoleteYear,
				NumBuildingAnimations = (uint8_t)ao.BuildingVariationAnimations.Count,
				NumBuildingVariations = (uint8_t)ao.BuildingVariationParts.Count,
				NumMovementEdges = (uint8_t)ao.MovementEdges.Count,
				NumMovementNodes = (uint8_t)ao.MovementNodes.Count,
			};
		}
	}
}
