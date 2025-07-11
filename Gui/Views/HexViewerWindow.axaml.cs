using Avalonia.ReactiveUI;
using Gui.ViewModels;

namespace Gui.Views
{
	public partial class HexViewerWindow : ReactiveWindow<HexWindowViewModel>
	{
		public HexViewerWindow()
		{
			InitializeComponent();
		}
	}
}
