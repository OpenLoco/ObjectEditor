namespace OpenLoco.Definitions.DTO
{
	public interface IHasId
	{
		int Id { get; }
	}

	public record DtoAuthorEntry(
		int Id,
		string Name) : IHasId;
}
