using Definitions.ObjectModels;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Gui.ViewModels;

public abstract class LocoObjectViewModel<T> : ReactiveObject, IObjectViewModel<T> where T : class, ILocoStruct
{
	[Browsable(false)]
	[JsonIgnore]
	public T Model { get; init; } = null!;

	protected LocoObjectViewModel(T model)
	{
		ArgumentNullException.ThrowIfNull(model);
		Model = model;
	}
}
