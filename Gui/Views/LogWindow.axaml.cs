using Gui.ViewModels;
using ReactiveUI.Avalonia;

namespace Gui.Views;

public partial class LogWindow : ReactiveWindow<LogWindowViewModel>
{
	public LogWindow()
	{
		InitializeComponent();
	}
}
