namespace Definitions.Database.Base;

public abstract class DbIdObject : IHasId
{
	public UniqueObjectId Id { get; set; }
}
