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

			//var vm = new MainWindowViewModel();
			//vm.Menu
			//DataContext = vm;
		}
	}
}
