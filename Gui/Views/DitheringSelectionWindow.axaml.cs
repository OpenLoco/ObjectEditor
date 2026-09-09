using Gui.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;

namespace Gui.Views;

public partial class DitheringSelectionWindow : ReactiveWindow<DitheringSelectionWindowViewModel>
{
	public DitheringSelectionWindow()
	{
		InitializeComponent();
		_ = this.WhenActivated(d =>
		{
			d(ViewModel!.ConfirmCommand.Subscribe(Observer.Create<System.Reactive.Unit>(_ => Close(ViewModel))));
			d(ViewModel!.CancelCommand.Subscribe(Observer.Create<System.Reactive.Unit>(_ => Close(null))));
		});
	}
}