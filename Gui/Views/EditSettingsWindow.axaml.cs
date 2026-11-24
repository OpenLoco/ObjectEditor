using Avalonia.Controls;
using Gui.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;
using System;
using System.Reactive.Linq;

namespace Gui.Views;

public partial class EditSettingsWindow : ReactiveWindow<EditorSettingsWindowViewModel>
{
	public EditSettingsWindow()
	{
		InitializeComponent();
		_ = this.WhenActivated(d => d(
			Observable.FromEventPattern<WindowClosingEventArgs>(h => Closing += h, h => Closing -= h)
				.Subscribe(_ => ViewModel?.Commit())));
	}
}
