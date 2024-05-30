using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace MenuItemCommandBug.ViewModels
{
	public record MenuItemModel(string Name, ICommand MenuCommand);

	public class MainWindowViewModel : ViewModelBase
	{
		public List<MenuItemModel> Items { get; set; }

		public MainWindowViewModel()
			=> Items =
			[
				new("Item1", ReactiveCommand.Create(() => Debug.WriteLine("Item1"))),
				new("Item2", ReactiveCommand.Create(() => Debug.WriteLine("Item2"))),
				new("Item3", ReactiveCommand.Create(() => Debug.WriteLine("Item3"))),
			];
	}
}
