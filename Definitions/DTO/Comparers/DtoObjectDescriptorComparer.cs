using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoObjectDescriptorComparer : IEqualityComparer<DtoObjectPostResponse>
{
	public bool Equals(DtoObjectPostResponse? x, DtoObjectPostResponse? y)
	{
		if (x is null || y is null)
		{
			return false;
		}

		return x.Id == y.Id
			&& x.Name == y.Name
			&& x.DisplayName == y.DisplayName
			&& x.DatChecksum == y.DatChecksum
			&& x.Description == y.Description
			&& x.ObjectSource == y.ObjectSource
			&& x.ObjectType == y.ObjectType
			&& x.VehicleType == y.VehicleType
			&& x.Availability == y.Availability
			&& x.CreatedDate == y.CreatedDate
			&& x.ModifiedDate == y.ModifiedDate
			&& x.UploadedDate == y.UploadedDate
			&& new DtoLicenceEntryComparer().Equals(x.Licence, y.Licence)
			&& x.Authors.SequenceEqual(y.Authors, new DtoAuthorEntryComparer())
			&& x.Tags.SequenceEqual(y.Tags, new DtoTagEntryComparer())
			&& x.ObjectPacks.SequenceEqual(y.ObjectPacks, new DtoItemPackEntryComparer())
			&& x.DatObjects.SequenceEqual(y.DatObjects, new DtoDatObjectEntryComparer())
			&& x.StringTable.Equals(y.StringTable);
	}

	public int GetHashCode([DisallowNull] DtoObjectPostResponse obj)
		=> obj.GetHashCode();
}
