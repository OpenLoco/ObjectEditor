using Avalonia.ReactiveUI;
using OpenLoco.Gui.ViewModels;

namespace OpenLoco.Gui.Views
{
	public partial class HexViewerWindow : ReactiveWindow<HexWindowViewModel>
	{
		public HexViewerWindow()
		{
			InitializeComponent();
		}
	}
}
