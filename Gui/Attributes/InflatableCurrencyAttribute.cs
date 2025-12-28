using System;

namespace Gui.Attributes;

/// <summary>
/// Marks a property as a currency value that should display with inflation adjustment.
/// The property must be of type int16_t and have a corresponding CostIndex property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InflatableCurrencyAttribute(string CostIndexPropertyName, string? DesignedYearPropertyName = null) : Attribute
{
	/// <summary>
	/// The name of the property that contains the CostIndex for this currency value.
	/// Defaults to "CostIndex".
	/// </summary>
	public string CostIndexPropertyName { get; } = CostIndexPropertyName;
	public string DesignedYearPropertyName { get; } = DesignedYearPropertyName;
}
