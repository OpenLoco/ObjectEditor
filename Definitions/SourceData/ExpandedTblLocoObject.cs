using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.SourceData
{
	// this must be done because eager-loading related many-to-many data in entity framework is recursive and cannot be turned off...
	public record ExpandedTbl<T, TPack>(T Object, ICollection<TblAuthor> Authors, ICollection<TblTag> Tags, ICollection<TPack> Packs);
	public record ExpandedTblLookup<T, U, TPack>(T Object, U Lookup, ICollection<TblAuthor> Authors, ICollection<TblTag> Tags, ICollection<TPack> Packs);
	public record ExpandedTblPack<T, TPackItem>(T Pack, ICollection<TPackItem> Items, ICollection<TblAuthor> Authors, ICollection<TblTag> Tags);
}
