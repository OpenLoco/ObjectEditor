using DynamicData.Binding;
using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Gui.ViewModels;

public class TabViewPageViewModel : ViewModelBase
{
	readonly ObservableAsPropertyHelper<bool> openInNewTabIsVisible;

	public ObservableCollection<IFileViewModel> Documents { get; }

	[Reactive]
	public bool OpenInNewTab { get; set; }

	public bool OpenInNewTabIsVisible => openInNewTabIsVisible.Value;

	[Reactive]
	public IFileViewModel? SelectedDocument { get; set; }

	public ReactiveCommand<IFileViewModel, Unit> RemoveTabCommand { get; }

	public ReactiveCommand<Unit, Unit> CloseAllTabsCommand { get; }

	public ReactiveCommand<IFileViewModel, Unit> CloseOtherTabsCommand { get; }

	public TabViewPageViewModel()
	{
		Documents = [];
		RemoveTabCommand = ReactiveCommand.Create<IFileViewModel>(RemoveDocument);
		CloseAllTabsCommand = ReactiveCommand.Create(ClearDocuments);
		CloseOtherTabsCommand = ReactiveCommand.Create<IFileViewModel>(ClearDocumentsExcept);

		openInNewTabIsVisible = Documents.ToObservableChangeSet()
			.Select(_ => Documents.Count > 0)
			.ToProperty(this, o => o.OpenInNewTabIsVisible);
	}

	public async Task ReloadAllAsync()
	{
		foreach (var model in Documents)
		{
			_ = await model.ReloadCommand.Execute();
		}
	}

	public void AddDocument(IFileViewModel model)
	{
		if (OpenInNewTab)
		{
			var existing = Documents.SingleOrDefault(x => x.CurrentFile.FileName == model.CurrentFile.FileName);
			if (existing != null)
			{
				SelectedDocument = existing;
				return;
			}
		}
		else
		{
			ClearDocuments();
		}

		Documents.Add(model);
		SelectedDocument = model;
	}

	void RemoveDocument(IFileViewModel tabToRemove)
	{
		DisposeDocument(tabToRemove);
		_ = Documents.Remove(tabToRemove);
	}

	void ClearDocuments()
	{
		foreach (var document in Documents.ToList())
		{
			DisposeDocument(document);
		}

		Documents.Clear();
	}

	void ClearDocumentsExcept(IFileViewModel tabToKeep)
	{
		foreach (var document in Documents.Where(document => !ReferenceEquals(document, tabToKeep)).ToList())
		{
			DisposeDocument(document);
		}

		Documents.Clear();
		Documents.Add(tabToKeep);
	}

	static void DisposeDocument(IFileViewModel model)
		=> (model as IDisposable)?.Dispose();

	public bool DocumentExistsWithFile(FileSystemItem fsi)
		=> Documents.Any(x => x.CurrentFile == fsi);
}
