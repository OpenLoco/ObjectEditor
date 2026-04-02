using Gui.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace Gui.Views;

public partial class MainView : ReactiveUserControl<MainWindowViewModel>
{
	public MainView()
	{
		InitializeComponent();
	}
}
