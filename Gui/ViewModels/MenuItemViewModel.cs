using ReactiveUI;
using System.Windows.Input;

namespace Gui.ViewModels;

public class MenuItemViewModel(string name, ICommand menuCommand) : ReactiveObject
{
	public string Name { get; } = name;
	public ICommand MenuCommand { get; } = menuCommand;
}
