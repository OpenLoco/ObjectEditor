using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using System.Collections.Generic;

namespace AvaGui2.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public string Greeting => "Welcome to Avalonia!";

		public DictionaryViewModel DictionaryEditor { get; } = new DictionaryViewModel();
	}
}
