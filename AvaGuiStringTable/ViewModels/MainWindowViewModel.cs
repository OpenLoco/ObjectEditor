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

			_ = this.WhenAnyValue(o => o.Table)
				.Subscribe(_ => Strings = new ObservableCollection<string>(Table.Keys));

			_ = this.WhenAnyValue(o => o.SelectedString)
				.Subscribe(_ => SelectedStringChanged());
		}
	}
}
