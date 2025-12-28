using NUnit.Framework;

namespace Common.Tests;

[TestFixture]
public class EconomyTests
{
	[Test]
	public void CalculateCurrencyMultiplicationFactors_Year1900_ReturnsBaseFactors()
	{
		// Year 1900 should return base factors (1024 for all indices)
		var factors = Economy.CalculateCurrencyMultiplicationFactors(1900);

		Assert.That(factors, Has.Length.EqualTo(32));
		foreach (var factor in factors)
		{
			Assert.That(factor, Is.EqualTo(1024));
		}
	}

	[Test]
	public void CalculateCurrencyMultiplicationFactors_YearBefore1900_ReturnsSameAs1900()
	{
		// Years before 1900 should be treated as 1900
		var factors1800 = Economy.CalculateCurrencyMultiplicationFactors(1800);
		var factors1900 = Economy.CalculateCurrencyMultiplicationFactors(1900);

		Assert.That(factors1800, Is.EqualTo(factors1900));
	}

	[Test]
	public void CalculateCurrencyMultiplicationFactors_Year1901_HasInflation()
	{
		// Year 1901 should have inflation applied (12 months)
		var factors = Economy.CalculateCurrencyMultiplicationFactors(1901);

		Assert.That(factors, Has.Length.EqualTo(32));
		// All factors should be greater than base value due to inflation
		foreach (var factor in factors)
		{
			Assert.That(factor, Is.GreaterThan(1024));
		}
	}

	[Test]
	public void CalculateCurrencyMultiplicationFactors_Year2030_ClampsCorrectly()
	{
		// Year 2030 is the max, so 2031 should be treated as 2030
		var factors2030 = Economy.CalculateCurrencyMultiplicationFactors(2030);
		var factors2031 = Economy.CalculateCurrencyMultiplicationFactors(2031);

		Assert.That(factors2031, Is.EqualTo(factors2030));
	}

	[Test]
	public void GetInflationAdjustedCost_Year1900_ReturnsBaseCost()
	{
		// For year 1900, with factor 1024 and divisor 10, we should get:
		// cost = (100 * 1024) / (1 << 10) = 102400 / 1024 = 100
		var cost = Economy.GetInflationAdjustedCost(100, 0, 1900, 10);
		Assert.That(cost, Is.EqualTo(100));
	}

	[Test]
	public void GetInflationAdjustedCost_Year1901_HasHigherCost()
	{
		// For year 1901, cost should be higher than year 1900 due to inflation
		var cost1900 = Economy.GetInflationAdjustedCost(100, 0, 1900, 10);
		var cost1901 = Economy.GetInflationAdjustedCost(100, 0, 1901, 10);

		Assert.That(cost1901, Is.GreaterThan(cost1900));
	}

	[Test]
	public void GetInflationAdjustedCost_NegativeFactor_ReturnsNegativeCost()
	{
		// Sell costs use negative factors
		var cost = Economy.GetInflationAdjustedCost(-50, 0, 1900, 10);
		Assert.That(cost, Is.LessThan(0));
		Assert.That(cost, Is.EqualTo(-50));
	}

	[Test]
	public void GetInflationAdjustedCost_InvalidCostIndex_ThrowsException()
	{
		_ = Assert.Throws<ArgumentOutOfRangeException>(() =>
			Economy.GetInflationAdjustedCost(100, 32, 1900, 10));
	}

	[Test]
	public void GetInflationAdjustedCost_InvalidDivisor_ThrowsException()
	{
		_ = Assert.Throws<ArgumentOutOfRangeException>(() =>
			Economy.GetInflationAdjustedCost(100, 0, 1900, 63));
	}
}
