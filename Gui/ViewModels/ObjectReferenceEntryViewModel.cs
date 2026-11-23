using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Definitions.ObjectModels.Types;
using Gui.Views;
using Index;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Gui.ViewModels;

/// <summary>
/// Backs the inline <see cref="ObjectReferenceView"/> cell used by the property grid's
/// <c>ObjectReferenceCellEditFactory</c> for any property whose type derives from
/// <see cref="ObjectModelHeaderViewModel"/>. It shows the object a reference currently points at and
/// lets the user pick a replacement (or clear the reference) from the objects known to the editor.
/// </summary>
public class ObjectReferenceEntryViewModel : ViewModelBase
{
	readonly Func<ObjectModelHeaderViewModel?> getReference;
	readonly Type referenceViewModelType;
	readonly Action<ObjectModelHeaderViewModel?> commitNewReference;

	ObjectModelHeaderViewModel? reference;
	bool isManualEditorVisible;

	public ObjectReferenceEntryViewModel(
		Func<ObjectModelHeaderViewModel?> getReference,
		Type referenceViewModelType,
		Action<ObjectModelHeaderViewModel?> commitNewReference)
	{
		ArgumentNullException.ThrowIfNull(getReference);
		ArgumentNullException.ThrowIfNull(referenceViewModelType);
		ArgumentNullException.ThrowIfNull(commitNewReference);

		this.getReference = getReference;
		this.referenceViewModelType = referenceViewModelType;
		this.commitNewReference = commitNewReference;

		PickCommand = ReactiveCommand.CreateFromTask(PickAsync, this.WhenAnyValue(x => x.CanPick));
		ClearCommand = ReactiveCommand.Create(Clear, this.WhenAnyValue(x => x.HasReference));

		Refresh();
	}

	/// <summary>The text shown for the current reference; a placeholder is shown when unset.</summary>
	public string ReferenceName
		=> string.IsNullOrWhiteSpace(reference?.Name) ? "(none)" : reference!.Name;

	/// <summary>The object type of the current reference, if any.</summary>
	public ObjectType? ReferenceObjectType
		=> reference?.ObjectType;

	/// <summary>True when the reference currently points at an object.</summary>
	public bool HasReference
		=> reference is not null && !string.IsNullOrWhiteSpace(reference.Name);

	/// <summary>The reference bound to the manual header editor; null when the reference is unset.</summary>
	public ObjectModelHeaderViewModel? Reference
		=> reference;

	/// <summary>
	/// Whether the manual header editor is shown. Off by default - picking an object from the object
	/// index is the normal way to set a reference; editing the raw header values is the exception.
	/// </summary>
	public bool IsManualEditorVisible
	{
		get => isManualEditorVisible;
		set
		{
			if (isManualEditorVisible == value)
			{
				return;
			}

			isManualEditorVisible = value;

			// There is nothing to edit when no reference exists yet, so create an empty one on demand.
			if (value && reference is null)
			{
				commitNewReference(CreateBlankReference());
			}

			RaisePropertiesChanged(nameof(IsManualEditorVisible));
			Refresh();
		}
	}

	/// <summary>The object types the reference is allowed to point at, derived from its view model type.</summary>
	public IReadOnlySet<ObjectType> AllowedObjectTypes
		=> reference?.AllowedObjectTypes ?? CreatePrototype().AllowedObjectTypes;

	/// <summary>True when the editor knows of an object the reference is allowed to point at.</summary>
	public bool CanPick
		=> GetCandidateObjects().Any();

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PickCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> ClearCommand { get; }

	/// <summary>
	/// Re-reads the underlying reference and raises change notifications. Called by the cell edit
	/// factory whenever the reference is changed outside of this editor.
	/// </summary>
	public void Refresh()
	{
		reference = getReference();

		RaisePropertiesChanged(
			nameof(ReferenceName),
			nameof(ReferenceObjectType),
			nameof(Reference),
			nameof(HasReference),
			nameof(AllowedObjectTypes),
			nameof(CanPick));
	}

	void Clear()
	{
		commitNewReference(null);

		if (isManualEditorVisible)
		{
			isManualEditorVisible = false;
			RaisePropertiesChanged(nameof(IsManualEditorVisible));
		}

		Refresh();
	}

	async Task PickAsync()
	{
		var owner = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime app
			? app.MainWindow
			: null;

		if (owner is null)
		{
			return;
		}

		var dialogViewModel = new ObjectSelectionWindowViewModel(GetCandidateObjects());
		var dialog = new ObjectSelectionWindow { DataContext = dialogViewModel };

		var result = await dialog.ShowDialog<ObjectSelectionWindowViewModel?>(owner);
		if (result?.SelectedObject is not { DatChecksum: not null } selected)
		{
			return;
		}

		var header = new ObjectModelHeader(selected.DisplayName, selected.ObjectType, selected.ObjectSource, selected.DatChecksum.Value);
		var newReference = (ObjectModelHeaderViewModel)Activator.CreateInstance(referenceViewModelType, header)!;

		commitNewReference(newReference);
		Refresh();
	}

	IEnumerable<ObjectIndexEntry> GetCandidateObjects()
	{
		var objectIndex = TryGetObjectIndex();
		if (objectIndex is null)
		{
			return [];
		}

		var allowed = AllowedObjectTypes;
		return objectIndex.Objects
			.Where(entry => entry.DatChecksum.HasValue && allowed.Contains(entry.ObjectType))
			.OrderBy(entry => entry.DisplayName, StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	ObjectModelHeaderViewModel CreatePrototype()
		=> (ObjectModelHeaderViewModel)Activator.CreateInstance(referenceViewModelType)!;

	ObjectModelHeaderViewModel CreateBlankReference()
	{
		var created = CreatePrototype();
		created.Name = string.Empty;
		return created;
	}

	static ObjectIndex? TryGetObjectIndex()
	{
		foreach (var window in App.GetOpenWindows())
		{
			if (window.DataContext is MainWindowViewModel mainWindowViewModel)
			{
				return mainWindowViewModel.EditorContext.ObjectIndex;
			}
		}

		return null;
	}

	void RaisePropertiesChanged(params string[] propertyNames)
	{
		foreach (var propertyName in propertyNames)
		{
			this.RaisePropertyChanged(propertyName);
		}
	}
}
