using Avalonia.Controls;
using OpenLoco.Gui.ViewModels;

namespace OpenLoco.Gui.Views
{
	public partial class EditSettingsWindow : Window
	{
		public EditSettingsWindow(EditorSettings settings)
		{
			InitializeComponent();
			DataContext = new EditorSettingsWindowViewModel() { Settings = settings };
		}
	}
}
