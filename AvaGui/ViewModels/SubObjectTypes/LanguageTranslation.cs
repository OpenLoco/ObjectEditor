using OpenLoco.Dat.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace AvaGui.ViewModels
{
	public class LanguageTranslation(LanguageId language, string translation) : ReactiveObject
	{
		[Reactive] public LanguageId Language { get; set; } = language;

		[Reactive] public string Translation { get; set; } = translation;
	}
}
