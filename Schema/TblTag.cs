using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Db.Schema
{
	[PrimaryKey("Name")]
	public class TblTag
	{
		public string Name { get; set; }
	}
}
