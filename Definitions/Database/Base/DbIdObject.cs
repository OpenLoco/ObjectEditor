using Definitions;

namespace OpenLoco.Definitions.Database
{
	//[Index(nameof(GuidId), IsUnique = true)]
	public abstract class DbIdObject : IHasId
	{
		public UniqueObjectId Id { get; set; }
	}
}
