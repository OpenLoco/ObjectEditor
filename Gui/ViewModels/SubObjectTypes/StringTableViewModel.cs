using OpenLoco.Dat.Data;
using OpenLoco.Dat.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvaGui.ViewModels
{
	public class StringTableViewModel : ReactiveObject
	{
		public StringTableViewModel(StringTable table)
		{
			OriginalTable = table;
			SelectedKey = table.Table.Keys.FirstOrDefault();
			Keys = new([.. OriginalTable.Table.Keys]);

			_ = this.WhenAnyValue(o => o.SelectedKey)
				.Subscribe(_ => SelectedInnerDictionary = OriginalTable.Table.TryGetValue(SelectedKey, out var innerDict) ? innerDict : null);

			_ = this.WhenAnyValue(o => o.SelectedInnerDictionary)
				.Subscribe(newDict =>
				{
					if (SelectedKey != null && newDict != null)
					{
						OriginalTable.Table[SelectedKey] = newDict;
					}
				});
		}

		StringTable OriginalTable { get; init; }

		[Reactive]
		public Dictionary<LanguageId, string> SelectedInnerDictionary { get; set; }

		[Reactive]
		public string? SelectedKey { get; set; }

		public Collection<string> Keys { get; init; }
	}
	//public void WriteTableBackToObject()
	//{
	//	foreach (var key in Keys)
	//	{
	//		foreach (var t in TranslationTable)
	//		{
	//			OriginalTable[key][t.Language] = t.Translation;
	//		}
	//	}
	//}
}
