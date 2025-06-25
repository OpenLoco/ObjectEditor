using Avalonia.ReactiveUI;
using OpenLoco.Gui.ViewModels;

namespace OpenLoco.Gui.Views
{
	public partial class LogWindow : ReactiveWindow<LogWindowViewModel>
	{
		public LogWindow()
		{
			InitializeComponent();
		}
	}
}
