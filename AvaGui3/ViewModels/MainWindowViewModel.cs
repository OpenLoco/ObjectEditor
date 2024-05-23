using OpenLoco.ObjectEditor.Data;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace AvaGui3.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private string? _selectedString;
		public string? SelectedString
		{
			get => _selectedString;
			set => _ = this.RaiseAndSetIfChanged(ref _selectedString, value);
		}

		ObservableCollection<string> _strings;
		public ObservableCollection<string> Strings
		{
			get => _strings;
			set => this.RaiseAndSetIfChanged(ref _strings, value);
		}

		ObservableCollection<LanguageTranslation> _translationTable;
		public ObservableCollection<LanguageTranslation> TranslationTable
		{
			get => _translationTable;
			set => this.RaiseAndSetIfChanged(ref _translationTable, value);
		}

		public void SelectedStringChanged()
		{
			if (SelectedString != null) // && Table.Table.ContainsKey(SelectedString))
			{
				TranslationTable = new ObservableCollection<LanguageTranslation>(Table[SelectedString]
					.Select(kvp => new LanguageTranslation(kvp.Key, kvp.Value)));

				foreach (var kvp in TranslationTable)
				{
					_ = kvp.WhenAnyValue(o => o.Translation)
						.Subscribe(_ => Table[SelectedString][kvp.Language] = kvp.Translation);
				}
			}
		}

		Dictionary<string, Dictionary<LanguageId, string>> _table1;
		Dictionary<string, Dictionary<LanguageId, string>> _table2;
		Dictionary<string, Dictionary<LanguageId, string>> Table;

		public MainWindowViewModel()
		{
			_table1 = new Dictionary<string, Dictionary<LanguageId, string>>
			{
				{
					"Name",
					new Dictionary<LanguageId, string>
					{
						{ LanguageId.English_UK, "Dollar" },
						{ LanguageId.English_US, "Dollar" },
						{ LanguageId.German, "Deutschmark" }
					}
				},
				{
					"Text",
					new Dictionary<LanguageId, string>
					{
						{ LanguageId.English_UK, "K" },
						{ LanguageId.English_US, "N" },
						{ LanguageId.German, "L" }
					}
				},
			};

			_table2 = new Dictionary<string, Dictionary<LanguageId, string>>
			{
				{
					"Name2",
					new Dictionary<LanguageId, string>
					{
						{ LanguageId.English_UK, "Dollar2" },
						{ LanguageId.English_US, "Dollar2" },
						{ LanguageId.German, "Deutschmark2" }
					}
				},
				{
					"Text2",
					new Dictionary<LanguageId, string>
					{
						{ LanguageId.English_UK, "K2" },
						{ LanguageId.English_US, "N2" },
						{ LanguageId.German, "L2" }
					}
				},
			};

			Table = _table1;

			_ = this.WhenAnyValue(o => o.Table)
				.Subscribe(_ => Strings = new ObservableCollection<string>(Table.Keys));

			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(_ => SelectedStringChanged());

		}
	}
}
