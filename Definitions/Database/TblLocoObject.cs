using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(OriginalName), nameof(OriginalChecksum), IsDescending = [true, false], IsUnique = true)]
	[Index(nameof(Name), IsUnique = true)]
	[Index(nameof(PathOnDisk), IsUnique = true)]
	public class TblLocoObject
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string PathOnDisk { get; set; }

		#region OriginalDatdata

		public string OriginalName { get; set; }

		public uint OriginalChecksum { get; set; }

		#endregion

		public bool IsVanilla { get; set; }

		public ObjectType ObjectType { get; set; }

		public VehicleType? VehicleType { get; set; }

		#region Metadata

		public string? Description { get; set; }

		public ICollection<TblAuthor> Authors { get; set; }

		public DateTimeOffset? CreationDate { get; set; }

		public DateTimeOffset? LastEditDate { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTimeOffset? UploadDate { get; set; }

		public ICollection<TblTag> Tags { get; set; }

		public ICollection<TblModpack> Modpacks { get; set; }

		public ObjectAvailability Availability { get; set; }

		public TblLicence? Licence { get; set; }

		#endregion
	}
}