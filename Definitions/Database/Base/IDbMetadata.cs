using Definitions.Database.ReferenceDataTables;

namespace Definitions.Database.Base;

public interface IDbMetadata
{
	ICollection<TblTag> Tags { get; set; }

	TblLicence? Licence { get; set; }

	ICollection<TblAuthor> Authors { get; set; }
}
