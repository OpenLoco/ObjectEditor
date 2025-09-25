using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace Gui.ViewModels.Filters;

public class FilterViewModel : ReactiveObject
{
	[Reactive] public FilterField SelectedField { get; set; }
	[Reactive] public FilterOperator SelectedOperator { get; set; }
	[Reactive] public string? FilterValue { get; set; }

	public ObservableCollection<FilterField> AvailableFields { get; }
	public ObservableCollection<FilterOperator> AvailableOperators { get; }

	public ReactiveCommand<Unit, Unit> RemoveFilterCommand { get; }

	public FilterViewModel(Action<FilterViewModel> onRemove)
	{
		AvailableFields = new(Enum.GetValues<FilterField>());
		AvailableOperators = new(Enum.GetValues<FilterOperator>());
		RemoveFilterCommand = ReactiveCommand.Create(() => onRemove(this));
	}

	public Expression<Func<ObjectIndexEntry, bool>>? BuildExpression()
	{
		if (string.IsNullOrEmpty(FilterValue))
		{
			return null;
		}

		var parameter = Expression.Parameter(typeof(ObjectIndexEntry), "entry");
		var member = GetMemberExpression(parameter, SelectedField);

		if (member == null)
		{
			return null; // Field not supported
		}

		var constant = Expression.Constant(FilterValue);
		Expression? body = null;

		if (member.Type == typeof(string))
		{
			var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string), typeof(StringComparison)]);
			var comparisonConstant = Expression.Constant(StringComparison.OrdinalIgnoreCase);

			body = SelectedOperator switch
			{
				FilterOperator.Contains => Expression.Call(member, method!, constant, comparisonConstant),
				FilterOperator.Equals => Expression.Equal(Expression.Call(member, "ToLower", null), Expression.Call(constant, "ToLower", null)),
				FilterOperator.NotEquals => Expression.NotEqual(Expression.Call(member, "ToLower", null), Expression.Call(constant, "ToLower", null)),
				_ => null
			};
		}
		else if (member.Type.IsEnum || (Nullable.GetUnderlyingType(member.Type)?.IsEnum ?? false))
		{
			// Handle enums by parsing the string value
			var enumType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
			if (Enum.TryParse(enumType, FilterValue, true, out var enumValue))
			{
				var enumConstant = Expression.Constant(enumValue, member.Type);
				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(member, enumConstant),
					FilterOperator.NotEquals => Expression.NotEqual(member, enumConstant),
					_ => null // Contains is not valid for enums
				};
			}
		}

		return body != null ? Expression.Lambda<Func<ObjectIndexEntry, bool>>(body, parameter) : null;
	}

	private static MemberExpression? GetMemberExpression(ParameterExpression parameter, FilterField field)
	{
		return field switch
		{
			FilterField.DisplayName => Expression.Property(parameter, nameof(ObjectIndexEntry.DisplayName)),
			FilterField.ObjectType => Expression.Property(parameter, nameof(ObjectIndexEntry.ObjectType)),
			FilterField.VehicleType => Expression.Property(parameter, nameof(ObjectIndexEntry.VehicleType)),
			FilterField.ObjectSource => Expression.Property(parameter, nameof(ObjectIndexEntry.ObjectSource)),
			_ => null
		};
	}
}
