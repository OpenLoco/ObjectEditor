using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Streetlight;

public class StreetLightObject : ILocoStruct
{
#pragma warning disable IL2026 // LengthAttribute constructor uses reflection to get 'Count' on non-ICollection types; our properties use List<T> which implements ICollection so Count is preserved.
	[Length(3, 3)]
	public List<uint16_t> DesignedYears { get; set; } = [];
#pragma warning restore IL2026

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
