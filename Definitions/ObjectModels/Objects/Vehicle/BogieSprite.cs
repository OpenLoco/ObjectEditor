using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Vehicle;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BogieSprite : ILocoStruct
{
	public uint8_t RollStates { get; set; }
	public BogieSpriteFlags Flags { get; set; }
	public uint8_t Width { get; set; }
	public uint8_t HeightNegative { get; set; }
	public uint8_t HeightPositive { get; set; }
	public uint8_t _NumRollSprites { get; set; }
	public uint32_t _FlatImageIds { get; set; }
	public uint32_t _GentleImageIds { get; set; }
	public uint32_t _SteepImageIds { get; set; }

	public uint8_t NumRollSprites { get; set; }
	public Dictionary<BogieSpriteSlopeType, List<int>> ImageIds { get; set; } = new();
	public int NumImages { get; set; }

	public bool Validate() => true;
}
