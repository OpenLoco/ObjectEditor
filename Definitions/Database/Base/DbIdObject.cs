using Definitions;

namespace OpenLoco.Definitions.Database
{
	public abstract class DbIdObject : IHasId
	{
		public UniqueObjectId Id { get; set; }
	}
}
