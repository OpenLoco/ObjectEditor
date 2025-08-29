using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Steam;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class SteamImageAndHeight
{
	public uint8_t ImageOffset { get; set; }
	public uint8_t Height { get; set; }
}
