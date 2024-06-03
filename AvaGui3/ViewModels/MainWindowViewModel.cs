using OpenLoco.ObjectEditor.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace AvaGui3.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		[Reactive] public string? SelectedString { get; set; }
		[Reactive] public ObservableCollection<string> Strings { get; set; }
		[Reactive] public ObservableCollection<LanguageTranslation> TranslationTable { get; set; }

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
