using Definitions;

namespace OpenLoco.Definitions.Database
{
	public interface IDbReferenceData : IHasId
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
