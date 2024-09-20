using AvaGui.Models;
using OpenLoco.Dat.Types.SCV5;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace AvaGui.ViewModels
{
	public class SCV5ViewModel : ReactiveObject, ILocoFileViewModel
	{
		public SCV5ViewModel()
			=> _ = this.WhenAnyValue(o => o.CurrentFile)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentS5File)));

		[Reactive]
		public S5File CurrentS5File { get; set; }

		[Reactive]
		public FileSystemItem CurrentFile { get; init; }
	}
}
