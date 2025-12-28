using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Definitions.ObjectModels.Types;
using Gui.Attributes;
using Gui.ViewModels;
using System.ComponentModel;
using System.Linq;

namespace Gui.Views;

public class ExtendedPropertyGrid : PropertyGrid
{
	public ExtendedPropertyGrid()
	{
		Factories.AddFactory(new Pos3CellEditFactory());
		Factories.AddFactory(new InflatableCurrencyCellEditFactory());
	}
}

internal class Pos3CellEditFactory : AbstractCellEditFactory
{
	public override bool Accept(object accessToken)
		=> accessToken is ExtendedPropertyGrid;

	public override Control? HandleNewProperty(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		_ = context.Target;

		if (propertyDescriptor.PropertyType != typeof(Pos3))
		{
			return null;
		}

		return new Pos3View();
	}

	public override bool HandlePropertyChanged(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		var target = context.Target;
		var control = context.CellEdit!;

		if (propertyDescriptor.PropertyType != typeof(Pos3))
		{
			return false;
		}

		ValidateProperty(control, propertyDescriptor, target);

		if (control is Pos3View pv)
		{
			var pos = (Pos3)propertyDescriptor.GetValue(target)!;

			var model = new Pos3ViewModel { Pos = pos };
			pv.DataContext = model;

			model.PropertyChanged += (s, e) => SetAndRaise(context, control, model.Pos);
			return true;
		}

		return false;
	}
}

internal class InflatableCurrencyCellEditFactory : AbstractCellEditFactory
{
	public override bool Accept(object accessToken)
		=> accessToken is ExtendedPropertyGrid;

	public override Control? HandleNewProperty(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		_ = context.Target;

		var currencyAttr = propertyDescriptor.Attributes.OfType<InflatableCurrencyAttribute>().FirstOrDefault();
		if (currencyAttr == null)
		{
			return null;
		}

		if (propertyDescriptor.PropertyType != typeof(short))
		{
			return null;
		}

		return new InflatableCurrencyView();
	}

	public override bool HandlePropertyChanged(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		var target = context.Target;
		var control = context.CellEdit!;

		var currencyAttr = propertyDescriptor.Attributes.OfType<InflatableCurrencyAttribute>().FirstOrDefault();
		if (currencyAttr == null || propertyDescriptor.PropertyType != typeof(short))
		{
			return false;
		}

		ValidateProperty(control, propertyDescriptor, target);

		if (control is InflatableCurrencyView cv)
		{
			if (cv is null)
			{
				return false;
			}

			var costFactor = (short)propertyDescriptor.GetValue(target)!;

			// Get CostIndex from the target object
			var costIndexProperty = TypeDescriptor.GetProperties(target)[currencyAttr.CostIndexPropertyName];
			var costIndex = costIndexProperty != null
				? (uint8_t)costIndexProperty.GetValue(target)!
				: (uint8_t)0;

			var designedYearProperty = TypeDescriptor.GetProperties(target)[currencyAttr.DesignedYearPropertyName];
			var designedYear = designedYearProperty != null
				? (uint16_t)designedYearProperty.GetValue(target)!
				: (uint16_t)1950;

			var currVm = (InflatableCurrencyViewModel?)cv?.DataContext;
			var year = currVm?.Year ?? designedYear;
			// objects can actually set any year as designed year, even 0, so lets sanitize it
			if (year < 1800)
			{
				year = InflatableCurrencyViewModel.DefaultYear;
			}

			var model = new InflatableCurrencyViewModel
			{
				CostFactor = costFactor,
				CostIndex = costIndex,
				Year = year
			};

			cv.DataContext = model;

			model.PropertyChanged += (s, e) => SetAndRaise(context, control, model.CostFactor);
			return true;
		}

		return false;
	}
}
