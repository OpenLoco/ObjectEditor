using ReactiveUI;
using System;

namespace Gui.ViewModels;

public abstract class LocoObjectViewModel<T> : ReactiveObject, IObjectViewModel<T> where T : class
{
	public T Model { get; init; } = null!;

	protected LocoObjectViewModel(T model)
	{
		ArgumentNullException.ThrowIfNull(model);
		Model = model;
	}

	public virtual void CopyBackToModel()
	{ }

}
