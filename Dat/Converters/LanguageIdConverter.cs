using Dat.Data;
using Definitions.ObjectModels.Types;

namespace Dat.Converters;

public static class LanguageIdConverter
{
	public static DatLanguageId Convert(this LanguageId languageId)
		=> languageId switch
		{
			LanguageId.English_UK => DatLanguageId.English_UK,
			LanguageId.English_US => DatLanguageId.English_US,
			LanguageId.French => DatLanguageId.French,
			LanguageId.German => DatLanguageId.German,
			LanguageId.Spanish => DatLanguageId.Spanish,
			LanguageId.Italian => DatLanguageId.Italian,
			LanguageId.Dutch => DatLanguageId.Dutch,
			LanguageId.Swedish => DatLanguageId.Swedish,
			LanguageId.Japanese => DatLanguageId.Japanese,
			LanguageId.Korean => DatLanguageId.Korean,
			LanguageId.Chinese_Simplified => DatLanguageId.Chinese_Simplified,
			LanguageId.Chinese_Traditional => DatLanguageId.Chinese_Traditional,
			LanguageId.id_12 => DatLanguageId.id_12,
			LanguageId.Portuguese => DatLanguageId.Portuguese,
			_ => throw new NotImplementedException(),
		};

	public static LanguageId Convert(this DatLanguageId languageId)
		=> languageId switch
		{
			DatLanguageId.English_UK => LanguageId.English_UK,
			DatLanguageId.English_US => LanguageId.English_US,
			DatLanguageId.French => LanguageId.French,
			DatLanguageId.German => LanguageId.German,
			DatLanguageId.Spanish => LanguageId.Spanish,
			DatLanguageId.Italian => LanguageId.Italian,
			DatLanguageId.Dutch => LanguageId.Dutch,
			DatLanguageId.Swedish => LanguageId.Swedish,
			DatLanguageId.Japanese => LanguageId.Japanese,
			DatLanguageId.Korean => LanguageId.Korean,
			DatLanguageId.Chinese_Simplified => LanguageId.Chinese_Simplified,
			DatLanguageId.Chinese_Traditional => LanguageId.Chinese_Traditional,
			DatLanguageId.id_12 => LanguageId.id_12,
			DatLanguageId.Portuguese => LanguageId.Portuguese,
			_ => throw new NotImplementedException(),
		};
}
