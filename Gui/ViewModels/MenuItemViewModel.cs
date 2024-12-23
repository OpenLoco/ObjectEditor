using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;

namespace OpenLoco.Gui.ViewModels
{
	public class MenuItemViewModel(string name, ICommand menuCommand/*, ICommand deleteCommand*/) : ReactiveObject
	{
		[Reactive] public string Name { get; set; } = name;
		[Reactive] public ICommand MenuCommand { get; set; } = menuCommand;
		//[Reactive] public ICommand DeleteCommand { get; set; } = deleteCommand;
	}
}
