using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using Definitions.ObjectModels.Types;
using Gui.ViewModels;

namespace Gui.Views;

public class ExtendedPropertyGrid : PropertyGrid
{
	public ExtendedPropertyGrid()
	{
		Factories.AddFactory(new Pos3CellEditFactory());
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
