using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class TabViewPageViewModel : ViewModelBase
	{
		public ObservableCollection<ILocoFileViewModel> Documents { get; }

		public void AddDocument(ILocoFileViewModel model)
		{
			var existing = Documents.SingleOrDefault(x => x.CurrentFile.Filename == model.CurrentFile.Filename);
			if (existing != null)
			{
				SelectedDocument = existing;
				return;
			}

			Documents.Add(model);
			SelectedDocument = model;
		}

		[Reactive]
		public ILocoFileViewModel? SelectedDocument { get; set; }

		public ReactiveCommand<ILocoFileViewModel, Unit> RemoveTabCommand { get; }

		public TabViewPageViewModel()
		{
			Documents = [];
			RemoveTabCommand = ReactiveCommand.Create<ILocoFileViewModel>(RemoveTab);
		}

		public async void ReloadAll()
		{
			foreach (var model in Documents)
			{
				_ = await model.ReloadCommand.Execute();
			}
		}

		void RemoveTab(ILocoFileViewModel tabToRemove)
		{
			_ = Documents.Remove(tabToRemove);
		}
	}
}
