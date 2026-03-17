using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Gui.ViewModels;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Gui;

public class ViewLocator : IDataTemplate
{
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2057", Justification = "ViewLocator uses Type.GetType() to resolve view types by convention from ViewModel type names. All View types must be preserved via TrimmerRootDescriptor or explicit references.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2072", Justification = "Activator.CreateInstance creates view types resolved via Type.GetType. Callers must ensure view types are preserved.")]
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
