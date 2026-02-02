using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Gui.ViewModels;

public class LanguageTranslationModel : ReactiveObject
{
	public LanguageTranslationModel(LanguageId language, string translation)
	{
		Language = language;
		Translation = translation;
	}

	public LanguageId Language { get; init; }

	[Reactive]
	public string Translation { get; set; }
}

public class StringTableViewModel : ReactiveObject, IViewModel
{
	public string DisplayName
		=> "String Table";

	BindingList<LanguageTranslationModel>? subscribedInnerDictionary;

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
			.Subscribe(_ => SelectedInnerDictionary = SelectedKey == null ? null : TableView[SelectedKey]);

		_ = this.WhenAnyValue(o => o.SelectedInnerDictionary)
			.Subscribe(SubscribeToSelectedInnerDictionary);
	}

	[Reactive]
	public Dictionary<string, BindingList<LanguageTranslationModel>> TableView { get; set; }

	[Reactive]
	public BindingList<LanguageTranslationModel>? SelectedInnerDictionary { get; set; }

	[Reactive]
	public string? SelectedKey { get; set; }

	[Reactive]
	public BindingList<string> Keys { get; init; }

	StringTable OriginalTable { get; init; }

	void SubscribeToSelectedInnerDictionary(BindingList<LanguageTranslationModel>? innerDictionary)
	{
		if (subscribedInnerDictionary != null)
		{
			subscribedInnerDictionary.ListChanged -= OnSelectedInnerDictionaryChanged;
		}

		subscribedInnerDictionary = innerDictionary;

		if (subscribedInnerDictionary != null)
		{
			subscribedInnerDictionary.ListChanged += OnSelectedInnerDictionaryChanged;
			UpdateSelectedInnerDictionaryInModel();
		}
	}

	void OnSelectedInnerDictionaryChanged(object? sender, ListChangedEventArgs e)
		=> UpdateSelectedInnerDictionaryInModel();

	void UpdateSelectedInnerDictionaryInModel()
	{
		if (SelectedKey == null || SelectedInnerDictionary == null)
		{
			return;
		}

		var target = OriginalTable[SelectedKey];
		target.Clear();

		foreach (var translation in SelectedInnerDictionary)
		{
			target[translation.Language] = translation.Translation;
		}
	}

}
