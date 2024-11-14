using Avalonia.ReactiveUI;
using Microsoft.VisualBasic;
using OpenLoco.Gui.ViewModels;
using ReactiveUI;
using System;
using System.Threading.Tasks;

namespace OpenLoco.Gui.Views
{
	public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
	{
		public MainWindow()
		{
			InitializeComponent();
			_ = this.WhenActivated(action => action(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
		}

		private async Task DoShowDialogAsync(IInteractionContext<EditorSettingsWindowViewModel, EditorSettingsWindowViewModel?> interaction)
		{
			var dialog = new EditSettingsWindow
			{
				DataContext = interaction.Input
			};

			var result = await dialog.ShowDialog<EditorSettingsWindowViewModel?>(this);
			interaction.SetOutput(result);
		}
	}
}
