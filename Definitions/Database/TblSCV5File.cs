using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(DatName), nameof(DatChecksum), IsDescending = [true, false], IsUnique = true)]
	[Index(nameof(UniqueName), IsUnique = true)]
	public class TblSCV5File
	{
		public int Id { get; set; }

		public required string UniqueName { get; set; }

		#region OriginalDatData

		public required string DatName { get; set; }

		public required uint DatChecksum { get; set; }

		#endregion

		public ObjectSource ObjectSource { get; set; }

		public string? Description { get; set; }

		public ICollection<TblAuthor> Authors { get; set; }

		public DateTimeOffset? CreationDate { get; set; }

		public DateTimeOffset? LastEditDate { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTimeOffset UploadDate { get; set; }

		public ICollection<TblTag> Tags { get; set; }

		public ICollection<TblSCV5FilePack> SCV5FilePacks { get; set; }

		public TblLicence? Licence { get; set; }
	}
}
