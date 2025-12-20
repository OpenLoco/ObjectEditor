using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Gui.ViewModels;

public class TabViewPageViewModel : ViewModelBase
{
	public ObservableCollection<IFileViewModel> Documents { get; }

	public bool OpenInNewTab { get; set; }

	public bool OpenInNewTabIsVisible => Documents.Any();

	[Reactive]
	public IFileViewModel? SelectedDocument { get; set; }

	public ReactiveCommand<IFileViewModel, Unit> RemoveTabCommand { get; }

	[Reactive]
	public ICommand CloseAllTabsCommand { get; set; }

	[Reactive]
	public ReactiveCommand<IFileViewModel, Unit> CloseOtherTabsCommand { get; set; }

	public TabViewPageViewModel()
	{
		Documents = [];
		RemoveTabCommand = ReactiveCommand.Create<IFileViewModel>(RemoveDocument);
		CloseAllTabsCommand = ReactiveCommand.Create(ClearDocuments);
		CloseOtherTabsCommand = ReactiveCommand.Create<IFileViewModel>(ClearDocumentsExcept);

		_ = this.WhenAnyValue(o => o.SelectedDocument)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(OpenInNewTabIsVisible)));
	}

	public async void ReloadAll()
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
		=> _ = Documents.Remove(tabToRemove);

	void ClearDocuments()
		=> Documents.Clear();

	void ClearDocumentsExcept(IFileViewModel tabToKeep)
	{
		Documents.Clear();
		Documents.Add(tabToKeep);
	}

	public bool DocumentExistsWithFile(FileSystemItem fsi)
		=> Documents.Any(x => x.CurrentFile == fsi);
}
