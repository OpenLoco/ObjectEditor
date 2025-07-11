using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoAuthorEntryComparer : IEqualityComparer<DtoAuthorEntry>
{
	public bool Equals(DtoAuthorEntry? x, DtoAuthorEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.Id == y.Id && x.Name == y.Name;
	}

	public int GetHashCode([DisallowNull] DtoAuthorEntry obj)
		=> HashCode.Combine(obj.Id, obj.Name);
}
