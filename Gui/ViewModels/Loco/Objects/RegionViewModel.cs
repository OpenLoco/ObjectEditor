using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;
using Gui.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
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

		PopulateDependentObjectsFromFolderCommand = ReactiveCommand.CreateFromTask(PopulateDependentObjectsFromFolder);
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

	[Category("Cargo")]
	public BindingList<ObjectModelHeader> CargoInfluenceObjects { get; }

	[Category("Cargo")]
	public BindingList<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PopulateDependentObjectsFromFolderCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> CopyDependentObjectsCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PasteDependentObjectsCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> ClearDependentObjectsCommand { get; }

	Task PopulateDependentObjectsFromFolder()
	{
		var objectIndex = editorContext?.ObjectIndex;
		if (objectIndex == null)
		{
			return Task.CompletedTask;
		}

		DependentObjects.Clear();
		foreach (var entry in objectIndex.Objects.Where(x => x.DatChecksum.HasValue))
		{
			DependentObjects.Add(new ObjectModelHeader(entry.DisplayName, entry.ObjectType, entry.ObjectSource, entry.DatChecksum!.Value));
		}

		return Task.CompletedTask;
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
