using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoDatObjectEntryComparer : IEqualityComparer<DtoDatObjectEntry>
{
	public bool Equals(DtoDatObjectEntry? x, DtoDatObjectEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.Id == y.Id
			&& x.DatName == y.DatName
			&& x.DatChecksum == y.DatChecksum
			&& x.xxHash3 == y.xxHash3
			&& x.ObjectId == y.ObjectId
			&& x.DatBytesAsBase64 == y.DatBytesAsBase64;
	}

	public int GetHashCode([DisallowNull] DtoDatObjectEntry obj)
		=> HashCode.Combine(obj.Id, obj.DatName, obj.DatChecksum, obj.xxHash3, obj.ObjectId, obj.DatBytesAsBase64);
}
