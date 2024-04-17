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

		ObservableCollection<LanguageRow> _selectedTranslations;
		public ObservableCollection<LanguageRow> SelectedTranslations
		{
			get => _selectedTranslations;
			set => this.RaiseAndSetIfChanged(ref _selectedTranslations, value);
		}

		private LanguageRow? _selectedTranslation;
		public LanguageRow? SelectedTranslation
		{
			get => _selectedTranslation;
			set => _ = this.RaiseAndSetIfChanged(ref _selectedTranslation, value);
		}

		public StringTable Table { get; set; }

		public void SelectedStringChanged()
		{
			if (SelectedString != null && Table.Table.ContainsKey(SelectedString))
			{
				SelectedTranslations = new ObservableCollection<LanguageRow>();
				foreach (var x in Table.Table[SelectedString])
				{
					var newRow = new LanguageRow(x.Key, x.Value);
					_ = newRow.WhenAnyValue(o => o.Translation)
						.Subscribe(o => UpdateTranslation());
					SelectedTranslations.Add(newRow);
				}
			}
		}
		public void UpdateTranslation()
		{
			if (SelectedString != null
				&& SelectedTranslation != null
				&& Table.Table.ContainsKey(SelectedString)
				&& Table.Table[SelectedString].ContainsKey(SelectedTranslation.Language))
			{
				Table.Table[SelectedString][SelectedTranslation.Language] = SelectedTranslation.Translation;
			}
		}

		public DictionaryViewModel()
		{
			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(o => SelectedStringChanged());

			_ = this.WhenAnyValue(o => o.SelectedTranslation)
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
