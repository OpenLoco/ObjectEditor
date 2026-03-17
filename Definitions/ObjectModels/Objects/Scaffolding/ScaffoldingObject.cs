using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Scaffolding;

public class ScaffoldingObject : ILocoStruct
{
#pragma warning disable IL2026 // LengthAttribute constructor uses reflection to get 'Count' on non-ICollection types; our properties use List<T> which implements ICollection so Count is preserved.
	[Length(3, 3)]
	public List<uint16_t> SegmentHeights { get; set; } = [];

	[Length(3, 3)]
	public List<uint16_t> RoofHeights { get; set; } = [];
#pragma warning restore IL2026

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
