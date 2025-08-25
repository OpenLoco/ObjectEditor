namespace Definitions.ObjectModels.Objects.TownNames;

public class Category
{
	public uint8_t Count { get; set; }
	public uint8_t Bias { get; set; }
	public uint16_t Offset { get; set; }
}

public class TownNamesObject : ILocoStruct
{
	public List<Category> Categories { get; set; } = [];

	public bool Validate() => true;
}
