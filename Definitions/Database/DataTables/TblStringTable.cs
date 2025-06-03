using Definitions;
using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(ObjectId))]
	[Index(nameof(Text))]
	public class TblStringTable : IHasId
	{
		public int Id { get; set; }

		public required int StringIndex { get; set; } // eg 1 is usually name

		public required LanguageId Language { get; set; }
		public required string Text { get; set; }

		public required int ObjectId { get; set; } // FK, the TblObject that owns this string
		public required TblObject Object { get; set; } // navigation property. a stringtable object must ALWAYS reference an OL object
	}
}
