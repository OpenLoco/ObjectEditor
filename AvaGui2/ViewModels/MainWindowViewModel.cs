namespace AvaGui2.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public string Greeting => "Welcome to Avalonia!";

		public DictionaryViewModel DictionaryEditor { get; } = new DictionaryViewModel();
	}
}
