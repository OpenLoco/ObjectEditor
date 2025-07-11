namespace Definitions.Database
{
	public interface IDbMetadata
	{
		ICollection<TblTag> Tags { get; set; }

		TblLicence? Licence { get; set; }

		ICollection<TblAuthor> Authors { get; set; }
	}
}
