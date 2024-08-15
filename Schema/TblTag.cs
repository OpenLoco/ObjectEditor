using Microsoft.EntityFrameworkCore;

namespace Schema
{
	[PrimaryKey("Name")]
	public class TblTag
	{
		public string Name { get; set; }
	}
}
