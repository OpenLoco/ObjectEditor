using Definitions;
using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public abstract class DbReferenceObject : IDbReferenceData, IHasId
	{
		#region IDbReferenceData

		public int Id { get; set; }

		public required string Name { get; set; }

		#endregion
	}
}
