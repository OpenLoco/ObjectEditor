using Definitions.ObjectModels.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

/// <summary>
/// A <see cref="BindingList{T}"/> of typed object reference view models (<typeparamref name="TViewModel"/>)
/// that keeps the object model's list of <see cref="ObjectModelHeader"/>s in sync. The type parameter
/// carries the reference type restriction (eg <see cref="CargoReferenceViewModel"/>) to every place the
/// collection is used, and any structural change made through the property grid (add / insert / remove /
/// clear) is mirrored straight back onto the object model.
/// </summary>
public sealed class ObjectReferenceList<TViewModel> : BindingList<TViewModel>
	where TViewModel : ObjectModelHeaderViewModel
{
	readonly Action<IReadOnlyList<ObjectModelHeader>> syncToModel;
	bool synchronizing;

	public ObjectReferenceList(
		IEnumerable<ObjectModelHeader> initialItems,
		Func<ObjectModelHeader, TViewModel> factory,
		Action<IReadOnlyList<ObjectModelHeader>> syncToModel)
		: base([.. initialItems.Select(factory)])
	{
		this.syncToModel = syncToModel;
		ListChanged += OnListChanged;
	}

	void OnListChanged(object? sender, ListChangedEventArgs e)
		=> Synchronize();

	/// <summary>
	/// Writes the current view models' underlying headers back to the object model. Called automatically
	/// whenever the collection changes, and again from <see cref="BaseViewModel.SynchronizeToModel"/>.
	/// </summary>
	public void Synchronize()
	{
		// The handler is also invoked for value changes on the items; re-entrancy is impossible here as
		// we only write to the object model, but guard anyway so a model setter that raises the event
		// cannot recursively re-enter.
		if (synchronizing)
		{
			return;
		}

		synchronizing = true;
		try
		{
			syncToModel([.. this.Select(x => x.Model)]);
		}
		finally
		{
			synchronizing = false;
		}
	}
}
