using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.SourceData
{
	// this must be done because eager-loading related many-to-many data in entity framework is recursive and cannot be turned off...
	public record ExpandedTblLocoObject(TblLocoObject Object, ICollection<TblAuthor> Authors, ICollection<TblTag> Tags, ICollection<TblLocoObjectPack> ObjectPacks);
}
