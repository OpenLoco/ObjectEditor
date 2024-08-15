using Microsoft.EntityFrameworkCore;

namespace Schema
{

	[PrimaryKey("Name")]
	public class TblAuthor
	{
		public string Name { get; set; }
	}
}
