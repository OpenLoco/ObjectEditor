using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoItemPackEntryComparer : IEqualityComparer<DtoItemPackEntry>
{
	public bool Equals(DtoItemPackEntry? x, DtoItemPackEntry? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.Id == y.Id
			&& x.Name == y.Name
			&& x.Description == y.Description
			&& x.CreatedDate == y.CreatedDate
			&& x.ModifiedDate == y.ModifiedDate
			&& x.UploadedDate == y.UploadedDate
			&& new DtoLicenceEntryComparer().Equals(x.Licence, y.Licence);
	}

	public int GetHashCode([DisallowNull] DtoItemPackEntry obj)
		=> HashCode.Combine(obj.Id, obj.Name, obj.Description, obj.CreatedDate, obj.ModifiedDate, obj.UploadedDate, obj.Licence);
}
