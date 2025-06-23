using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.DTO
{
	public record DtoStringTableEntry(
		DbKey Id,
		string RowName,
		LanguageId RowLanguage,
		string RowText,
		DbKey ObjectId);

	public record DtoStringTableDescriptor(
		Dictionary<string, Dictionary<LanguageId, string>> Table,
		DbKey ObjectId);
}
