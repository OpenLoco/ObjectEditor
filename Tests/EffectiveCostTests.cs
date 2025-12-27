using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.Bridge;
using Gui;
using Gui.ViewModels;
using NUnit.Framework;

namespace Gui.Tests;

[TestFixture]
public class EffectiveCostTests
{
	[SetUp]
	public void Setup()
	{
		// Set up global settings
		GlobalSettings.CurrentSettings = new EditorSettings
		{
			InflationYear = 1950
		};
	}

	[Test]
	public void TrackViewModel_CalculatesEffectiveCosts()
	{
		var trackObject = new TrackObject
		{
			BuildCostFactor = 100,
			SellCostFactor = -50,
			TunnelCostFactor = 200,
			CostIndex = 0
		};

		var viewModel = new TrackViewModel(trackObject);

		// For year 1950, with factor 1024 and divisor 10, we should get: (100 * X) / 1024
		// where X is the inflation-adjusted currency multiplication factor
		Assert.That(viewModel.EffectiveBuildCost, Is.GreaterThan(0));
		Assert.That(viewModel.EffectiveSellCost, Is.LessThan(0));
		Assert.That(viewModel.EffectiveTunnelCost, Is.GreaterThan(0));
		
		// Build cost should be proportional to the factors
		Assert.That(viewModel.EffectiveTunnelCost, Is.GreaterThan(viewModel.EffectiveBuildCost));
	}

	[Test]
	public void BridgeViewModel_CalculatesEffectiveCosts()
	{
		var bridgeObject = new BridgeObject
		{
			BaseCostFactor = 150,
			HeightCostFactor = 50,
			SellCostFactor = -75,
			CostIndex = 1
		};

		var viewModel = new BridgeViewModel(bridgeObject);

		Assert.That(viewModel.EffectiveBaseCost, Is.GreaterThan(0));
		Assert.That(viewModel.EffectiveHeightCost, Is.GreaterThan(0));
		Assert.That(viewModel.EffectiveSellCost, Is.LessThan(0));
		
		// Base cost should be greater than height cost
		Assert.That(viewModel.EffectiveBaseCost, Is.GreaterThan(viewModel.EffectiveHeightCost));
	}

	[Test]
	public void InflationYear_AffectsCosts()
	{
		var trackObject = new TrackObject
		{
			BuildCostFactor = 100,
			CostIndex = 0
		};

		// Test with year 1900 (no inflation)
		GlobalSettings.CurrentSettings!.InflationYear = 1900;
		var viewModel1900 = new TrackViewModel(trackObject);
		var cost1900 = viewModel1900.EffectiveBuildCost;

		// Test with year 2000 (with inflation)
		GlobalSettings.CurrentSettings!.InflationYear = 2000;
		var viewModel2000 = new TrackViewModel(trackObject);
		var cost2000 = viewModel2000.EffectiveBuildCost;

		// Cost in 2000 should be higher due to inflation
		Assert.That(cost2000, Is.GreaterThan(cost1900));
	}

	[Test]
	public void GlobalSettings_DefaultYear_WorksCorrectly()
	{
		// Test that if GlobalSettings is null, default year is used
		GlobalSettings.CurrentSettings = null;

		var trackObject = new TrackObject
		{
			BuildCostFactor = 100,
			CostIndex = 0
		};

		var viewModel = new TrackViewModel(trackObject);
		
		// Should not throw and should return a valid cost
		Assert.That(viewModel.EffectiveBuildCost, Is.GreaterThan(0));
	}
}
