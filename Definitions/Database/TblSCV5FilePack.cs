using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblSCV5FilePack
	{
		public required int Id { get; set; }

		public required string Name { get; set; }

		public string? Description { get; set; }

		public TblAuthor? Author { get; set; }

		public ICollection<TblSCV5File> Files { get; set; }
	}
}
