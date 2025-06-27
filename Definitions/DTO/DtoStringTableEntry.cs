using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.DTO
{
	public record DtoStringTableEntry(
		UniqueObjectId Id,
		string RowName,
		LanguageId RowLanguage,
		string RowText,
		UniqueObjectId ObjectId);

	public record DtoStringTableDescriptor(
		Dictionary<string, Dictionary<LanguageId, string>> Table,
		UniqueObjectId ObjectId);
}
