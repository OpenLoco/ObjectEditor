namespace Definitions.Database
{
	public abstract class DbIdObject : IHasId
	{
		public UniqueObjectId Id { get; set; }
	}
}
