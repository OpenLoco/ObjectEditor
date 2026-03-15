using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Common.Logging;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using DynamicData;
using Gui.Models;
using Gui.Views;
using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class RegionViewModel : BaseViewModel<RegionObject>
{
	readonly ObjectEditorContext? editorContext;
	readonly SourceCache<ObjectModelHeader, uint> dependentObjects = new(x => x.DatChecksum);
	readonly CompositeDisposable dependentObjectsSubscriptions = [];
	readonly ReadOnlyObservableCollection<ObjectModelHeader> dependentObjectsCollection;

	public RegionViewModel(RegionObject model, ObjectEditorContext? editorContext = null)
		: base(model)
	{
		this.editorContext = editorContext;

		_ = dependentObjects.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out dependentObjectsCollection)
			.Subscribe(Observer.Create<IChangeSet<ObjectModelHeader, uint>>(_ => SyncDependentObjectsToModel()))
			.DisposeWith(dependentObjectsSubscriptions);

		dependentObjects.Edit(updater => updater.AddOrUpdate(model.DependentObjects));
		CargoInfluenceObjects = new(model.CargoInfluenceObjects);
		CargoInfluenceTownFilter = new(model.CargoInfluenceTownFilter);

		var hasSelection = this.WhenAnyValue(x => x.SelectedDependentObject).Select(obj => obj != null);

		PopulateDependentObjectsFromFolderCommand = ReactiveCommand.CreateFromTask(PopulateDependentObjectsFromFolder);
		AddDependentObjectCommand = ReactiveCommand.CreateFromTask(AddDependentObjectAsync);
		RemoveSelectedDependentObjectCommand = ReactiveCommand.Create(RemoveSelectedDependentObject, hasSelection);
		CopyDependentObjectsCommand = ReactiveCommand.CreateFromTask(CopyDependentObjectsAsync);
		PasteDependentObjectsCommand = ReactiveCommand.CreateFromTask(PasteDependentObjectsAsync);
		ClearDependentObjectsCommand = ReactiveCommand.Create(ClearDependentObjects);
	}

	public DrivingSide VehiclesDriveOnThe
	{
		get => Model.VehiclesDriveOnThe;
		set => Model.VehiclesDriveOnThe = value;
	}

	public uint8_t pad_07
	{
		get => Model.pad_07;
		set => Model.pad_07 = value;
	}

	[Browsable(false)]
	public ReadOnlyObservableCollection<ObjectModelHeader> DependentObjects
		=> dependentObjectsCollection;

	[Browsable(false)]
	[Reactive]
	public ObjectModelHeader? SelectedDependentObject { get; set; }

	[Category("Cargo")]
	public BindingList<ObjectModelHeader> CargoInfluenceObjects { get; }

	[Category("Cargo")]
	public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PopulateDependentObjectsFromFolderCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> AddDependentObjectCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> RemoveSelectedDependentObjectCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> CopyDependentObjectsCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PasteDependentObjectsCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> ClearDependentObjectsCommand { get; }

	async Task PopulateDependentObjectsFromFolder()
	{
		var folders = await PlatformSpecific.OpenFolderPicker();
		var dir = folders.FirstOrDefault();
		if (dir == null)
		{
			return;
		}

		var dirPath = dir.Path.LocalPath;
		var logger = editorContext?.Logger ?? new Logger();
		var objectIndex = await ObjectIndex.CreateIndexAsync(dirPath, logger);

		var headers = objectIndex.Objects
			  .Where(x => x.DatChecksum.HasValue)
			  .Select(entry => new ObjectModelHeader(entry.DisplayName, entry.ObjectType, entry.ObjectSource, entry.DatChecksum!.Value));

		ReplaceDependentObjects(headers);
	}

	async Task AddDependentObjectAsync()
	{
		var objectIndex = editorContext?.ObjectIndex;
		if (objectIndex == null)
		{
			return;
		}

		if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime app || app.MainWindow == null)
		{
			return;
		}

		var vm = new ObjectSelectionWindowViewModel(objectIndex.Objects);
		var dialog = new ObjectSelectionWindow { DataContext = vm };
		var result = await dialog.ShowDialog<ObjectSelectionWindowViewModel?>(app.MainWindow);

		if (result?.SelectedObject is { DatChecksum: not null } selected)
		{
			dependentObjects.AddOrUpdate(new ObjectModelHeader(selected.DisplayName, selected.ObjectType, selected.ObjectSource, selected.DatChecksum.Value));
		}
	}

	void RemoveSelectedDependentObject()
	{
		if (SelectedDependentObject != null)
		{
			dependentObjects.RemoveKey(SelectedDependentObject.DatChecksum);
		}
	}

	async Task CopyDependentObjectsAsync()
	{
		var json = JsonSerializer.Serialize(DependentObjects, JsonSerializerOptions);
		await PlatformSpecific.SetClipboardTextAsync(json);
	}

	async Task PasteDependentObjectsAsync()
	{
		var text = await PlatformSpecific.GetClipboardTextAsync();
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		List<ObjectModelHeader>? headers;
		try
		{
			headers = JsonSerializer.Deserialize<List<ObjectModelHeader>>(text, JsonSerializerOptions);
		}
		catch (JsonException)
		{
			return;
		}

		if (headers == null)
		{
			return;
		}

		ReplaceDependentObjects(headers);
	}

	void ClearDependentObjects()
		=> dependentObjects.Clear();

	void ReplaceDependentObjects(IEnumerable<ObjectModelHeader> headers)
		=> dependentObjects.Edit(updater =>
		{
			updater.Clear();
			updater.AddOrUpdate(headers);
		});

	void SyncDependentObjectsToModel()
	{
		Model.DependentObjects.Clear();
		Model.DependentObjects.AddRange(DependentObjects);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			dependentObjectsSubscriptions.Dispose();
			dependentObjects.Dispose();
		}

		base.Dispose(disposing);
	}

	static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = false };
}
