using Definitions;

namespace OpenLoco.Definitions.Database
{
	//[Index(nameof(GuidId), IsUnique = true)]
	public abstract class DbIdObject : IHasId
	{
		public int Id { get; set; }
		public Guid? GuidId { get; set; }
	}
}
