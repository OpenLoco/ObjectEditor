using Avalonia.ReactiveUI;
using OpenLoco.Gui.ViewModels;
using ReactiveUI;
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

		async Task DoShowDialogAsync(IInteractionContext<EditorSettingsWindowViewModel, EditorSettingsWindowViewModel?> interaction)
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
