using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.TownNames;

public class TownNamesObject : ILocoStruct
{
#pragma warning disable IL2026 // LengthAttribute constructor uses reflection to get 'Count' on non-ICollection types; our properties use List<T> which implements ICollection so Count is preserved.
	[Length(6, 6)]
	public List<Category> Categories { get; set; } = [];
#pragma warning restore IL2026

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
