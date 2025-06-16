using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(ObjectId))]
	[Index(nameof(RowText))]
	public class TblStringTable : DbIdObject
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; }
		public required string RowName { get; set; }
		public required LanguageId RowLanguage { get; set; }
		public required string RowText { get; set; }

		public required int ObjectId { get; set; } // FK, the TblObject that owns this string
		public TblObject Object { get; set; } // navigation property. a stringtable object must ALWAYS reference an OL object
	}
}
