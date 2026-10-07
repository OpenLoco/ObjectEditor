using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public interface IDbObjectSource
{
	ObjectSource ObjectSource { get; set; }
}
