using Avalonia.Controls;
using AvaGui.Models;

namespace AvaGui.Views
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Title = ObjectEditorModel.ApplicationName;
		}
	}
}
