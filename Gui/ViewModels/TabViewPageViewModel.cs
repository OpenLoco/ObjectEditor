using Gui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Gui.ViewModels
{
	public class TabViewPageViewModel : ViewModelBase
	{
		public ObservableCollection<ILocoFileViewModel> Documents { get; }

		public bool OpenInNewTab { get; set; }

		public bool OpenInNewTabIsVisible => Documents.Any();

		[Reactive]
		public ILocoFileViewModel? SelectedDocument { get; set; }

		public ReactiveCommand<ILocoFileViewModel, Unit> RemoveTabCommand { get; }

		[Reactive]
		public ICommand CloseAllTabsCommand { get; set; }

		[Reactive]
		public ReactiveCommand<ILocoFileViewModel, Unit> CloseOtherTabsCommand { get; set; }

		public TabViewPageViewModel()
		{
			Documents = [];
			RemoveTabCommand = ReactiveCommand.Create<ILocoFileViewModel>(RemoveDocument);
			CloseAllTabsCommand = ReactiveCommand.Create(ClearDocuments);
			CloseOtherTabsCommand = ReactiveCommand.Create<ILocoFileViewModel>(ClearDocumentsExcept);

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

		public void AddDocument(ILocoFileViewModel model)
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

		void RemoveDocument(ILocoFileViewModel tabToRemove)
			=> _ = Documents.Remove(tabToRemove);

		void ClearDocuments()
			=> Documents.Clear();

		void ClearDocumentsExcept(ILocoFileViewModel tabToKeep)
		{
			Documents.Clear();
			Documents.Add(tabToKeep);
		}

		public bool DocumentExistsWithFile(FileSystemItem fsi)
			=> Documents.Any(x => x.CurrentFile == fsi);
	}
}
