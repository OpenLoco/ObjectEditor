using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace OpenLoco.Gui.ViewModels
{
	public class TabViewPageViewModel : ViewModelBase
	{
		public ObservableCollection<ILocoFileViewModel> Documents { get; }

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
			//if (tabToRemove is DatObjectEditorViewModel dvm && dvm.ExtraContentViewModel is ImageTableViewModel itvm)
			//{
			//	itvm.Dispose();
			//}
			_ = Documents.Remove(tabToRemove);
		}
	}
}
