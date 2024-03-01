namespace AvaGui.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public string Greeting => "Welcome to Avalonia!";

		public ObjectEditorViewModel ObjectEditorViewModel { get; } = new ObjectEditorViewModel();
	}
}
