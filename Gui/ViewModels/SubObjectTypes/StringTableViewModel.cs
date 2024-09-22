using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AvaGui.ViewModels
{
	public record LanguageTranslationModel(LanguageId Language, string Translation);

	public class StringTableViewModel : ReactiveObject
	{
		public StringTableViewModel(StringTable table)
		{
			OriginalTable = table;
			SelectedKey = table.Table.Keys.FirstOrDefault();
			Keys = table.Table.Keys.ToBindingList();

			TableView = table.Table.ToDictionary(
				x => x.Key,
				x => x.Value.Select(y => new LanguageTranslationModel(y.Key, y.Value)).ToBindingList()
			);

			_ = this.WhenAnyValue(o => o.SelectedKey)
				.Subscribe(_ => SelectedInnerDictionary = TableView[SelectedKey]);
		}

		[Reactive] public Dictionary<string, BindingList<LanguageTranslationModel>> TableView { get; set; }

		[Reactive] public BindingList<LanguageTranslationModel> SelectedInnerDictionary { get; set; }

		[Reactive]
		public string? SelectedKey { get; set; }

		public BindingList<string> Keys { get; init; }

		StringTable OriginalTable { get; init; }

		public void WriteTableBackToObject()
		{
			foreach (var key in TableView)
			{
				foreach (var t in key.Value)
				{
					OriginalTable[key.Key][t.Language] = t.Translation;
				}
			}
		}
	}
}
