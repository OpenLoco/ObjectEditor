using Avalonia.Media;
using Common;
using Definitions.ObjectModels;
using DynamicData;
using Gui.Models;
using Index;
using PropertyModels.Extensions;
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

	public bool IsValid
		=> SelectedObjectType != null
		&& SelectedProperty != null
		&& SelectedOperator != null
		&& GetFilterValue() != null;

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

				// only add properties that are searchable (so no collections, sub objects)
				var searchableProperties = SelectedObjectType!.Type
					.GetProperties(BindingFlags.Public | BindingFlags.Instance)
					.Where(x => x.GetUnderlyingType().IsPrimitive
						|| x.GetUnderlyingType() == typeof(string)
						|| x.GetUnderlyingType() == typeof(DateOnly)
						|| x.GetUnderlyingType() == typeof(DateTime)
						|| x.GetUnderlyingType() == typeof(DateTimeOffset)
						|| x.GetUnderlyingType().IsEnum
						|| x.GetUnderlyingType() == typeof(bool)
						|| (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) && (
							Nullable.GetUnderlyingType(x.PropertyType)!.IsPrimitive
							|| Nullable.GetUnderlyingType(x.PropertyType) == typeof(DateOnly)
							|| Nullable.GetUnderlyingType(x.PropertyType) == typeof(DateTime)
							|| Nullable.GetUnderlyingType(x.PropertyType) == typeof(DateTimeOffset)
							|| Nullable.GetUnderlyingType(x.PropertyType)!.IsEnum
							|| Nullable.GetUnderlyingType(x.PropertyType) == typeof(bool)
						)))
					.OrderBy(p => p.Name);

				AvailableProperties.AddRange(searchableProperties);
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

		if (underlyingType.IsEnum)
		{
			if (underlyingType.GetCustomAttribute<FlagsAttribute>() != null)
			{
				AvailableOperators.Add(FilterOperator.Contains);
			}
			AvailableOperators.AddRange([FilterOperator.Equals, FilterOperator.NotEquals]);
		}
		else if (underlyingType == typeof(bool))
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

	public Func<ObjectIndexEntry, bool>? BuildExpression(bool isLocal)
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
		//else if (SelectedObjectType.Type == typeof(MetadataModel))
		//{
		//	return BuildFilterExpression<MetadataModel>()?.Compile();
		//}
		// Otherwise, build a delegate that loads the object from disk
		return (ObjectIndexEntry entry) => BuildObjectFilter(entry, isLocal);
	}

	//bool BuildMetadataFilter(ObjectIndexEntry entry)
	//{
	//	return BuildFilterExpression<MetadataModel>()?.Compile();
	//}

	bool BuildObjectFilter(ObjectIndexEntry entry, bool isLocal)
	{
		if (!isLocal)
		{
			// online mode not supported yet
			return false;
		}

		if (ObjectTypeMapping.StructTypeToObjectType(SelectedObjectType.Type) != entry.ObjectType)
		{
			return false;
		}

		var fileSystemItem = FolderTreeViewModel.IndexEntryToFileSystemItem(entry, _model.Settings.ObjDataDirectory, FileLocation.Local);
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

		return CreateFilterExpression<T>(parameter, member);
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
		return CreateFilterExpression<ILocoStruct>(parameter, member);
	}

	private Expression<Func<T, bool>>? CreateFilterExpression<T>(ParameterExpression parameter, MemberExpression member)
	{
		Expression? body = null;

		var underlyingType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
		var isNullable = Nullable.GetUnderlyingType(member.Type) != null || !member.Type.IsValueType;

		// For nullable value types, we need to access the .Value property
		var memberToCompare = Nullable.GetUnderlyingType(member.Type) != null
			? Expression.Property(member, "Value")
			: member;

		if (underlyingType.IsEnum && EnumValue != null)
		{
			var enumConstant = Expression.Constant(EnumValue, underlyingType);

			if (underlyingType.GetCustomAttribute<FlagsAttribute>() == null)
			{

				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(memberToCompare, enumConstant),
					FilterOperator.NotEquals => Expression.NotEqual(memberToCompare, enumConstant),
					_ => null
				};
			}
			else
			{
				// special handling required for flags enums - we need to convert both sides to the underlying type to do a bitwise AND
				var enumUnderlyingType = Enum.GetUnderlyingType(underlyingType);
				var convertedEnumConstant = Expression.Constant(
					Convert.ChangeType(EnumValue, enumUnderlyingType),
					enumUnderlyingType);

				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(memberToCompare, enumConstant),
					FilterOperator.NotEquals => Expression.NotEqual(memberToCompare, enumConstant),
					FilterOperator.Contains => Expression.NotEqual(
						Expression.And(
							Expression.Convert(memberToCompare, enumUnderlyingType),
							convertedEnumConstant),
						Expression.Constant(Convert.ChangeType(0, enumUnderlyingType), enumUnderlyingType)),
					_ => null
				};
			}
		}
		else if (underlyingType == typeof(bool) && BoolValue != null)
		{
			var constant = Expression.Constant(BoolValue);
			body = SelectedOperator switch
			{
				FilterOperator.Equals => Expression.Equal(memberToCompare, constant),
				FilterOperator.NotEquals => Expression.NotEqual(memberToCompare, constant),
				_ => null
			};
		}
		else if (IsNumericType(underlyingType) && TextValue != null)
		{
			try
			{
				var convertedValue = Convert.ChangeType(TextValue, underlyingType);
				var constant = Expression.Constant(convertedValue, underlyingType);

				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(memberToCompare, constant),
					FilterOperator.NotEquals => Expression.NotEqual(memberToCompare, constant),
					FilterOperator.GreaterThan => Expression.GreaterThan(memberToCompare, constant),
					FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(memberToCompare, constant),
					FilterOperator.LessThan => Expression.LessThan(memberToCompare, constant),
					FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(memberToCompare, constant),
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
				var constant = Expression.Constant(convertedValue, underlyingType);

				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(memberToCompare, constant),
					FilterOperator.NotEquals => Expression.NotEqual(memberToCompare, constant),
					FilterOperator.GreaterThan => Expression.GreaterThan(memberToCompare, constant),
					FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(memberToCompare, constant),
					FilterOperator.LessThan => Expression.LessThan(memberToCompare, constant),
					FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(memberToCompare, constant),
					_ => null
				};
			}
			catch (Exception)
			{
				return null; // Conversion failed
			}
		}
		else if (underlyingType == typeof(string) && !string.IsNullOrEmpty(TextValue))
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

		// Generic null check for all nullable types
		if (isNullable)
		{
			Expression nullCheck;
			
			// For Nullable<T> value types, check HasValue property
			if (Nullable.GetUnderlyingType(member.Type) != null)
			{
				nullCheck = Expression.Property(member, "HasValue");
			}
			// For reference types (like string), check for null
			else
			{
				nullCheck = Expression.NotEqual(member, Expression.Constant(null, member.Type));
			}

			body = Expression.AndAlso(nullCheck, body);
		}

		return Expression.Lambda<Func<T, bool>>(body, parameter);
	}
}
