using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Gui.Views
{
	public partial class ViewModelGroupView : UserControl
	{
		public ViewModelGroupView()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
