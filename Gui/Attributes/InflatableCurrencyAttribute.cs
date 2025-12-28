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
	/// </summary>
	public string CostIndexPropertyName { get; } = CostIndexPropertyName;

	/// <summary>
	/// The name of the property that contains the designed year associated with this currency value.
	/// This is used as the default year value if it exists.
	/// Defaults to <see langword="null"/>, meaning no designed year is used.
	/// </summary>
	public string DesignedYearPropertyName { get; } = DesignedYearPropertyName;
}
