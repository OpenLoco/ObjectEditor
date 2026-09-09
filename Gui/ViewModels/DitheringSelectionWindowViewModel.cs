using Definitions.ObjectModels.Graphics.Dithering;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Gui.ViewModels;

public class DitheringSelectionWindowViewModel : ViewModelBase
{
	/// <summary>The dithering method selected by the user.</summary>
	[Reactive]
	public DitheringMethod SelectedMethod { get; set; } = DitheringMethod.FloydSteinberg;

	/// <summary>When true, the selected method is saved as the default in editor settings.</summary>
	[Reactive]
	public bool SetAsDefault { get; set; }

	/// <summary>True if the user confirmed the dialog (pressed OK).</summary>
	[Reactive]
	public bool Confirmed { get; private set; }

	public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ConfirmCommand { get; }
	public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> CancelCommand { get; }

	public DitheringSelectionWindowViewModel() : this(null) { }

	public DitheringSelectionWindowViewModel(DitheringMethod? defaultMethod)
	{
		if (defaultMethod.HasValue)
		{
			SelectedMethod = defaultMethod.Value;
		}

		ConfirmCommand = ReactiveCommand.Create(() =>
		{
			Confirmed = true;
		});

		CancelCommand = ReactiveCommand.Create(() =>
		{
			Confirmed = false;
		});
	}
}
