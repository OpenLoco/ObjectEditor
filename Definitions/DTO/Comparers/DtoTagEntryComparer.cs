using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoTagEntryComparer : IEqualityComparer<DtoTagEntry>
{
	public bool Equals(DtoTagEntry? x, DtoTagEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.Id == y.Id && x.Name == y.Name;
	}

	public int GetHashCode([DisallowNull] DtoTagEntry obj)
		=> HashCode.Combine(obj.Id, obj.Name);
}
