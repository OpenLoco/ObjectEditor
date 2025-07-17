using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Windows.Input;

namespace Gui.ViewModels;

public class MenuItemViewModel(string name, ICommand menuCommand, string iconName, ICommand deleteCommand) : ReactiveObject
{
	[Reactive] public string Name { get; set; } = name;
	[Reactive] public ICommand MenuCommand { get; set; } = menuCommand;
	[Reactive] public string IconName { get; set; } = iconName;
	[Reactive] public ICommand DeleteCommand { get; set; } = deleteCommand;
}
