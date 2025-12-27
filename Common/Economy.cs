namespace Common;

/// <summary>
/// Provides economy-related calculations for Locomotion, including inflation and cost calculations.
/// Based on OpenLoco's Economy.cpp: https://github.com/OpenLoco/OpenLoco/blob/master/src/OpenLoco/src/Economy/Economy.cpp
/// </summary>
public static class Economy
{
	// Inflation factors from OpenLoco (kInflationFactors)
	private static readonly uint[] InflationFactors =
	[
		20, 20, 20, 20, 23, 20, 23, 23,
		20, 17, 17, 20, 20, 20, 20, 20,
		20, 20, 20, 20, 20, 20, 20, 20,
		20, 20, 20, 20, 20, 20, 20, 20
	];

	/// <summary>
	/// Calculates the currency multiplication factors for all 32 cost indices for a given year.
	/// </summary>
	/// <param name="year">The year to calculate inflation for (clamped to 1900-2030 range)</param>
	/// <returns>An array of 32 currency multiplication factors</returns>
	public static uint[] CalculateCurrencyMultiplicationFactors(int year)
	{
		var factors = new uint[32];
		
		// Initialize all factors to 1024 (base value)
		for (var i = 0; i < 32; i++)
		{
			factors[i] = 1024;
		}

		// OpenLoco allows 1800 as the minimum year, whereas Locomotion uses 1900.
		// Treat years before 1900 as though they were 1900 to not change vanilla scenarios.
		var baseYear = Math.Clamp(year, 1900, 2030) - 1900;
		var monthCount = baseYear * 12;

		// Apply inflation for each month
		for (var month = 0; month < monthCount; month++)
		{
			for (var i = 0; i < 32; i++)
			{
				factors[i] += (uint)((ulong)InflationFactors[i] * factors[i] >> 12);
			}
		}

		return factors;
	}

	/// <summary>
	/// Calculates the inflation-adjusted cost for a given cost factor and cost index.
	/// </summary>
	/// <param name="costFactor">The base cost factor from the object</param>
	/// <param name="costIndex">The cost index (0-31)</param>
	/// <param name="year">The year to calculate the cost for</param>
	/// <param name="divisor">The divisor to apply (default is 10, which is common for most objects)</param>
	/// <returns>The inflation-adjusted cost</returns>
	public static int GetInflationAdjustedCost(short costFactor, byte costIndex, int year, byte divisor = 10)
	{
		if (costIndex >= 32)
		{
			throw new ArgumentOutOfRangeException(nameof(costIndex), "Cost index must be between 0 and 31");
		}

		if (divisor >= 63)
		{
			throw new ArgumentOutOfRangeException(nameof(divisor), "Divisor must be less than 63");
		}

		var factors = CalculateCurrencyMultiplicationFactors(year);
		var val = costFactor * (long)factors[costIndex];
		var result = val / (1L << divisor);
		
		return (int)result;
	}
}
