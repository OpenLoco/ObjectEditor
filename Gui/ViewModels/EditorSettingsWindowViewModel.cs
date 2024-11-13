using ReactiveUI.Fody.Helpers;

namespace OpenLoco.Gui.ViewModels
{
	public class EditorSettingsWindowViewModel : ViewModelBase
	{
		[Reactive]
		public EditorSettings Settings { get; set; }

	}
}
