using OpenLoco.ObjectEditor.Data;
using ReactiveUI;

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
}
