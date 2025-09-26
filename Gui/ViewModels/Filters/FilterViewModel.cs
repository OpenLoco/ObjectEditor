using Avalonia.Media;
using Common;
using Definitions.ObjectModels;
using DynamicData;
using Gui.Models;
using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections;
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
	private readonly ObjectEditorModel _model;

	[Reactive] public FilterTypeViewModel? SelectedObjectType { get; set; }
	[Reactive] public PropertyInfo? SelectedProperty { get; set; }
	[Reactive] public FilterOperator? SelectedOperator { get; set; }
	[Reactive] public string? TextValue { get; set; }
	[Reactive] public DateTimeOffset? DateValue { get; set; }
	[Reactive] public object? EnumValue { get; set; }
	[Reactive] public bool? BoolValue { get; set; }

	public ObservableCollection<PropertyInfo> AvailableProperties { get; set; } = [];
	public ObservableCollection<FilterOperator> AvailableOperators { get; set; } = [];
	public ObservableCollection<object> AvailableEnumValues { get; set; } = [];

	[Reactive] public bool IsTextValue { get; private set; }
	[Reactive] public bool IsDateValue { get; private set; }
	[Reactive] public bool IsEnumValue { get; private set; }
	[Reactive] public bool IsBoolValue { get; private set; }

	public ReactiveCommand<Unit, Unit> RemoveFilterCommand { get; }

	public ObservableCollection<FilterTypeViewModel> AvailableFiltersList { get; set; } = [];

	public FilterViewModel(ObjectEditorModel model, List<FilterTypeViewModel> availableFilters, Action<FilterViewModel> onRemove)
	{
		_model = model;
		AvailableFiltersList.AddRange(availableFilters);
		SelectedObjectType = AvailableFiltersList.FirstOrDefault();

		_ = this.WhenAnyValue(x => x.SelectedObjectType)
			.Where(x => x != null)
			.Subscribe(_ =>
			{
				AvailableProperties.Clear();
				AvailableProperties.AddRange(SelectedObjectType!.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name));
				SelectedProperty = AvailableProperties.FirstOrDefault();
				SelectedOperator = null;
			});

		_ = this.WhenAnyValue(x => x.SelectedProperty)
			.Where(x => x != null)
			.Subscribe(property =>
			{
				UpdateAvailableOperators(property!.PropertyType);
				UpdateValueInputType(property!.PropertyType);
			});

		RemoveFilterCommand = ReactiveCommand.Create(() => onRemove(this));
	}

	private void UpdateAvailableOperators(Type propertyType)
	{
		AvailableOperators.Clear();
		var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

		if (underlyingType.IsEnum || underlyingType == typeof(bool))
		{
			AvailableOperators.AddRange([FilterOperator.Equals, FilterOperator.NotEquals]);
		}
		else if (underlyingType == typeof(string))
		{
			AvailableOperators.AddRange([
				FilterOperator.Contains,
				FilterOperator.Equals,
				FilterOperator.NotEquals,
				FilterOperator.StartsWith,
				FilterOperator.EndsWith
			]);
		}
		else if (IsNumericType(underlyingType) || underlyingType == typeof(DateOnly) || underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
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

	private void UpdateValueInputType(Type propertyType)
	{
		var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
		AvailableEnumValues.Clear();

		IsTextValue = false;
		IsDateValue = false;
		IsEnumValue = false;
		IsBoolValue = false;

		if (underlyingType.IsEnum)
		{
			IsEnumValue = true;
			var enumValues = Enum.GetValues(underlyingType);
			AvailableEnumValues.AddRange(enumValues.Cast<object>());
			EnumValue = enumValues.GetValue(0);
			//Debug.Assert(EnumValue == enumValues.GetValue(0));
		}
		else if (underlyingType == typeof(bool))
		{
			IsBoolValue = true;
			BoolValue = false;
			//Debug.Assert((bool)BoolValue == false);
		}
		else if (underlyingType == typeof(DateOnly) || underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
		{
			IsDateValue = true;
			// Set a default value for the date picker
			DateValue = DateTimeOffset.Now;
			//Debug.Assert((DateOnly)DateValue == DateOnly.Today);
		}
		else if (underlyingType == typeof(string) || IsNumericType(underlyingType))
		{
			IsTextValue = true;
			TextValue = string.Empty;
			//Debug.Assert((string)TextValue == string.Empty);
		}
		else if (underlyingType == typeof(ICollection) || underlyingType == typeof(ICollection<>) || underlyingType.IsGenericType)
		{
			// not implemented at the moment
		}
		else
		{
			//Debug.Assert(false, "Unsupported property type for filtering");
		}
	}

	private static bool IsNumericType(Type type)
		=> Type.GetTypeCode(type) switch
		{
			TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
			_ => false,
		};

	public Func<ObjectIndexEntry, bool>? BuildExpression()
	{
		if (SelectedProperty == null || SelectedObjectType == null)
		{
			return null;
		}

		// If the filter is on the index itself, build a fast expression tree
		if (SelectedObjectType.Type == typeof(ObjectIndexEntry))
		{
			return BuildFilterExpression<ObjectIndexEntry>()?.Compile();
		}

		// Otherwise, build a delegate that loads the object from disk
		return BuildObjectFilter;
	}

	private object? GetFilterValue()
	{
		if (IsBoolValue)
		{
			return BoolValue;
		}

		if (IsDateValue)
		{
			return DateValue;
		}

		if (IsEnumValue)
		{
			return EnumValue;
		}

		if (IsTextValue)
		{
			return TextValue;
		}

		return null;
	}

	bool BuildObjectFilter(ObjectIndexEntry entry)
	{
		if (ObjectTypeMapping.StructTypeToObjectType(SelectedObjectType.Type) != entry.ObjectType)
		{
			return false;
		}

		var fileSystemItem = FolderTreeViewModel.IndexEntryToFileSystemItem(entry, _model.Settings.ObjDataDirectory, FileLocation.Local); // todo: change this to support online mode
		if (!_model.TryLoadObject(fileSystemItem, out var locoFile) || locoFile?.LocoObject == null)
		{
			return false;
		}

		// should never trigger because the first type check should catch this
		if (locoFile.LocoObject.ObjectType != entry.ObjectType)
		{
			return false;
		}

		var locoObject = locoFile.LocoObject.Object;
		return BuildObjectFilterExpression(SelectedObjectType.Type)?.Compile().Invoke(locoObject) ?? false;
	}

	private Expression<Func<T, bool>>? BuildFilterExpression<T>()
	{
		if (SelectedProperty == null)
		{
			return null;
		}

		var parameter = Expression.Parameter(typeof(T), "x");
		var member = Expression.Property(parameter, SelectedProperty.Name);
		Expression? body = null;

		var underlyingType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

		if (underlyingType.IsEnum && EnumValue != null)
		{
			var enumConstant = Expression.Constant(EnumValue, member.Type);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(member, enumConstant),
				FilterOperator.NotEquals => Expression.NotEqual(member, enumConstant),
				_ => null
			};
		}
		else if (underlyingType == typeof(bool) && BoolValue != null)
		{
			var constant = Expression.Constant(BoolValue);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(member, constant),
				FilterOperator.NotEquals => Expression.NotEqual(member, constant),
				_ => null
			};
		}
		else if (IsNumericType(underlyingType) && TextValue != null)
		{
			try
			{
				var convertedValue = Convert.ChangeType(TextValue, underlyingType);
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
		else if (underlyingType == typeof(DateOnly) && DateValue != null)
		{
			try
			{
				var convertedValue = Convert.ChangeType(DateValue, underlyingType);
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
		else if (underlyingType == typeof(string) && TextValue != null)
		{
			var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string), typeof(StringComparison)]);
			var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string), typeof(StringComparison)]);
			var endsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string), typeof(StringComparison)]);
			var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

			var comparisonConstant = Expression.Constant(StringComparison.OrdinalIgnoreCase);
			var constant = Expression.Constant(TextValue);

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

		if (body == null)
		{
			return null;
		}

		return Expression.Lambda<Func<T, bool>>(body, parameter);
	}

	private Expression<Func<ILocoStruct, bool>>? BuildObjectFilterExpression(Type t)
	{
		if (SelectedProperty == null)
		{
			return null;
		}

		var parameter = Expression.Parameter(typeof(ILocoStruct), "x");
		var convertedParameter = Expression.Convert(parameter, t); // Cast the interface to the concrete type
		var member = Expression.Property(convertedParameter, SelectedProperty.Name);
		Expression? body = null;

		var underlyingType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;

		if (underlyingType.IsEnum && EnumValue != null)
		{
			var enumConstant = Expression.Constant(EnumValue, member.Type);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(member, enumConstant),
				FilterOperator.NotEquals => Expression.NotEqual(member, enumConstant),
				_ => null
			};
		}
		else if (underlyingType == typeof(bool) && BoolValue != null)
		{
			var constant = Expression.Constant(BoolValue);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(member, constant),
				FilterOperator.NotEquals => Expression.NotEqual(member, constant),
				_ => null
			};
		}
		else if (IsNumericType(underlyingType) && TextValue != null)
		{
			try
			{
				var convertedValue = Convert.ChangeType(TextValue, underlyingType);
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
		else if (underlyingType == typeof(DateOnly) && DateValue != null)
		{
			try
			{
				var convertedValue = Convert.ChangeType(DateValue, underlyingType);
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
		else if (underlyingType == typeof(string) && TextValue != null)
		{
			var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string), typeof(StringComparison)]);
			var startsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string), typeof(StringComparison)]);
			var endsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string), typeof(StringComparison)]);
			var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

			var comparisonConstant = Expression.Constant(StringComparison.OrdinalIgnoreCase);
			var constant = Expression.Constant(TextValue);

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

		if (body == null)
		{
			return null;
		}

		return Expression.Lambda<Func<ILocoStruct, bool>>(body, parameter);
	}
}
