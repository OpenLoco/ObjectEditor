using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;

namespace Gui.ViewModels;

public class LanguageTranslationModel(LanguageId language, string translation) : ReactiveObject
{
	public LanguageId Language { get; init; } = language;

	[Reactive]
	public string Translation { get; set; } = translation;
}

public class StringTableViewModel : ReactiveObject, IViewModel, IDisposable
{
	readonly CompositeDisposable subscriptions = [];
	BindingList<LanguageTranslationModel>? subscribedInnerDictionary;
	bool disposed;

	public string DisplayName
		=> "String Table";

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
			.Subscribe(_ => SelectedInnerDictionary = SelectedKey == null ? null : TableView[SelectedKey])
			.DisposeWith(subscriptions);

		_ = this.WhenAnyValue(o => o.SelectedInnerDictionary)
			.Subscribe(SubscribeToSelectedInnerDictionary)
			.DisposeWith(subscriptions);
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

	public void Dispose()
	{
		if (disposed)
		{
			return;
		}

		disposed = true;
		subscribedInnerDictionary?.ListChanged -= OnSelectedInnerDictionaryChanged;
		subscriptions.Dispose();
		GC.SuppressFinalize(this);
	}
}
