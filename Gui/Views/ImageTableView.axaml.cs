using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;

namespace OpenLoco.Gui.Views
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
			switch (e.Key)
			{
				case Key.R:
					ZoomBorder?.ResetMatrix();
					break;
			}
		}
	}
}
