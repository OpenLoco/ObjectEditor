using Avalonia.Controls;
using Avalonia.Controls.Templates;
using OpenLoco.Gui.ViewModels;
using System;

namespace OpenLoco.Gui
{
	public class ViewLocator : IDataTemplate
	{
		public Control Build(object? data)
		{
			if (data == null)
			{
				return new TextBlock { Text = "<object passed in was null>" };
			}

			var name = data.GetType().FullName!.Replace("ViewModel", "View");
			var type = Type.GetType(name);

			if (type != null)
			{
				return (Control)Activator.CreateInstance(type)!;
			}

			return new TextBlock { Text = "Not Found: " + name };
		}

		public bool Match(object? data)
			=> data is ViewModelBase;
	}
}
