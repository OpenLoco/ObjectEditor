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
	[Reactive] public FilterOperator SelectedOperator { get; set; } = FilterOperator.Equals;
	[Reactive] public object? FilterValue { get; set; }

	public ObservableCollection<PropertyInfo> AvailableFields { get; set; } = [];
	public ObservableCollection<FilterOperator> AvailableOperators { get; set; } = [];

	public ReactiveCommand<Unit, Unit> RemoveFilterCommand { get; }

	public ObservableCollection<FilterTypeViewModel> AvailableFiltersList { get; }

	public FilterViewModel(List<FilterTypeViewModel> availableFilters, Action<FilterViewModel> onRemove)
	{
		AvailableFiltersList = new(availableFilters);

		_ = this.WhenAnyValue(x => x.SelectedObjectType)
			.Where(x => x != null)
			.Subscribe(_ =>
			{
				FilterValue = null;

				AvailableFields.Clear();
				AvailableFields.AddRange(SelectedObjectType!.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(p => p.Name));
				SelectedField = AvailableFields.FirstOrDefault();

				SelectedOperator = FilterOperator.Equals;
				AvailableOperators.Clear();
				AvailableOperators.AddRange(Enum.GetValues<FilterOperator>());
			});

		RemoveFilterCommand = ReactiveCommand.Create(() => onRemove(this));
	}

	public Expression<Func<ObjectIndexEntry, bool>>? BuildExpression()
	{
		if (FilterValue == null || SelectedField == null)
		{
			return null;
		}

		var parameter = Expression.Parameter(SelectedObjectType.Type, SelectedObjectType.Type.Name);
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
			if (Enum.TryParse(enumType, FilterValue as string, true, out var enumValue))
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
