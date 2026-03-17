using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Validation;

[AttributeUsage(AttributeTargets.Property)]
public class CountEqualToAttribute(string otherProperty) : ValidationAttribute
{
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075", Justification = "ValidationContext.ObjectType is expected to have its public properties preserved when validation is performed via DataAnnotations.")]
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var otherPropertyInfo = validationContext.ObjectType.GetProperty(otherProperty);
		if (otherPropertyInfo == null)
		{
			return new ValidationResult($"Unknown property: {otherProperty}");
		}

		var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

		if (value is not ICollection currentCollection || otherValue is not ICollection otherCollection)
		{
			// This validation is only for collections. If properties are not collections, skip validation.
			return ValidationResult.Success;
		}

		if (currentCollection.Count != otherCollection.Count)
		{
			return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} and {otherProperty} must have the same number of items.");
		}

		return ValidationResult.Success;
	}
}
