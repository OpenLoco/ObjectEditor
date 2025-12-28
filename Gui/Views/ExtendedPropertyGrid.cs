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
		Factories.AddFactory(new CurrencyCellEditFactory());
	}
}

internal class Pos3CellEditFactory : AbstractCellEditFactory
{
	public override bool Accept(object accessToken) => accessToken is ExtendedPropertyGrid;

	public override Control? HandleNewProperty(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		_ = context.Target;

		if (propertyDescriptor.PropertyType != typeof(Pos3))
		{
			return null;
		}

		var control = new Pos3View();

		return control;
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

		if (control is Pos3View vv)
		{
			var pos = (Pos3)propertyDescriptor.GetValue(target)!;

			var model = new Pos3ViewModel { Pos = pos };
			vv.DataContext = model;

			model.PropertyChanged += (s, e) => SetAndRaise(context, control, model.Pos);

			return true;
		}

		return false;
	}
}

internal class CurrencyCellEditFactory : AbstractCellEditFactory
{
	public override bool Accept(object accessToken) => accessToken is ExtendedPropertyGrid;

	public override Control? HandleNewProperty(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		var target = context.Target;

		// Check if property has CurrencyAttribute
		var currencyAttr = propertyDescriptor.Attributes.OfType<CurrencyAttribute>().FirstOrDefault();
		if (currencyAttr == null)
		{
			return null;
		}

		// Property must be int16_t (short)
		if (propertyDescriptor.PropertyType != typeof(short))
		{
			return null;
		}

		var control = new CurrencyView();
		return control;
	}

	public override bool HandlePropertyChanged(PropertyCellContext context)
	{
		var propertyDescriptor = context.Property;
		var target = context.Target;
		var control = context.CellEdit!;

		var currencyAttr = propertyDescriptor.Attributes.OfType<CurrencyAttribute>().FirstOrDefault();
		if (currencyAttr == null || propertyDescriptor.PropertyType != typeof(short))
		{
			return false;
		}

		ValidateProperty(control, propertyDescriptor, target);

		if (control is CurrencyView cv)
		{
			var costFactor = (short)propertyDescriptor.GetValue(target)!;

			// Get CostIndex from the target object
			var costIndexProperty = TypeDescriptor.GetProperties(target)[currencyAttr.CostIndexPropertyName];
			var costIndex = costIndexProperty != null ? (byte)costIndexProperty.GetValue(target)! : (byte)0;

			// Get the year from GlobalSettings or use default
			var year = GlobalSettings.CurrentSettings?.InflationYear ?? 1950;

			var model = new CurrencyEditorViewModel
			{
				CostFactor = costFactor,
				CostIndex = costIndex,
				Year = year
			};

			cv.DataContext = model;

			model.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(CurrencyEditorViewModel.CostFactor))
				{
					SetAndRaise(context, control, model.CostFactor);
				}
				else if (e.PropertyName == nameof(CurrencyEditorViewModel.Year))
				{
					// Update global settings with new year
					if (GlobalSettings.CurrentSettings != null)
					{
						GlobalSettings.CurrentSettings.InflationYear = model.Year;
					}
				}
			};

			return true;
		}

		return false;
	}
}
