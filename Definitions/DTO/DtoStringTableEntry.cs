using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.DTO
{
	public record DtoStringTableEntry(
		int Id,
		string RowName,
		LanguageId RowLanguage,
		string RowText,
		int ObjectId);

	public record DtoStringTableDescriptor(
		Dictionary<string, Dictionary<LanguageId, string>> Table,
		int ObjectId);
}
