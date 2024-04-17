using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.Types;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System;

namespace AvaGui2.ViewModels
{
	public class LanguageRow : ReactiveObject
	{
		public LanguageRow(LanguageId language, string translation)
		{
			_language = language;
			_translation = translation;
		}

		private LanguageId _language;
		public LanguageId Language
		{
			get => _language;
			set => _ = this.RaiseAndSetIfChanged(ref _language, value);
		}

		private string _translation;

		public string Translation
		{
			get => _translation;
			set => _ = this.RaiseAndSetIfChanged(ref _translation, value);
		}
	}

	public class DictionaryViewModel : ReactiveObject
	{
		public ReactiveCommand<Unit, Unit> AddItemCommand { get; }
		public ReactiveCommand<Unit, Unit> RemoveItemCommand { get; }

		ObservableCollection<string> _strings;
		public ObservableCollection<string> Strings
		{
			get => _strings;
			set => this.RaiseAndSetIfChanged(ref _strings, value);
		}

		private string? _selectedString;
		public string? SelectedString
		{
			get => _selectedString;
			set => _ = this.RaiseAndSetIfChanged(ref _selectedString, value);
		}

		private string? _selectedlanguage;
		public string? SelectedLanguage
		{
			get => _selectedlanguage;
			set => _ = this.RaiseAndSetIfChanged(ref _selectedlanguage, value);
		}

		ObservableCollection<string> _selectedLanguages;
		public ObservableCollection<string> SelectedLanguages
		{
			get => _selectedLanguages;
			set => this.RaiseAndSetIfChanged(ref _selectedLanguages, value);
		}

		ObservableCollection<string> _selectedTexts;
		public ObservableCollection<string> SelectedTexts
		{
			get => _selectedTexts;
			set => this.RaiseAndSetIfChanged(ref _selectedTexts, value);
		}

		private string? _selectedText;
		public string? SelectedText
		{
			get => _selectedText;
			set => _ = this.RaiseAndSetIfChanged(ref _selectedText, value);
		}

		public StringTable Table { get; set; }

		public void SelectedStringChanged()
		{
			if (SelectedString != null && Table.Table.ContainsKey(SelectedString))
			{
				SelectedLanguages = new ObservableCollection<string>();
				SelectedTexts = new ObservableCollection<string>();
				foreach (var x in Table.Table[SelectedString])
				{
					SelectedLanguages.Add(x.Key.ToString());
					SelectedTexts.Add(x.Value);
				}
			}
		}
		public void UpdateTranslation()
		{

			if (SelectedString != null
				&& SelectedLanguage != null
				&& Table.Table.ContainsKey(SelectedString))
			{
				if (Enum.TryParse<LanguageId>(SelectedLanguage, out var lang) && Table.Table[SelectedString].ContainsKey(lang))
				{
					Table.Table[SelectedString][lang] = SelectedText;
				}
			}
		}

		public DictionaryViewModel()
		{
			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(o => SelectedStringChanged());

			_ = this.WhenAnyValue(o => o.SelectedText)
				.Subscribe(o => UpdateTranslation());

			Table = new StringTable();
			Table.Table.Add("Name", new Dictionary<LanguageId, string>() { { LanguageId.English_UK, "John Doe" }, { LanguageId.German, "Jane Doe" } });
			Table.Table.Add("Cargo", new Dictionary<LanguageId, string>() { { LanguageId.English_UK, "Water" }, { LanguageId.German, "Beer" } });

			//AddItemCommand = ReactiveCommand.Create(AddNewListItem);
			//RemoveItemCommand = ReactiveCommand.Create(RemoveSelectedListItem);

			_strings = new ObservableCollection<string>(Table.Table.Keys.Select(x => x));
			//Data.CollectionChanged += Data_CollectionChanged;

		}

		//private void Data_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		//	=> this.RaisePropertyChanged(nameof(Sum));

		//public void AddNewListItem()
		//{
		//	Strings.Add("<placeholder>");
		//}

		//public void RemoveSelectedListItem()
		//{
		//	if (Strings != null)
		//	{
		//		Strings.Remove(SelectedString);
		//	}
		//}
	}
}
