using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoMissingObjectEntryComparer : IEqualityComparer<DtoMissingObjectEntry>
{
	public bool Equals(DtoMissingObjectEntry? x, DtoMissingObjectEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.DatName == y.DatName
			&& x.DatChecksum == y.DatChecksum
			&& x.ObjectType == y.ObjectType;
	}

	public int GetHashCode([DisallowNull] DtoMissingObjectEntry obj)
		=> HashCode.Combine(obj.DatName, obj.DatChecksum, obj.ObjectType);
}
