using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Common.Logging;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using Gui.Models;
using Gui.Views;
using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class RegionViewModel : BaseViewModel<RegionObject>
{
	readonly ObjectEditorContext? editorContext;

	public RegionViewModel(RegionObject model, ObjectEditorContext? editorContext = null)
		: base(model)
	{
		this.editorContext = editorContext;

		DependentObjects = new BindingList<ObjectModelHeader>(model.DependentObjects);
		CargoInfluenceObjects = new BindingList<ObjectModelHeader>(model.CargoInfluenceObjects);
		CargoInfluenceTownFilter = new BindingList<CargoInfluenceTownFilterType>(model.CargoInfluenceTownFilter);

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
	public BindingList<ObjectModelHeader> DependentObjects { get; }

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

		DependentObjects.Clear();
		foreach (var entry in objectIndex.Objects.Where(x => x.DatChecksum.HasValue))
		{
			DependentObjects.Add(new ObjectModelHeader(entry.DisplayName, entry.ObjectType, entry.ObjectSource, entry.DatChecksum!.Value));
		}
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
			DependentObjects.Add(new ObjectModelHeader(selected.DisplayName, selected.ObjectType, selected.ObjectSource, selected.DatChecksum.Value));
		}
	}

	void RemoveSelectedDependentObject()
	{
		if (SelectedDependentObject != null)
		{
			DependentObjects.Remove(SelectedDependentObject);
		}
	}

	async Task CopyDependentObjectsAsync()
	{
		var json = JsonSerializer.Serialize(DependentObjects.ToList(), JsonSerializerOptions);
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

		DependentObjects.Clear();
		foreach (var header in headers)
		{
			DependentObjects.Add(header);
		}
	}

	void ClearDependentObjects()
		=> DependentObjects.Clear();

	static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = false };
}
