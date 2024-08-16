using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Db.Schema
{

	[PrimaryKey("Name")]
	public class TblAuthor
	{
		public string Name { get; set; }
	}
}
