using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(ObjectId))]
	[Index(nameof(Text))]
	public class TblStringTableRow : DbIdObject
	{
		public required string Name { get; set; }
		public required LanguageId Language { get; set; }
		public required string Text { get; set; }

		public required UniqueObjectId ObjectId { get; set; } // FK, the TblObject that owns this string
		public TblObject Object { get; set; } // navigation property. a stringtable object must ALWAYS reference an OL object
	}
}
