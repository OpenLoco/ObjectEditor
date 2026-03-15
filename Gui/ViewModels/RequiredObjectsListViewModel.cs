using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Common.Logging;
using Definitions.ObjectModels.Types;
using DynamicData;
using Gui.Models;
using Gui.Views;
using Index;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
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

public class RequiredObjectsListViewModel : ReactiveObject, IDisposable
{
	readonly ObjectEditorContext? editorContext;
	readonly SourceCache<ObjectModelHeader, uint> sourceCache = new(x => x.DatChecksum);
	readonly CompositeDisposable subscriptions = [];
	readonly ReadOnlyObservableCollection<ObjectModelHeader> itemsCollection;
	bool disposed;

	public RequiredObjectsListViewModel(ObjectEditorContext? editorContext = null)
	{
		this.editorContext = editorContext;

		_ = sourceCache.Connect()
			.ObserveOn(RxApp.MainThreadScheduler)
			.Bind(out itemsCollection)
			.Subscribe()
			.DisposeWith(subscriptions);

		var hasSelection = this.WhenAnyValue(x => x.SelectedItem).Select(obj => obj != null);

		AddItemCommand = ReactiveCommand.CreateFromTask(AddItemAsync);
		RemoveSelectedItemCommand = ReactiveCommand.Create(RemoveSelectedItem, hasSelection);
		PopulateFromFolderCommand = ReactiveCommand.CreateFromTask(PopulateFromFolderAsync);
		CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
		PasteCommand = ReactiveCommand.CreateFromTask(PasteAsync);
		ClearCommand = ReactiveCommand.Create(ClearItems);
	}

	[Browsable(false)]
	public ReadOnlyObservableCollection<ObjectModelHeader> Items => itemsCollection;

	[Browsable(false)]
	[Reactive]
	public ObjectModelHeader? SelectedItem { get; set; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> AddItemCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> RemoveSelectedItemCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PopulateFromFolderCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> CopyCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> PasteCommand { get; }

	[Browsable(false)]
	public ReactiveCommand<Unit, Unit> ClearCommand { get; }

	public IObservable<IChangeSet<ObjectModelHeader, uint>> Connect()
		=> sourceCache.Connect();

	public void AddOrUpdate(IEnumerable<ObjectModelHeader> headers)
		=> sourceCache.Edit(updater => updater.AddOrUpdate(headers));

	public void Replace(IEnumerable<ObjectModelHeader> headers)
		=> sourceCache.Edit(updater =>
		{
			updater.Clear();
			updater.AddOrUpdate(headers);
		});

	async Task AddItemAsync()
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
			sourceCache.AddOrUpdate(new ObjectModelHeader(selected.DisplayName, selected.ObjectType, selected.ObjectSource, selected.DatChecksum.Value));
		}
	}

	void RemoveSelectedItem()
	{
		if (SelectedItem != null)
		{
			sourceCache.RemoveKey(SelectedItem.DatChecksum);
		}
	}

	async Task PopulateFromFolderAsync()
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
			.Select(entry => new ObjectModelHeader(entry.DisplayName, entry.ObjectType, entry.ObjectSource, entry.DatChecksum.Value));

		Replace(headers);
	}

	async Task CopyAsync()
	{
		var json = JsonSerializer.Serialize(Items.ToList(), JsonSerializerOptions);
		await PlatformSpecific.SetClipboardTextAsync(json);
	}

	async Task PasteAsync()
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

		Replace(headers);
	}

	void ClearItems()
		=> sourceCache.Clear();

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposed)
		{
			return;
		}

		if (disposing)
		{
			subscriptions.Dispose();
			sourceCache.Dispose();
		}

		disposed = true;
	}

	static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = false };
}
