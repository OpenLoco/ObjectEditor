using Avalonia.Media;
using DynamicData;
using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

namespace Gui.ViewModels.Filters;

public class FilterTypeViewModel : ReactiveObject
{
	public Type Type { get; set; }
	public string DisplayName { get; set; }
	public string IconName { get; set; } = "CircleSmall";

	public IBrush? BackgroundColour { get; set; }
}

public class FilterViewModel : ReactiveObject
{
	[Reactive] public FilterTypeViewModel? SelectedObjectType { get; set; }
	[Reactive] public PropertyInfo? SelectedField { get; set; }
	[Reactive] public FilterOperator SelectedOperator { get; set; }
	[Reactive]
	public object? FilterValue
	{
		get;
		set;
	}

	public ObservableCollection<PropertyInfo> AvailableFields { get; set; } = [];
	public ObservableCollection<FilterOperator> AvailableOperators { get; set; } = [];
	public ObservableCollection<object> AvailableEnumValues { get; } = [];

	[Reactive] public bool IsTextValue { get; private set; }
	[Reactive] public bool IsDateValue { get; private set; }
	[Reactive] public bool IsEnumValue { get; private set; }
	[Reactive] public bool IsBoolValue { get; private set; }

	public ReactiveCommand<Unit, Unit> RemoveFilterCommand { get; }

	public ObservableCollection<FilterTypeViewModel> AvailableFiltersList { get; }

	public FilterViewModel(List<FilterTypeViewModel> availableFilters, Action<FilterViewModel> onRemove)
	{
		AvailableFiltersList = new(availableFilters);

		_ = this.WhenAnyValue(x => x.SelectedObjectType)
			.Where(x => x != null)
			.Subscribe(_ =>
			{
				AvailableFields.Clear();
				AvailableFields.AddRange(SelectedObjectType!.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name));
				SelectedField = AvailableFields.FirstOrDefault();
			});

		_ = this.WhenAnyValue(x => x.SelectedField)
			.Where(field => field is not null)
			.Subscribe(field =>
			{
				UpdateAvailableOperators(field!.PropertyType);
				UpdateValueInputType(field!.PropertyType);
			});

		RemoveFilterCommand = ReactiveCommand.Create(() => onRemove(this));
	}

	private void UpdateValueInputType(Type propertyType)
	{
		var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

		IsTextValue = false;
		IsDateValue = false;
		IsEnumValue = false;
		IsBoolValue = false;
		AvailableEnumValues.Clear();

		if (underlyingType.IsEnum)
		{
			IsEnumValue = true;
			var enumValues = Enum.GetValues(underlyingType);
			AvailableEnumValues.AddRange(enumValues.Cast<object>());
			FilterValue = enumValues.GetValue(0);
		}
		else if (underlyingType == typeof(bool))
		{
			IsBoolValue = true;
			FilterValue = false;
		}
		else if (underlyingType == typeof(string) || IsNumericType(underlyingType))
		{
			IsTextValue = true;
			FilterValue = string.Empty;
		}
		else if (underlyingType == typeof(DateOnly))
		{
			IsDateValue = true;
			// Set a default value for the date picker
			FilterValue = DateOnly.FromDateTime(DateTime.Now);
		}
	}

	private void UpdateAvailableOperators(Type propertyType)
	{
		AvailableOperators.Clear();
		var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

		if (underlyingType == typeof(string))
		{
			AvailableOperators.AddRange([
				FilterOperator.Contains,
				FilterOperator.Equals,
				FilterOperator.NotEquals,
				FilterOperator.StartsWith,
				FilterOperator.EndsWith
			]);
		}
		else if (underlyingType.IsEnum || underlyingType == typeof(bool))
		{
			AvailableOperators.AddRange([FilterOperator.Equals, FilterOperator.NotEquals]);
		}
		else if (IsNumericType(underlyingType) || underlyingType == typeof(DateOnly))
		{
			AvailableOperators.AddRange([
				FilterOperator.Equals,
				FilterOperator.NotEquals,
				FilterOperator.GreaterThan,
				FilterOperator.GreaterThanOrEqual,
				FilterOperator.LessThan,
				FilterOperator.LessThanOrEqual
			]);
		}
		else
		{
			AvailableOperators.AddRange([FilterOperator.Equals, FilterOperator.NotEquals]);
		}

		SelectedOperator = AvailableOperators.FirstOrDefault();
	}

	private static bool IsNumericType(Type type) => Type.GetTypeCode(type) switch
	{
		TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
		_ => false,
	};

	public Expression<Func<ObjectIndexEntry, bool>>? BuildExpression()
	{
		if (FilterValue == null || SelectedField == null)
		{
			return null;
		}

		var parameter = Expression.Parameter(typeof(ObjectIndexEntry), "x");
		var member = Expression.Property(parameter, SelectedField.Name);
		Expression? body = null;

		var underlyingType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

		if (underlyingType == typeof(string))
		{
			var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string), typeof(StringComparison)]);
			var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string), typeof(StringComparison)]);
			var endsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string), typeof(StringComparison)]);
			var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

			var comparisonConstant = Expression.Constant(StringComparison.OrdinalIgnoreCase);
			var constant = Expression.Constant(FilterValue);

			var memberLower = Expression.Call(member, toLowerMethod!);
			var constantLower = Expression.Call(constant, toLowerMethod!);

			body = SelectedOperator switch
			{
				FilterOperator.Contains => Expression.Call(member, method!, constant, comparisonConstant),
				FilterOperator.StartsWith => Expression.Call(member, startsWithMethod!, constant, comparisonConstant),
				FilterOperator.EndsWith => Expression.Call(member, endsWithMethod!, constant, comparisonConstant),
				FilterOperator.Equals => Expression.Equal(memberLower, constantLower),
				FilterOperator.NotEquals => Expression.NotEqual(memberLower, constantLower),
				_ => null
			};
		}
		else if (underlyingType.IsEnum)
		{
			var enumConstant = Expression.Constant(FilterValue, member.Type);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(member, enumConstant),
				FilterOperator.NotEquals => Expression.NotEqual(member, enumConstant),
				_ => null
			};
		}
		else if (IsNumericType(underlyingType) || underlyingType == typeof(DateOnly))
		{
			try
			{
				var convertedValue = Convert.ChangeType(FilterValue, underlyingType);
				var constant = Expression.Constant(convertedValue, member.Type);

				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(member, constant),
					FilterOperator.NotEquals => Expression.NotEqual(member, constant),
					FilterOperator.GreaterThan => Expression.GreaterThan(member, constant),
					FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(member, constant),
					FilterOperator.LessThan => Expression.LessThan(member, constant),
					FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(member, constant),
					_ => null
				};
			}
			catch (Exception)
			{
				return null; // Conversion failed
			}
		}
		else if (underlyingType == typeof(bool))
		{
			var constant = Expression.Constant(FilterValue);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(member, constant),
				FilterOperator.NotEquals => Expression.NotEqual(member, constant),
				_ => null
			};
		}

		if (body == null)
		{
			return null;
		}

		return Expression.Lambda<Func<ObjectIndexEntry, bool>>(body, parameter);
	}
}
