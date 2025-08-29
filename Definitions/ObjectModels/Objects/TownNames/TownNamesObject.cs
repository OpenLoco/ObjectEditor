namespace Definitions.ObjectModels.Objects.TownNames;

public class TownNamesObject : ILocoStruct
{
	public List<Category> Categories { get; set; } = [];

	public bool Validate() => true;
}
