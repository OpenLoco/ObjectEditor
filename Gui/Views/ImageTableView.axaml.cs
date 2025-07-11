using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;

namespace Gui.Views
{
	public partial class ImageTableView : UserControl
	{
		public ImageTableView()
		{
			InitializeComponent();
			ZoomBorder = this.Find<ZoomBorder>("ZoomBorder");
			if (ZoomBorder != null)
			{
				ZoomBorder.KeyDown += ZoomBorder_KeyDown;
			}
		}

		void ZoomBorder_KeyDown(object? sender, KeyEventArgs e)
		{
			ZoomBorder = this.Find<ZoomBorder>("ZoomBorder");
#pragma warning disable IDE0010 // Add missing cases
			switch (e.Key)
			{
				case Key.R:
					ZoomBorder?.ResetMatrix();
					break;
				default:
					break;
			}
#pragma warning restore IDE0010 // Add missing cases
		}
	}
}
