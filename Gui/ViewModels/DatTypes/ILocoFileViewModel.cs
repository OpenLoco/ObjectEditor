using AvaGui.Models;
using ReactiveUI.Fody.Helpers;

namespace AvaGui.ViewModels
{
	public interface ILocoFileViewModel
	{
		[Reactive]
		public FileSystemItem CurrentFile { get; init; }
	}
}
