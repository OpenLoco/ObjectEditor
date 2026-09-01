using System.Diagnostics.CodeAnalysis;

namespace Definitions.DTO.Comparers;

public class DtoObjectDescriptorComparer : IEqualityComparer<DtoObjectPostResponse>
{
	private static readonly DtoLicenceEntryComparer s_licenceComparer = new();
	private static readonly DtoAuthorEntryComparer s_authorComparer = new();
	private static readonly DtoTagEntryComparer s_tagComparer = new();
	private static readonly DtoItemPackEntryComparer s_packComparer = new();
	private static readonly DtoDatObjectEntryComparer s_datComparer = new();

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
			&& s_licenceComparer.Equals(x.Licence, y.Licence)
			&& x.Authors.SequenceEqual(y.Authors, s_authorComparer)
			&& x.Tags.SequenceEqual(y.Tags, s_tagComparer)
			&& x.ObjectPacks.SequenceEqual(y.ObjectPacks, s_packComparer)
			&& x.DatObjects.SequenceEqual(y.DatObjects, s_datComparer)
			&& x.StringTable.Equals(y.StringTable);
	}

	public int GetHashCode([DisallowNull] DtoObjectPostResponse obj)
		=> obj.GetHashCode();
}
