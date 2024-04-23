using OpenLoco.ObjectEditor.Data;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace AvaGui3.ViewModels
{
	public class LanguageTranslation : ReactiveObject
	{
		public LanguageTranslation(LanguageId language, string translation)
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

	public class MainWindowViewModel : ViewModelBase
	{
		private string? _selectedString;
		public string? SelectedString
		{
			get => _selectedString;
			set => _ = this.RaiseAndSetIfChanged(ref _selectedString, value);
		}

		public ObservableCollection<string> Strings { get; }

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

		Dictionary<string, Dictionary<LanguageId, string>> Table;

		public MainWindowViewModel()
		{
			Table = new Dictionary<string, Dictionary<LanguageId, string>>
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

			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(_ => SelectedStringChanged());

			Strings = new ObservableCollection<string>(Table.Keys);
		}
	}
}
