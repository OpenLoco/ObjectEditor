using AvaGui.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaGui.ViewModels
{
	public class G1ViewModel : ReactiveObject, ILocoFileViewModel
	{
		[Reactive]
		public FileSystemItem CurrentFile { get; init; }
	}
}
