using Avalonia.Controls;
using ReactiveUI;
using System.Threading.Tasks;

namespace Gui.Views;

public static class WindowInteractionExtensions
{
	public static async Task DoShowDialogAsync<TViewModel, TWindow>(this Window owner, IInteractionContext<TViewModel, TViewModel?> interaction)
				where TWindow : Window, new()
	{
		var dialog = new TWindow
		{
			DataContext = interaction.Input
		};

		var result = await dialog.ShowDialog<TViewModel?>(owner);
		interaction.SetOutput(result);
	}

	public static void DoShow<TViewModel, TWindow>(this Window owner, IInteractionContext<TViewModel, TViewModel?> interaction)
		where TWindow : Window, new()
	{
		var dialog = new TWindow
		{
			DataContext = interaction.Input
		};

		dialog.Show(owner);
		interaction.SetOutput(interaction.Input); // this is necessary since otherwise the UI crashes :|
	}
}
