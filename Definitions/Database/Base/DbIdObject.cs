using Definitions;

namespace OpenLoco.Definitions.Database
{
	//[Index(nameof(GuidId), IsUnique = true)]
	public abstract class DbIdObject : IHasId
	{
		public DbKey Id { get; set; }
	}
}
