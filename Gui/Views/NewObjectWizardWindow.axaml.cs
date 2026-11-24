using Gui.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System.Reactive;

namespace Gui.Views;

public partial class NewObjectWizardWindow : ReactiveWindow<NewObjectWizardViewModel>
{
	public NewObjectWizardWindow()
	{
		InitializeComponent();
		_ = this.WhenActivated(d =>
		{
			d(ViewModel!.CreateCommand.Subscribe(Observer.Create<Unit>(_ => Close(ViewModel))));
			d(ViewModel!.CancelCommand.Subscribe(Observer.Create<Unit>(_ => Close(null))));
		});
	}
}
