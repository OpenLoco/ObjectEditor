namespace OpenLoco.Definitions.Database
{
	public interface IDbStandardData : IDbReferenceData
	{
		public string? Description { get; set; }
	}
}
