using OpenLoco.ObjectEditor.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaGui3.ViewModels
{
	public record LanguageTranslation(LanguageId Language, string Translation);

	public class MainWindowViewModel : ViewModelBase
	{
		public ObservableCollection<LanguageTranslation> TranslationTable { get; }

		public MainWindowViewModel()
		{
			var people = new List<LanguageTranslation>
			{
				new LanguageTranslation(LanguageId.English_UK, "Dollar"),
				new LanguageTranslation(LanguageId.English_US, "Dollar"),
				new LanguageTranslation(LanguageId.German, "Deutschmark")
			};
			TranslationTable = new ObservableCollection<LanguageTranslation>(people);
		}
	}
}
