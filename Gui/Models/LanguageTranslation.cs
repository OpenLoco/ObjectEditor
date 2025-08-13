using Dat.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Gui.Models;

public class LanguageTranslation(DatLanguageId language, string translation) : ReactiveObject
{
	[Reactive]
	public DatLanguageId Language { get; set; } = language;

	[Reactive]
	public string Translation { get; set; } = translation;
}
