using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoObjectMissingUploadComparer : IEqualityComparer<DtoObjectMissingUpload>
{
	public bool Equals(DtoObjectMissingUpload? x, DtoObjectMissingUpload? y)
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

	public int GetHashCode([DisallowNull] DtoObjectMissingUpload obj)
		=> HashCode.Combine(obj.DatName, obj.DatChecksum, obj.ObjectType);
}
