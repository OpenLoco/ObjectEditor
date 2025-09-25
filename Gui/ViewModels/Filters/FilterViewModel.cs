using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reflection;

namespace Gui.ViewModels.Filters;

public class FilterViewModel : ReactiveObject
{
	[Reactive] public PropertyInfo? SelectedField { get; set; }
	[Reactive] public FilterOperator SelectedOperator { get; set; }
	[Reactive] public string? FilterValue { get; set; }

	public ObservableCollection<PropertyInfo> AvailableFields { get; }
	public ObservableCollection<FilterOperator> AvailableOperators { get; }

	public ReactiveCommand<Unit, Unit> RemoveFilterCommand { get; }

	public FilterViewModel(Type objectType, Action<FilterViewModel> onRemove)
	{
		AvailableFields = new(objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name));
		AvailableOperators = new(Enum.GetValues<FilterOperator>());
		RemoveFilterCommand = ReactiveCommand.Create(() => onRemove(this));
	}

	public Expression<Func<ObjectIndexEntry, bool>>? BuildExpression()
	{
		if (string.IsNullOrEmpty(FilterValue) || SelectedField == null)
		{
			return null;
		}

		var parameter = Expression.Parameter(typeof(ObjectIndexEntry), "entry");
		var member = Expression.Property(parameter, SelectedField.Name);
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
			var enumType = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
			if (Enum.TryParse(enumType, FilterValue, true, out var enumValue))
			{
				var enumConstant = Expression.Constant(enumValue, member.Type);
				body = SelectedOperator switch
				{
					FilterOperator.Equals => Expression.Equal(member, enumConstant),
					FilterOperator.NotEquals => Expression.NotEqual(member, enumConstant),
					_ => null
				};
			}
		}

		return body != null ? Expression.Lambda<Func<ObjectIndexEntry, bool>>(body, parameter) : null;
	}
}
