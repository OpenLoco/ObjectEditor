namespace Definitions.ObjectModels.Objects.Steam;

public class SteamObject : ILocoStruct
{
	public uint8_t NumStationaryTicks { get; set; }
	public uint8_t SpriteWidth { get; set; }
	public uint8_t SpriteHeightNegative { get; set; }
	public uint8_t SpriteHeightPositive { get; set; }
	public SteamObjectFlags Flags { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
