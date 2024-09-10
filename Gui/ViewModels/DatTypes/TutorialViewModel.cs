using AvaGui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaGui.ViewModels
{
	public class TutorialViewModel : ReactiveObject, ILocoFileViewModel
	{
		[Reactive]
		public FileSystemItem CurrentFile { get; init; }
	}
}
