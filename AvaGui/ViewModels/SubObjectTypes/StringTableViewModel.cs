using ReactiveUI;
using System;
using OpenLoco.Dat.Types;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace AvaGui.ViewModels
{
	public class StringTableViewModel : ViewModelBase
	{
		public StringTableViewModel(StringTable table)
		{
			OriginalTable = table;
			SelectedKey = table.Table.Keys.FirstOrDefault();

			_ = this.WhenAnyValue(o => o.Keys)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(SelectedKey)));
			_ = this.WhenAnyValue(o => o.SelectedKey)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(TranslationTable)));
		}

		StringTable OriginalTable { get; init; }

		[Reactive]
		public string? SelectedKey { get; set; }

		public ObservableCollection<string> Keys => new(OriginalTable.Table.Keys);

		public ObservableCollection<LanguageTranslation>? TranslationTable
			=> SelectedKey == null ? null : new(OriginalTable.Table[SelectedKey].Select(kvp => new LanguageTranslation(kvp.Key, kvp.Value)));
	}
}
