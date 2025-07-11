using DynamicData;
using Definitions.Database;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Gui.ViewModels
{
	public class ObjectSelectionWindowViewModel : ViewModelBase
	{
		public ObservableCollection<ObjectIndexEntry> ObjectView { get; init; } = [];

		readonly ImmutableList<ObjectIndexEntry> ObjectCache;

		[Reactive]
		public ObjectIndexEntry? SelectedObject { get; set; }

		[Reactive]
		public string? SearchTerm { get; set; }

		public ReactiveCommand<Unit, Unit> ConfirmCommand { get; }
		public ReactiveCommand<Unit, Unit> CancelCommand { get; }

		public ObjectSelectionWindowViewModel()
		{ }

		public ObjectSelectionWindowViewModel(IEnumerable<ObjectIndexEntry> objects)
		{
			ConfirmCommand = ReactiveCommand.Create(() => { });
			CancelCommand = ReactiveCommand.Create(() => { });

			ObjectCache = [.. objects];

			_ = this.WhenAnyValue(o => o.SearchTerm)
				.Throttle(TimeSpan.FromMilliseconds(250))
				.DistinctUntilChanged()
				.Subscribe(_ => UpdateObjectView());

			UpdateObjectView();
		}

		void UpdateObjectView()
		{
			ObjectView.Clear();
			ObjectView.AddRange(ObjectCache.Where(x => string.IsNullOrEmpty(SearchTerm) || (!string.IsNullOrEmpty(x.DisplayName) && x.DisplayName.Contains(SearchTerm, StringComparison.InvariantCultureIgnoreCase))));
		}
	}
}
