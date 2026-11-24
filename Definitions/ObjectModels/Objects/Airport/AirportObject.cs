using Definitions.ObjectModels.Objects.Common;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Airport;

public class AirportObject : ILocoStruct, IHasBuildingComponents
{
	[Range(1, int16_t.MaxValue, ErrorMessage = "BuildCostFactor must be greater than 0")]
	public int16_t BuildCostFactor { get; set; }

	[Range(int16_t.MinValue, -1, ErrorMessage = "SellCostFactor must be less than 0")]
	public int16_t SellCostFactor { get; set; }

	[Range(0, Constants.CurrencyMultiplicationFactorArraySize - 1, ErrorMessage = "CostIndex must be less than {1}")]
	public uint8_t CostIndex { get; set; }

	public AirportObjectFlags Flags { get; set; }
	public BuildingComponents BuildingComponents { get; set; } = new();
	public List<AirportBuilding> BuildingPositions { get; set; } = [];
	public uint32_t LargeTiles { get; set; }
	public int8_t MinX { get; set; }
	public int8_t MinY { get; set; }
	public int8_t MaxX { get; set; }
	public int8_t MaxY { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public List<MovementNode> MovementNodes { get; set; } = [];
	public List<MovementEdge> MovementEdges { get; set; } = [];
	public uint32_t RequiredClearEdges { get; set; } // bitfield of edges that must be clear for the airport to be built

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// not applicable for AirportObject
		//var bcValidationContext = new ValidationContext(BuildingComponents);
		//foreach (var result in BuildingComponents.Validate(bcValidationContext))
		//{
		//	yield return result;
		//}

		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (SellCostFactor >= 0)
		{
			yield return new ValidationResult($"{nameof(SellCostFactor)} must be less than 0 {nameof(SellCostFactor)}", [nameof(SellCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be greater than 0", [nameof(BuildCostFactor)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"-{nameof(SellCostFactor)} must be less than or equal to {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		// Movement graph integrity. The node and edge tables are index-based and are read directly by
		// Locomotion, so a stale index (a node/edge that a designer deleted or renumbered) can crash
		// the game when the airport is used in a scenario.
		var nodeCount = MovementNodes.Count;
		var edgeCount = MovementEdges.Count;
		var referencedNodes = new bool[nodeCount];

		for (var i = 0; i < edgeCount; i++)
		{
			var edge = MovementEdges[i];

			if (edge.CurrNode < nodeCount)
			{
				referencedNodes[edge.CurrNode] = true;
			}
			else
			{
				yield return new ValidationResult(
					$"{nameof(MovementEdges)}[{i}].{nameof(MovementEdge.CurrNode)} ({edge.CurrNode}) is not a valid index into {nameof(MovementNodes)} ({nodeCount} entries).",
					[nameof(MovementEdges)]);
			}

			if (edge.NextNode < nodeCount)
			{
				referencedNodes[edge.NextNode] = true;
			}
			else
			{
				yield return new ValidationResult(
					$"{nameof(MovementEdges)}[{i}].{nameof(MovementEdge.NextNode)} ({edge.NextNode}) is not a valid index into {nameof(MovementNodes)} ({nodeCount} entries).",
					[nameof(MovementEdges)]);
			}

			if (ReferencesMissingEdge(edge.MustBeClearEdges, edgeCount))
			{
				yield return new ValidationResult(
					$"{nameof(MovementEdges)}[{i}].{nameof(MovementEdge.MustBeClearEdges)} references an edge index that does not exist (there are {edgeCount} edges).",
					[nameof(MovementEdges)]);
			}

			if (ReferencesMissingEdge(edge.AtLeastOneClearEdges, edgeCount))
			{
				yield return new ValidationResult(
					$"{nameof(MovementEdges)}[{i}].{nameof(MovementEdge.AtLeastOneClearEdges)} references an edge index that does not exist (there are {edgeCount} edges).",
					[nameof(MovementEdges)]);
			}
		}

		for (var n = 0; n < nodeCount; n++)
		{
			if (!referencedNodes[n])
			{
				yield return new ValidationResult(
					$"{nameof(MovementNodes)}[{n}] is not referenced by any {nameof(MovementEdges)} entry (orphaned node).",
					[nameof(MovementNodes)]);
			}
		}

		if (ReferencesMissingEdge(RequiredClearEdges, edgeCount))
		{
			yield return new ValidationResult(
				$"{nameof(RequiredClearEdges)} references an edge index that does not exist (there are {edgeCount} edges).",
				[nameof(RequiredClearEdges)]);
		}
	}

	// Bit i of a clear-edges bitfield refers to movement edge i, so any bit at or above the edge count
	// names an edge that does not exist. Shift counts of 32+ are undefined for a uint, so guard them.
	private static bool ReferencesMissingEdge(uint32_t clearEdges, int edgeCount)
		=> edgeCount < 32 && (clearEdges >> edgeCount) != 0;
}
