using Avalonia.ReactiveUI;
using Gui.ViewModels;
using ReactiveUI;

namespace Gui.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
	public MainWindow()
	{
		InitializeComponent();
		_ = this.WhenActivated(action => action(ViewModel!.OpenEditorSettingsWindow.RegisterHandler(this.DoShowDialogAsync<EditorSettingsWindowViewModel, EditSettingsWindow>)));
		_ = this.WhenActivated(action => action(ViewModel!.OpenLogWindow.RegisterHandler(this.DoShow<LogWindowViewModel, LogWindow>)));
	}
}
