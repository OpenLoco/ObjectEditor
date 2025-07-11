using Avalonia.ReactiveUI;
using Gui.ViewModels;

namespace Gui.Views
{
	public partial class LogWindow : ReactiveWindow<LogWindowViewModel>
	{
		public LogWindow()
		{
			InitializeComponent();
		}
	}
}
