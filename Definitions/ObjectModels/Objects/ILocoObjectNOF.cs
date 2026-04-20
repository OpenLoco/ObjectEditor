using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects;

public interface ILocoObjectNOF
{
	public string DisplayName { get; set; }
	public string InternalName { get; set; }
	public ObjectType ObjectType { get; set; }
	public ObjectSource ObjectSource { get; set; }
}
