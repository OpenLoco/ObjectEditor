using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoObjectMissingEntryComparer : IEqualityComparer<DtoObjectMissingEntry>
{
	public bool Equals(DtoObjectMissingEntry? x, DtoObjectMissingEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return
			x.Id == y.Id
			&& x.DatName == y.DatName
			&& x.DatChecksum == y.DatChecksum
			&& x.ObjectType == y.ObjectType;
	}

	public int GetHashCode([DisallowNull] DtoObjectMissingEntry obj)
		=> HashCode.Combine(obj.Id, obj.DatName, obj.DatChecksum, obj.ObjectType);
}
