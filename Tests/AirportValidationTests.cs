using Dat.FileParsing;
using Definitions.ObjectModels.Objects.Airport;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using Logger = Common.Logging.Logger;

namespace Dat.Tests;

[TestFixture]
public class AirportValidationTests
{
	static List<string> Validate(AirportObject airport)
		=> [.. airport.Validate(new ValidationContext(airport)).Select(x => x.ToString() ?? string.Empty)];

	static AirportObject ValidAirport()
		=> new()
		{
			BuildCostFactor = 100,
			SellCostFactor = -50,
			CostIndex = 0,
			MovementNodes = [new MovementNode(), new MovementNode()],
			MovementEdges =
			[
				new MovementEdge { CurrNode = 0, NextNode = 1 },
				new MovementEdge { CurrNode = 1, NextNode = 0 },
			],
		};

	[Test]
	public void ValidAirport_Passes()
		=> Assert.That(Validate(ValidAirport()), Is.Empty);

	[Test]
	public void OutOfRangeCurrNode_IsReported()
	{
		var airport = ValidAirport();
		airport.MovementEdges[0].CurrNode = 5;

		var errors = Validate(airport);

		Assert.That(errors, Has.Some.Contains("CurrNode"));
	}

	[Test]
	public void OutOfRangeNextNode_IsReported()
	{
		var airport = ValidAirport();
		airport.MovementEdges[1].NextNode = 7;

		var errors = Validate(airport);

		Assert.That(errors, Has.Some.Contains("NextNode"));
	}

	[Test]
	public void OrphanedNode_IsReported()
	{
		var airport = ValidAirport();
		airport.MovementNodes.Add(new MovementNode()); // a third node that no edge references

		var errors = Validate(airport);

		Assert.That(errors, Has.Some.Contains("orphaned node"));
	}

	[Test]
	public void MustBeClearEdgesReferencingMissingEdge_IsReported()
	{
		var airport = ValidAirport();
		airport.MovementEdges[0].MustBeClearEdges = 1u << 2; // bit 2 >= 2 edges

		var errors = Validate(airport);

		Assert.That(errors, Has.Some.Contains("MustBeClearEdges"));
	}

	[Test]
	public void AtLeastOneClearEdgesReferencingMissingEdge_IsReported()
	{
		var airport = ValidAirport();
		airport.MovementEdges[0].AtLeastOneClearEdges = 1u << 2;

		var errors = Validate(airport);

		Assert.That(errors, Has.Some.Contains("AtLeastOneClearEdges"));
	}

	[Test]
	public void RequiredClearEdgesReferencingMissingEdge_IsReported()
	{
		var airport = ValidAirport();
		airport.RequiredClearEdges = 1u << 2;

		var errors = Validate(airport);

		Assert.That(errors, Has.Some.Contains("RequiredClearEdges"));
	}

	[Test]
	public void InRangeEdgeBitfields_AreNotReported()
	{
		var airport = ValidAirport();
		airport.MovementEdges[0].MustBeClearEdges = 1u << 1; // bit 1 < 2 edges
		airport.MovementEdges[0].AtLeastOneClearEdges = 1u << 0;
		airport.RequiredClearEdges = 0b11;

		Assert.That(Validate(airport), Is.Empty);
	}

	static IEnumerable<string> VanillaAirports()
	{
		foreach (var name in new[] { "AIRPORT1.DAT", "AIRPORT2.DAT", "AIRPORT3.DAT" })
		{
			var path = Path.Combine(TestConstants.BaseSteamObjDataPath, name);
			if (File.Exists(path))
			{
				yield return path;
			}
		}
	}

	[TestCaseSource(nameof(VanillaAirports))]
	public void VanillaAirport_PassesValidationWithoutErrors(string filename)
	{
		var obj = SawyerStreamReader.LoadFullObject(filename, new Logger())!;
		var airport = (AirportObject)obj.LocoObject!.Object;

		Assert.That(Validate(airport), Is.Empty);
	}
}
