using Gui.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;

namespace Gui.Views;

public partial class ObjectSelectionWindow : ReactiveWindow<ObjectSelectionWindowViewModel>
{
	public ObjectSelectionWindow()
	{
		InitializeComponent();
		this.WhenActivated(d =>
		{
			d(ViewModel!.ConfirmCommand.Subscribe(Observer.Create<RxVoid>(_ => Close(ViewModel))));
			d(ViewModel!.CancelCommand.Subscribe(Observer.Create<RxVoid>(_ => Close(null))));
		});
	}
}
