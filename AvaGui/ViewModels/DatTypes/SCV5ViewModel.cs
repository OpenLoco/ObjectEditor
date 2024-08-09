using Core.Types.SCV5;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace AvaGui.ViewModels
{
	public class SCV5ViewModel : ReactiveObject, ILocoFileViewModel
	{
		public SCV5ViewModel()
		{
			_ = this.WhenAnyValue(o => o.CurrentFileName)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentFile)));
		}

		[Reactive] public string CurrentFileName { get; set; }
		[Reactive] public S5File CurrentFile { get; set; }
	}
}
