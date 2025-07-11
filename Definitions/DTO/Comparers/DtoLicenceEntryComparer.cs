using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoLicenceEntryComparer : IEqualityComparer<DtoLicenceEntry>
{
	public bool Equals(DtoLicenceEntry? x, DtoLicenceEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.Id == y.Id
			&& x.Name == y.Name
			&& x.Text == y.Text;
	}

	public int GetHashCode([DisallowNull] DtoLicenceEntry obj)
		=> HashCode.Combine(obj.Id, obj.Name, obj.Text);
}
