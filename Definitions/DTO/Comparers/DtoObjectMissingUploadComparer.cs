using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoObjectMissingUploadComparer : IEqualityComparer<DtoObjectMissingPost>
{
	public bool Equals(DtoObjectMissingPost? x, DtoObjectMissingPost? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return
			x.DatName == y.DatName
			&& x.DatChecksum == y.DatChecksum
			&& x.ObjectType == y.ObjectType;
	}

	public int GetHashCode([DisallowNull] DtoObjectMissingPost obj)
		=> HashCode.Combine(obj.DatName, obj.DatChecksum, obj.ObjectType);
}
